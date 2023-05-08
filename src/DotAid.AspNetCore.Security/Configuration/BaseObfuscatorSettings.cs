using System.Text.Json;

namespace DotAid.AspNetCore.Security.Configuration;

public abstract class BaseObfuscatorSettings
{
    public string RouteConstraintName { get; set; } = "ObfuscatedId";
}