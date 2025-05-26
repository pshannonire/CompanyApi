using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetCompanyByIdQuery : IRequest<Result<CompanyDto>>
    {
        public int Id { get; set; }

        public GetCompanyByIdQuery(int id)
        {
            Id = id;
        }
    }
}
