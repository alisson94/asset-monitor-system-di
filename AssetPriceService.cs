using System.Net.Http.Json;
using System.Text.Json;

public class AssetPriceService
{
    private HttpClient httpClient;

    public AssetPriceService(HttpClient client) 
    {
        httpClient = client;
    }

    public async Task<decimal> CurrentAssetPrice(string asset, string apiBrapiKey)
    {   
        string url = $"https://brapi.dev/api/quote/{asset}?token={apiBrapiKey}";   
        
        try
        {   

            BrapiResponseModel? response = await httpClient.GetFromJsonAsync<BrapiResponseModel>(url);

            if(response != null)
            {
                QuoteResultModel firstQuote = response.results.First();
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

}