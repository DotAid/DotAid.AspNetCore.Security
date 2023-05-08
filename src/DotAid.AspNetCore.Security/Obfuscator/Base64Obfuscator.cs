using System.Text.RegularExpressions;
using DotAid.AspNetCore.Security.Configuration;

namespace DotAid.AspNetCore.Security.Obfuscator;

public partial class Base64Obfuscator : IObfuscator
{
    [GeneratedRegex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$", RegexOptions.Compiled)]
    private static partial Regex Base64ValidateRegex();

    private static readonly Regex CompiledRegex = Base64ValidateRegex();

    public Base64Obfuscator(Base64ObfuscatorSettings _)
    {
    }

    public long Decode(string? obfuscatedId)
    {
        if (obfuscatedId == null) throw new ArgumentNullException(nameof(obfuscatedId));
        var base64EncodedBytes = Convert.FromBase64String(obfuscatedId);
        return long.Parse(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
    }

    public bool TryDecode(string? obfuscatedId, out long id)
    {
        if (obfuscatedId == null)
        {
            id = long.MinValue;
            return false;
        }

        try
        {
            id = Decode(obfuscatedId);
        }
        catch (Exception)
        {
            id = long.MinValue;
            return false;
        }

        return true;
    }
    
    public bool TryDecode(string? obfuscatedId, out int id)
    {
        var result = TryDecode(obfuscatedId, out long longId);
        id = Convert.ToInt32(longId);

        return result;
    }

    public string Encode(long id) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(id.ToString()));

    public string Encode(int id) => Encode((long)id);

    public bool IsValidInput(string? obfuscatedId) =>
        CompiledRegex.IsMatch(obfuscatedId ?? throw new ArgumentNullException(nameof(obfuscatedId)));
}