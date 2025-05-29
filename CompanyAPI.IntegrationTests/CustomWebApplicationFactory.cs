using CompanyAPI.Domain.Entities;
using CompanyAPI.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> :WebApplicationFactory<TProgram> where TProgram : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Add in-memory DB
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to seed the DB.
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.EnsureCreated();

                    if (!db.Companies.Any())
                    {
                        var companies = Enumerable.Range(1, 100)
                            .Select(i => new Company(
                                $"Test Company {i}",
                                $"TST{i}",
                                "Test Exchange",
                                $"US{i:D10}" // 2 letters + 10 digits = 12 chars
                            ))
                            .ToList();
                        db.Companies.AddRange(companies);
                        db.SaveChanges();
                    }
                }
            });
            // You can add additional configuration here if needed
        }
    }
}
