using DotAid.AspNetCore.Security.Obfuscator;

namespace DotAid.AspNetCore.Security.Configuration;

public class Base64ObfuscatorSettings : BaseObfuscatorSettings, IObfuscatorSettings
{
    public Type GetObfuscatorType { get; } = typeof(Base64Obfuscator);
}