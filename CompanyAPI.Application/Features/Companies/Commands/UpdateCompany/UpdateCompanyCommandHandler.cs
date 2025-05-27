using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ILogger<UpdateCompanyCommandHandler> _logger;
        private readonly ICompanyRepository _companyRepository;
        public UpdateCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<UpdateCompanyCommandHandler> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }
        public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var existingCompany = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);

                if (existingCompany == null)
                {
                    _logger.LogWarning("Attempt to update non-existing company with ID: {CompanyId}", request.Id);
                    return Result.Failure<CompanyDto>($"Company with ID {request.Id} not found");
                }

                // Validate ISIN uniqueness if it is being changed
                if (existingCompany.Isin != request.Isin &&
                    await _companyRepository.ExistsByIsinAsync(request.Isin, cancellationToken))
                {
                    _logger.LogWarning("Attempt to update company with duplicate ISIN: {Isin}", request.Isin);
                    return Result.Failure<CompanyDto>($"A company with ISIN {request.Isin} already exists");
                }



                var company = new Company(
                       request.Name,
                       request.Ticker,
                       request.Exchange,
                       request.Isin,
                       request.Website);
                company.SetUpdatedDate();

                await _companyRepository.Update(company, cancellationToken);

                _logger.LogInformation("Successfully updated company with ID {CompanyId} and ISIN {Isin}",
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
