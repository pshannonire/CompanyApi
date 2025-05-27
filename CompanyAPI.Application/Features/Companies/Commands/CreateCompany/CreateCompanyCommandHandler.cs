using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ILogger<CreateCompanyCommandHandler> _logger;
        private readonly ICompanyRepository _companyRepository;
        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<CreateCompanyCommandHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }
        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {

            try
            {

                if (await _companyRepository.ExistsByIsinAsync(request.Isin, cancellationToken))
                {
                    _logger.LogWarning("Attempt to create company with duplicate ISIN: {Isin}", request.Isin);
                    return Result.Failure<CompanyDto>($"A company with ISIN {request.Isin} already exists");
                }

                var company = new Company(
                       request.Name,
                       request.Ticker,
                       request.Exchange,
                       request.Isin,
                       request.Website);

                var saveResult = _companyRepository.AddAsync(company, cancellationToken);

                _logger.LogInformation("Successfully created company with ID {CompanyId} and ISIN {Isin}",
                 company.Id, company.Isin);

                return Result.Success(new CompanyDto(company.Id, company.Name, company.Ticker, company.Exchange, company.Isin, company.Website, company.CreatedAt, company.UpdatedAt));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company with ISIN {Isin}", request.Isin);
                return Result.Failure<CompanyDto>("An error occurred while creating the company");
            }
        }
    }
}
