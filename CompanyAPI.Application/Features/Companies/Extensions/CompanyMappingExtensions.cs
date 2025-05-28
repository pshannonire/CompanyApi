using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Domain.Entities;
namespace CompanyAPI.Application.Features.Companies.Extensions
{
    public static class CompanyMappingExtensions
    {
        public static CompanyDto ToDto(this Company company)
        {
            return new CompanyDto(
                company.Id,
                company.Name,
                company.Ticker,
                company.Exchange,
                company.Isin,
                company.Website,
                company.CreatedAt,
                company.UpdatedAt);
        }

        public static List<CompanyDto> ToDto(this IEnumerable<Company> companies)
        {
            return companies.Select(c => c.ToDto()).ToList();
        }
    }
}
