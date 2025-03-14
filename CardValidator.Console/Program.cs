
using System.Text;
using System.Text.Json;
using CardValidator.Console.Models;

namespace CardValidator.Console;

public static class Program
{
    private const string Host = "acstopay.online";
    private const string KeyId = "47e8fde35b164e888a57b6ff27ec020f";
    private const string SharedKey = "ac/1LUdrbivclAeP67iDKX2gPTTNmP0DQdF+0LBcPE/3NWwUqm62u5g6u+GE8uev5w/VMowYXN8ZM+gWPdOuzg==";
    private const string ApiUrl = "https://{0}/api/testassignments/pan";
    private static readonly JwsGenerator _jwsGenerator = new(KeyId, SharedKey);
    
    public static async Task Main(string[] args)
    {
        while (true)
        {
            System.Console.WriteLine("Enter PAN or 'test' for test all scenarios: ");
            var pan = System.Console.ReadLine();
            if (pan == "test")
            {
                await RunTestScenariosAsync();
                return;
            }

            if (string.IsNullOrEmpty(pan)) continue;
            await ValidateCardAndWriteStatusAsync(pan);
        }
    }

    private static async Task ValidateCardAndWriteStatusAsync(string pan)
    {
        System.Console.WriteLine(
            await ValidateCardAsync(pan) ? "Successfully" : "Unsuccessfully"
        );
    }
    
    private static async Task<bool> ValidateCardAsync(string pan)
    {
        var jws = _jwsGenerator.Generate(new
        {
            CardInfo = new
            {
                Pan = pan
            }
        });

        using var httpClient = new HttpClient();
        var content = new StringContent(jws, Encoding.UTF8, "application/jose");
        var response = await httpClient.PostAsync(string.Format(ApiUrl, Host), content);

        if (!response.IsSuccessStatusCode)
            return false;

        var responseContent = await response.Content.ReadAsStringAsync();
                
        var responseParts = responseContent.Split('.');
        if (responseParts.Length < 2) 
            return false;

        var decodedPayload = Base64UrlHelper.Decode(responseParts[1]);
        var responseData = JsonSerializer.Deserialize<ResponseData>(decodedPayload);

        return responseData?.Status == "Success";
    }

    // Необязательная фича, для быстрых тестов
    private static async Task RunTestScenariosAsync()
    {
        var cards = new[] { "4111111111111111", "4627100101654724", "4486441729154030", "4024007123874108" };
        foreach (var card in cards)
            await ValidateCardAndWriteStatusAsync(card);
    }
}