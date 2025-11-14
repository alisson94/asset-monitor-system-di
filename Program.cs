using Microsoft.Extensions.Configuration;

class Program
{
    private static readonly HttpClient client = new HttpClient();

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
            Console.WriteLine($"ERRO: entrada <PRECO_VENDA> inválida");
            return;
        }

        if (!decimal.TryParse(args[2], out decimal buy_price))
        {
            Console.WriteLine($"ERRO: entrada <PRECO_COMPRA> inválida");
            return;
        }

        if(sell_price <= buy_price)
        {
            Console.WriteLine($"ERRO: Preco de venda R$({sell_price:F2}) menor ou igual ao preco de compra R$({buy_price:F2})");
            return;
        }

        string asset = args[0];

        Console.WriteLine($"Monitorando {asset} (Venda: R$ {sell_price:F2} | Compra: R$ {buy_price:F2})");


        //configurando serviço de email
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();
        
        SmtpSettings smtpSettings = new SmtpSettings();
        config.GetSection("Smtp").Bind(smtpSettings);

        EmailService emailService = new EmailService(smtpSettings);
        
        string? destEmail = config["DestEmail"];

        if (string.IsNullOrEmpty(destEmail))
        {
            Console.WriteLine("ERRO: A configuração 'DestEmail' não foi encotrada no arquivo de confirações.");
            Console.WriteLine("Adicione 'DestEmail' ao appsettings.json");
            return;
        }

        Console.WriteLine($"Emails serão enviados para: {destEmail}");


        string? apiBrapiKey = config["ApiKeyBrapi"];

        if (string.IsNullOrWhiteSpace(apiBrapiKey))
        {
            Console.WriteLine("ERRO: Chave API Brapi não encontrada.");
            return;
        }
        
        AssetPriceService assetPriceService = new AssetPriceService(client);
        
        int delay;
        try
        {
            delay = config.GetValue<int>("TimeoutBrapi", 60000);
        }
        catch (Exception)
        {
            Console.WriteLine($"Erro: 'TimeoutBrapi' inválido.");
            return;
        }

        while (true)
        {
            try
            {
                decimal current_price = await assetPriceService.CurrentAssetPrice(asset, apiBrapiKey);

                if (current_price == 0)
                {
                    Console.WriteLine("Não foi possivel achar o valor desse ativo.");
                    return;
                    
                }

                Console.WriteLine("\n==============================================\n");
                
                if(current_price < buy_price)
                {   
                    Console.WriteLine("Alerta de COMPRA!");
                    Console.WriteLine($"Preco atual (R$ {current_price:F2}) menor que o preco de compra (R$ {buy_price:F2})\n");
                    Console.WriteLine($"Enviando e-mail para {destEmail}...\n");

                    decimal diff = Math.Round((buy_price - current_price) / buy_price * 100, 2);
                    await emailService.SendEmail(destEmail, $"Alerta de Compra ({asset})", $"{asset} está com valor R$ {current_price:F2}. {diff}% menor que R$ {buy_price:F2}.");
                

                }else if(current_price > sell_price)
                {
                    Console.WriteLine("Alerta de VENDA!");
                    Console.WriteLine($"Preco atual (R$ {current_price:F2}) maior que o preco de venda (R$ {sell_price:F2})\n");
                    Console.WriteLine($"Enviando e-mail para {destEmail}...");

                    decimal diff = Math.Round((current_price - sell_price)/ sell_price * 100, 2);
                    await emailService.SendEmail(destEmail, $"Alerta de Venda ({asset})", $"{asset} está com valor R$ {current_price:F2}. {diff}% maior que R$ {sell_price:F2}.");

                }
                else
                {
                    Console.WriteLine($"Preco atual: R$ {current_price:F2}");
                }

                await Task.Delay(delay);
            }catch(Exception e)
            {
                Console.WriteLine($"Erro durante o loop: {e.Message}");
            }
        }
    }
}