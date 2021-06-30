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

public class ShowsAPITests
{

    private readonly TestServer _server;
    private readonly HttpClient _client;
    private IConfiguration _configuration;

    public ShowsAPITests()
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

    //[Fact(DisplayName ="If the date is Saturday, Sunday, or Monday, use the Previous Friday to retrieve upcoming shows")]
    //public async Task WeekendDateUsesFridayForUpcomingShows(){

    //    try
    //    {
    //        var response = await _client.GetAsync("/api/shows/GetUpcomingShows");
    //        //var content = response.Content.Read
    //        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    //    }
    //    catch (System.Exception x)
    //    {

    //        throw x;
    //    }
    //}

}