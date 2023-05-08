using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace DotAid.AspNetCore.Security.Test.Setup;

public class TestFixture<TStartup> : IDisposable where TStartup : class
{
    public HttpClient Client { get; set; }

    private TestServer Server { get; }

    public TestFixture()
    {
        var builder = new WebHostBuilder().UseStartup<TStartup>();
        Server = new TestServer(builder);
        Client = Server.CreateClient();
        Client.BaseAddress = new Uri("http://localhost:5000");
    }

    public void Dispose()
    {
        Client.Dispose();
        Server.Dispose();
        GC.SuppressFinalize(this);
    }
}