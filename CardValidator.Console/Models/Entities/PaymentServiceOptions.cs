namespace CardValidator.Console.Models.Entities;

public class PaymentServiceOptions
{
    public string Host { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
    public string SharedKey { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}