using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Typeracer.Context;
using Typeracer.Controllers;
using Typeracer.Models;

namespace TyperacerIntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); // Set the environment to Testing
        builder.ConfigureServices(services =>
        {
            // Remove the existing context registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Add a new in-memory database context
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestsDb"));
            
            var paragraphFiles = new Dictionary<string, List<Gamemode>>
            {
                { "paragraph1.txt", new List<Gamemode> { Gamemode.Standard, Gamemode.Hardcore } },
                { "paragraph2.txt", new List<Gamemode> { Gamemode.Short } }
            };

            services.AddSingleton(paragraphFiles);
            services.AddScoped<HomeController>();

            // Build the service provider and retrieve the DbContext
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        });
        
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }
}