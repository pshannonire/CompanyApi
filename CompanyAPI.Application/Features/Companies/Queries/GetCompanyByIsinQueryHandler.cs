using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetCompanyByIsinQueryHandler : IRequestHandler<GetCompanyByIsinQuery, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<GetCompanyByIdQueryHandler> _logger;

        public GetCompanyByIsinQueryHandler(
            ICompanyRepository companyRepository,
            ILogger<GetCompanyByIdQueryHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIsinQuery request, CancellationToken cancellationToken)

        {
            try
            {
                var company = await _companyRepository.GetByIsinAsync(request.Isin.Trim(), cancellationToken);

                if (company == null)
                {
                    _logger.LogDebug("Company with ISIN {ISIN} not found", request.Isin);
                    return Result.Failure<CompanyDto>($"Company with ISIN {request.Isin} not found");
                }

                return Result.Success(new CompanyDto(company.Id, company.Name, company.Ticker, company.Exchange, company.Isin, company.Website, company.CreatedAt, company.UpdatedAt));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company by ISIN {ISIN}", request.Isin);
                return Result.Failure<CompanyDto>("An error occurred while retrieving the company");
            }
        }
    }
}
