using System.Text;

namespace CardValidator.Console;

public static class Base64UrlHelper
{
    public static string Encode(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Encode(bytes);
    }

    public static string Encode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static string Decode(string input)
    {
        var base64 = input.Replace('-', '+').Replace('_', '/');

        // Добавляем padding при необходимости
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
}