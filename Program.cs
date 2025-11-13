

using System.Net.Http.Json;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    private static async Task<decimal> CurrentAssetPrice(string asset)
    {
        string url = $"https://brapi.dev/api/quote/{asset}";
        try
        {
            BrapiResponse? response = await client.GetFromJsonAsync<BrapiResponse>(url);

            if(response != null)
            {
                QuoteResult firstQuote = response.results.First();
                return firstQuote.regularMarketPrice;
            }

            Console.WriteLine("Requisção funcioou mas não retornou nenhum dado");
            return 0;
        }
        catch(HttpRequestException e)
        {
            Console.WriteLine($"Erro de requisição HTTP: {e.Message}");
            return 0;
        }
        catch(JsonException e)
        {
            Console.WriteLine($"Erro ao processar JSON: {e.Message}");
            return 0;
        }
        catch(Exception e)
        {
            Console.WriteLine($"Erro inesperado: {e.Message}");
            return 0;
        }
    }

    static async Task Main(string[] args)
    {   
        //validando entradas
        if (args.Length != 3)
        {
            Console.WriteLine("ERRO: número incorreto de argumentos.");
            Console.WriteLine("Faça: dotnet run <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>");
            return;
        }

        if (!decimal.TryParse(args[1], out decimal sell_price))
        {
            Console.WriteLine($"ERRO: entrada <PRECO_VENDA> inválida ");
            return;
        }

        if (!decimal.TryParse(args[2], out decimal buy_price))
        {
            Console.WriteLine($"ERRO: entrada <PRECO_COMPRA> inválida {buy_price}");
            return;
        }

        if(sell_price <= buy_price)
        {
            Console.WriteLine($"ERRO: Preco de venda ({sell_price}) menor ou igual ao preco de compra ({buy_price})");
            return;
        }

        string asset = args[0];

        Console.WriteLine($"Monitorando {asset} (Venda: {sell_price} | Compra: {buy_price})");

        //configurando serviço de email
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();

        string? destEmail = config["DestEmail"];

        SmtpSettings smtpSettings = new SmtpSettings();
        config.GetSection("Smtp").Bind(smtpSettings);

        EmailService emailService = new EmailService(smtpSettings);

        if (string.IsNullOrEmpty(destEmail))
        {
            Console.WriteLine("ERRO: A configuração 'DestEmail' não foi encotrada no arquivo de confirações.");
            Console.WriteLine("Adicione 'DestEmail' ao appsettings.json");
            return;
        }

        while (true)
        {
            decimal current_price = await CurrentAssetPrice(asset);

            if (current_price == 0)
            {
                return;
            }

            if(current_price < buy_price)
            {
                Console.WriteLine("Alerta de COMPRA!");
                Console.WriteLine($"Preco atual ({current_price}) menor que o preco de compra ({buy_price})");
                Console.WriteLine($"Enviando e-mail para {destEmail}...");
                
                decimal diff = Math.Round((buy_price - current_price) / buy_price * 100, 2);
                emailService.SendEmail(destEmail, $"Alerta de Compra ({asset})", $"{asset} está com valor {current_price}. {diff}% menor que {buy_price}.");

            }else if(current_price > sell_price)
            {
                Console.WriteLine("Alerta de VENDA!");
                Console.WriteLine($"Preco atual ({current_price}) maior que o preco de venda ({sell_price})");
                Console.WriteLine($"Enviando e-mail para {destEmail}...");

                decimal diff = (current_price - sell_price)/ sell_price * 100;
                emailService.SendEmail(destEmail, $"Alerta de Venda ({asset})", $"{asset} está com valor {current_price}. {diff}% maior que {buy_price}.");

            }
            else
            {
                Console.WriteLine($"Preco atual: {current_price}");
            }

            await Task.Delay(5000);
        }
    }
}