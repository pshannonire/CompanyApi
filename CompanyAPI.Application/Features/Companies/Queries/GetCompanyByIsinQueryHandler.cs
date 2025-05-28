using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Application.Features.Companies.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetCompanyByIsinQueryHandler : IRequestHandler<GetCompanyByIsinQuery, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<GetCompanyByIsinQueryHandler> _logger;

        public GetCompanyByIsinQueryHandler(
            ICompanyRepository companyRepository,
            ILogger<GetCompanyByIsinQueryHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIsinQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIsinAsync(request.Isin.Trim(), cancellationToken);

            if (company == null)
            {
                _logger.LogDebug("Company with ISIN {ISIN} not found", request.Isin);
                return Result.Failure<CompanyDto>($"Company with ISIN {request.Isin} not found");
            }

            return Result.Success(company.ToDto());
        }
    }
}
