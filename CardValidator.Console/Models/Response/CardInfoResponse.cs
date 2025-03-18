using CardValidator.Console.Models.Entities;

namespace CardValidator.Console.Models.Response;

public class CardInfoResponse
{
    public string Id { get; set; }
        
    public CardInfo CardInfo { get; set; }
        
    public string Status { get; set; }
}