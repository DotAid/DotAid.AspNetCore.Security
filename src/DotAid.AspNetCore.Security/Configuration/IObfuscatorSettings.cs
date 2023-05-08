namespace DotAid.AspNetCore.Security.Configuration;

public interface IObfuscatorSettings
{
    public Type GetObfuscatorType { get; }
    public string RouteConstraintName { get; set; }
}
