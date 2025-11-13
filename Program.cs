

using System.Reflection.Emit;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("ERRO: número incorreto de argumentos.");
            Console.WriteLine("Faça: dotnet run <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>");
            return;
        }

        if (!decimal.TryParse(args[1], out decimal sell_price))
        {
            Console.WriteLine($"ERRO: entrada inválida {sell_price}");
            return;
        }

        if (!decimal.TryParse(args[2], out decimal buy_price))
        {
            Console.WriteLine($"ERRO: entrada inválida {buy_price}");
            return;
        }

        string asset = args[0];

        Console.WriteLine($"Monitorando {asset} (Venda: {sell_price} | Compra: {buy_price})");
    }
}