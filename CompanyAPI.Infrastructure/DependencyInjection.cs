using CompanyAPI.Application.Common.Interfaces;
using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Infrastructure.Data;
using CompanyAPI.Infrastructure.Repositories;
using CompanyAPI.Infrastructure.Repositories.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CompanyAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseInMemoryDatabase("CompanyApiInMemoryDb"));

            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
