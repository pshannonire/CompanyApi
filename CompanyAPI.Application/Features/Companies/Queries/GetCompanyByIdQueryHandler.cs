using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Application.Features.Companies.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<GetCompanyByIdQueryHandler> _logger;

        public GetCompanyByIdQueryHandler(
            ICompanyRepository companyRepository,
            ILogger<GetCompanyByIdQueryHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (company == null)
            {
                _logger.LogDebug("Company with ID {CompanyId} not found", request.Id);
                return Result.Failure<CompanyDto>($"Company with ID {request.Id} not found");
            }

            return Result.Success(company.ToDto());
        }
    }
}
