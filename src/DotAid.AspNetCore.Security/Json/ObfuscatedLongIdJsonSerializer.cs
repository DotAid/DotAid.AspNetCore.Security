using System.Text.Json;
using System.Text.Json.Serialization;
using DotAid.AspNetCore.Security.Obfuscator;

namespace DotAid.AspNetCore.Security.Json;

public class ObfuscatedLongIdJsonSerializer : JsonConverter<HashedId<long>>
{
    private IObfuscator Obfuscator { get; }

    public ObfuscatedLongIdJsonSerializer(IObfuscator obfuscator)
    {
        Obfuscator = obfuscator;
    }

    public override HashedId<long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        if (Obfuscator.TryDecode(reader.GetString(), out long id))
        {
            throw new JsonException("Invalid hash.");
        }

        return id;
    }

    public override void Write(Utf8JsonWriter writer, HashedId<long> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Obfuscator.Encode(value));
    }
}