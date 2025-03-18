using System.Security.Cryptography;
using System.Text;
using CardValidator.Console.Models.Entities;
using CardValidator.Console.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CardValidator.Console.Utils;

public static class JwsHelper
{
    public static CardInfoResponse DecodeJwsMessage(string jwsMessage)
    {
        string[] parts = jwsMessage.Split('.');
        if (parts.Length != 3)
        {
            throw new InvalidOperationException("Invalid JWS format");
        }

        string payloadJson = Encoding.UTF8.GetString(Base64UrlHelper.Decode(parts[1]));
        return JsonConvert.DeserializeObject<CardInfoResponse>(payloadJson);
    }

    public static string CreateJwsMessage(string payload, string keyId, string sharedKey)
    {
        ProtectedHeader protectedHeader = new ()
        {
            Alg = "HS256",
            Kid = keyId,
            Signdate = DateTime.UtcNow.ToString("o"),
            Cty = "application/json"
        };

        JsonSerializerSettings jsonSerializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        string protectedHeaderJson = JsonConvert.SerializeObject(protectedHeader, jsonSerializerSettings);
        string protectedHeaderBase64 = Base64UrlHelper.Encode(Encoding.UTF8.GetBytes(protectedHeaderJson));
        string payloadBase64 = Base64UrlHelper.Encode(Encoding.UTF8.GetBytes(payload));

        string signingInput = $"{protectedHeaderBase64}.{payloadBase64}";
        string signature = SignPayload(signingInput, sharedKey);

        return $"{signingInput}.{signature}";
    }

    private static string SignPayload(string signingInput, string sharedKey)
    {
        using HMACSHA256 hmac = new(Convert.FromBase64String(sharedKey));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
        return Base64UrlHelper.Encode(hash);
    }
}