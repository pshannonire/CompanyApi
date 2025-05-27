using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;

namespace CompanyAPI.Application.Features.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommand : IRequest<Result<CompanyDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ticker { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string Isin { get; set; } = string.Empty;
        public string? Website { get; set; } = null;
    }
}
