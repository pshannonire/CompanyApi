using CompanyAPI.Application.Common.Interfaces;
using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Application.Features.Companies.Extensions;
using CompanyAPI.Domain.Entities;
using CompanyAPI.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ILogger<UpdateCompanyCommandHandler> _logger;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<UpdateCompanyCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {

                var isin = new Isin(request.Isin);
                var existingCompany = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);

                if (existingCompany == null)
                {
                    _logger.LogWarning("Attempt to update non-existing company with ID: {CompanyId}", request.Id);
                    return Result.Failure<CompanyDto>($"Company with ID {request.Id} not found");
                }

                // Validate ISIN uniqueness if it is being changed
                if (existingCompany.Isin != isin &&
                    await _companyRepository.ExistsByIsinAsync(isin, cancellationToken))
                {
                    _logger.LogWarning("Attempt to update company with duplicate ISIN: {Isin}", isin);
                    return Result.Failure<CompanyDto>($"A company with ISIN {isin} already exists");
                }

                existingCompany.Update(request.Name, request.Ticker, request.Exchange, isin, request.Website);


                await _companyRepository.Update(existingCompany, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated company with ID {CompanyId} and ISIN {Isin}",
                 existingCompany.Id, existingCompany.Isin);

                return Result.Success(existingCompany.ToDto());
            
        }
    }
}
