using System.Text.Json;
using System.Text.Json.Serialization;
using DotAid.AspNetCore.Security.Obfuscator;

namespace DotAid.AspNetCore.Security.Json;

public class ObfuscatedIntIdJsonSerializer : JsonConverter<HashedId<int>>
{
    private IObfuscator Obfuscator { get; }

    public ObfuscatedIntIdJsonSerializer(IObfuscator obfuscator)
    {
        Obfuscator = obfuscator;
    }

    public override HashedId<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        if (Obfuscator.TryDecode(reader.GetString(), out int id))
        {
            throw new JsonException("Invalid hash.");
        }

        return id;
    }

    public override void Write(Utf8JsonWriter writer, HashedId<int> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Obfuscator.Encode(value));
    }
}