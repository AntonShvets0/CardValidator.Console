using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CardValidator.Console;

public class JwsGenerator
{
    private readonly string _keyId;
    private readonly string _sharedKey;
    private readonly string _algorithm;
    
    public JwsGenerator(string keyId, string sharedKey, string algorithm = "HS256")
    {
        _keyId = keyId;
        _sharedKey = sharedKey;
        _algorithm = algorithm;

        if (_algorithm != "HS256") // Так как предоставленный SharedKey = HS256, я счел не нужным реализовывать все алгоритмы: HS512, HS384
            throw new NotImplementedException();
    }

    public string Generate(object payload)
    {
        // Создаем protected header
        var header = new
        {
            alg = _algorithm,
            kid = _keyId,
            signdate = DateTime.UtcNow.ToString("o"),
            cty = "application/json"
        };

        // Сериализуем header и payload
        var headerJson = JsonSerializer.Serialize(header);
        var payloadJson = JsonSerializer.Serialize(payload);

        // Base64Url кодирование header и payload
        var encodedHeader = Base64UrlEncode(headerJson);
        var encodedPayload = Base64UrlEncode(payloadJson);

        // Создаем подпись
        var dataToSign = encodedHeader + "." + encodedPayload;
        var signature = CreateSignature(dataToSign);

        // Формируем JWS
        return dataToSign + "." + signature;
    }
    
    public string Decode(string input)
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

    private string CreateSignature(string data)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(_sharedKey));
        
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Base64UrlEncode(hash);
    }

    private string Base64UrlEncode(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Base64UrlEncode(bytes);
    }

    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}