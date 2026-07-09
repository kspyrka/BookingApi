using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Integration;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    public Mock<IBookService> BookServiceMock { get; } = new();

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(x => x.ServiceType == typeof(IBookService));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddSingleton(
                BookServiceMock.Object);
        });
    }
}