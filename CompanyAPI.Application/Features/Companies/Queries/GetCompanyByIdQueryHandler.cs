using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
    {

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)

                   {
            // Simulate fetching company data from a database or external service
            var companyDto = new CompanyDto
            {
                Id = request.Id,
                Name = "Sample Company",
                Ticker = "SMP",
                Exchange = "NYSE",
                Isin = "US1234567890",
                Website = "https://www.samplecompany.com",
                CreatedAt = DateTime.UtcNow.AddYears(-1),
                UpdatedAt = DateTime.UtcNow
            };
            // In a real application, you would check if the company exists and return an error if not found
            if (companyDto.Id <= 0)
            {
                return Result.Failure<CompanyDto>("Company not found");
            }
            return Result.Success(companyDto);
        }
    }
}
