using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using CompanyAPI.Domain.Entities;
using MediatR;

namespace CompanyAPI.Application.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
    {
         

        private readonly ICompanyRepository _companyRepository;
        public CreateCompanyCommandHandler(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = new Company(
                       request.Name,
                       request.Ticker,
                       request.Exchange,
                       request.Isin,
                       request.Website);


           var saveResult =  _companyRepository.AddAsync(company, cancellationToken);

            return Result.Success(new CompanyDto(company.Id, company.Name, company.Ticker, company.Exchange, company.Isin, company.Website, company.CreatedAt, company.UpdatedAt));
        }
    }
}
