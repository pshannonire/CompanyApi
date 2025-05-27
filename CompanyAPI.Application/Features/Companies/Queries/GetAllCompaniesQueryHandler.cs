using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, Result<PaginatedList<CompanyDto>>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<GetAllCompaniesQueryHandler> _logger;

        public GetAllCompaniesQueryHandler(
            ICompanyRepository companyRepository,
            ILogger<GetAllCompaniesQueryHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<CompanyDto>>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (companies, totalCount) = await _companyRepository.GetAllAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                var companyDtos = companies.Select(c => new CompanyDto(
                    c.Id,
                    c.Name,
                    c.Ticker,
                    c.Exchange,
                    c.Isin,
                    c.Website,
                    c.CreatedAt,
                    c.UpdatedAt)).ToList();

                var paginatedList = new PaginatedList<CompanyDto>(companyDtos, totalCount, request.Page, request.PageSize);

                _logger.LogInformation("Retrieved {CompanyCount} companies (page {Page} of {TotalPages})",
                    companyDtos.Count, request.Page, paginatedList.TotalPages);

                return Result.Success(paginatedList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companies");
                return Result.Failure<PaginatedList<CompanyDto>>("An error occurred while retrieving companies");
            }
        }

    }
}
