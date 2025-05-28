using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application.Features.Companies.Queries.Validators
{
    public class GetCompanyByIsinQueryValidator : AbstractValidator<GetCompanyByIsinQuery>
    {
        public GetCompanyByIsinQueryValidator()
        {
            RuleFor(x => x.Isin)
                .NotEmpty()
                .WithMessage("ISIN is required")
                .Length(12)
                .WithMessage("ISIN must be exactly 12 characters")
                .Matches(@"^[A-Za-z]{2}[A-Za-z0-9]{10}$")
                .WithMessage("ISIN must start with 2 letters followed by 10 alphanumeric characters");
        }
    }
}
