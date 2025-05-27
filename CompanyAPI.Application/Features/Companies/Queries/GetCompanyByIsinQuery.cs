using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Companies.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application.Features.Companies.Queries
{
    public class GetCompanyByIsinQuery : IRequest<Result<CompanyDto>>
    {
        public string Isin { get; set; }

        public GetCompanyByIsinQuery(string isin)
        {
            Isin = isin;
        }
    }
}
