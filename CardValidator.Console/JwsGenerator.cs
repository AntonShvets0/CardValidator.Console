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
        var encodedHeader = Base64UrlHelper.Encode(headerJson);
        var encodedPayload = Base64UrlHelper.Encode(payloadJson);

        // Создаем подпись
        var dataToSign = encodedHeader + "." + encodedPayload;
        var signature = CreateSignature(dataToSign);

        // Формируем JWS
        return dataToSign + "." + signature;
    }

    private string CreateSignature(string data)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(_sharedKey));
        
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Base64UrlHelper.Encode(hash);
    }
}