using CompanyAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                logger.LogInformation("Initializing in-memory database...");

                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("In-memory database created successfully");

                await SeedDataAsync(context, logger);

                logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private static async Task SeedDataAsync(ApplicationDbContext context, ILogger logger)
        {
           
            if (await context.Companies.AnyAsync())
            {
                logger.LogInformation("Database already contains data, skipping seed");
                return;
            }

            logger.LogInformation("Seeding database with sample companies...");

            var companies = new List<Company>
            {
                new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005", "http://www.apple.com"),
                new Company("British Airways Plc", "BAIRY", "Pink Sheets", "US1104193065"),
                new Company("Heineken NV", "HEIA", "Euronext Amsterdam", "NL0000009165"),
                new Company("Panasonic Corp", "6752", "Tokyo Stock Exchange", "JP3866800000", "http://www.panasonic.co.jp"),
                new Company("Porsche Automobil", "PAH3", "Deutsche Börse", "DE000PAH0038", "https://www.porsche.com/")
            };


            await context.Companies.AddRangeAsync(companies);
            await context.SaveChangesAsync();


        }
    }
}
