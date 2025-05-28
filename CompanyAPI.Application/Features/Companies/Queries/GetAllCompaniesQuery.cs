using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;
namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetAllCompaniesQuery : IRequest<Result<PaginatedList<CompanyDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        public string? SearchTerm { get; set; }
    }
}
