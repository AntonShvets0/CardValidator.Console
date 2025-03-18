using System.Text;
using CardValidator.Console.Models.Entities;
using CardValidator.Console.Models.Request;
using CardValidator.Console.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CardValidator.Console.Services;

public class CardsValidationService
{
    private readonly HttpClient _client;
    private readonly PaymentServiceOptions _options;

    public CardsValidationService(HttpClient client, IOptions<PaymentServiceOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<bool> ValidateCardAsync(string pan)
    {
        if (!IsValidCardNumber(pan)) return false;
        
        CardInfoRequest cardInfo = new()
        {
            CardInfo = new CardInfo { Pan = pan }
        };
        string jsonPayload = JsonConvert.SerializeObject(cardInfo);
        string jwsMessage = JwsHelper.CreateJwsMessage(jsonPayload, _options.KeyId, _options.SharedKey);
        StringContent content = new(jwsMessage, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _client.PostAsync(_options.Path, content);
        if (!response.IsSuccessStatusCode)
            return false;
        string responseContent = await response.Content.ReadAsStringAsync();
        return JwsHelper.DecodeJwsMessage(responseContent).Status == "Success";
    }
    
    private bool IsValidCardNumber(string pan)
    {
        return !string.IsNullOrEmpty(pan) && pan.All(char.IsDigit) && pan.Length >= 13 && pan.Length <= 19;
    }
}