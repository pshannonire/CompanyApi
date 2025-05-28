using CompanyAPI.Application.Common.Interfaces;
using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Application.Features.Companies.Extensions;
using CompanyAPI.Domain.Entities;
using CompanyAPI.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyAPI.Application.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
        private readonly ILogger<CreateCompanyCommandHandler> _logger;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, ILogger<CreateCompanyCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
                //validate isin in the domain layer
                var isin = new Isin(request.Isin);

                if (await _companyRepository.ExistsByIsinAsync(isin, cancellationToken))
                {
                    _logger.LogWarning("Attempt to create company with duplicate ISIN: {Isin}", isin);
                    return Result.Failure<CompanyDto>($"A company with ISIN {isin} already exists");
                }

                var company = new Company(
                       request.Name,
                       request.Ticker,
                       request.Exchange,
                       isin,
                       request.Website);

               
                await _companyRepository.AddAsync(company, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created company with ID {CompanyId} and ISIN {Isin}",
                 company.Id, company.Isin);

                return Result.Success(company.ToDto());
            
        }
    }
}
