using API;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Infrastructure;

public class ChuckWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(service =>
        {
            service.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                "Test", _ => {});
        });
        base.ConfigureWebHost(builder);
    }
}