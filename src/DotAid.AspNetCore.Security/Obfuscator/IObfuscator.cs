namespace DotAid.AspNetCore.Security.Obfuscator;

public interface IObfuscator
{
    long Decode(string? obfuscatedId);

    bool TryDecode(string? obfuscatedId, out long id);
    
    bool TryDecode(string? obfuscatedId, out int id);

    string Encode(long id);

    string Encode(int id);

    bool IsValidInput(string? obfuscatedId);
}