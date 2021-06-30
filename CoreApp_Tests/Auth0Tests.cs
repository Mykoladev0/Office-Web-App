using Xunit;
using ABKCAPI;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

public class Auth0Tests
{

    private readonly TestServer _server;
    private readonly HttpClient _client;
    private IConfiguration _configuration;

    public Auth0Tests()
    {
        var basePath = Path.GetFullPath(@"../../..");
        var appPath = Path.GetFullPath(@"../../../../CoreApp");
        _configuration = new ConfigurationBuilder()
           .SetBasePath(basePath)
           .AddJsonFile("appsettings.json", optional: false)
           .Build();


        _server = new TestServer(new WebHostBuilder()
            .UseStartup<TestStartup>()
            .UseContentRoot(appPath)
            .UseConfiguration(_configuration));
        _client = _server.CreateClient();
    }

    // [Fact]
    public async Task LockedDashboardPageShouldRequireToken()
    {

        try
        {
            var response = await _client.GetAsync("/home/dashboardSecure");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
        catch (System.Exception x)
        {

            throw x;
        }
    }

}