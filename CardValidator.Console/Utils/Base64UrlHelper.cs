namespace CardValidator.Console.Utils;

public class Base64UrlHelper
{
    public static string Encode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static byte[] Decode(string input)
    {
        string paddedInput = input.Replace('-', '+').Replace('_', '/');
        switch (paddedInput.Length % 4)
        {
            case 2: paddedInput += "=="; break;
            case 3: paddedInput += "="; break;
        }
        return Convert.FromBase64String(paddedInput);
    }
}