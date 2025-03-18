namespace CardValidator.Console.Models.Entities;

public class ProtectedHeader
{
    public string Alg { get; set; }
    public string Kid { get; set; }
    public string Signdate { get; set; }
    public string Cty { get; set; }
}