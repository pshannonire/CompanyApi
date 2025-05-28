using FluentValidation;
using System;

namespace CompanyAPI.Application.Features.Companies.Queries.Validators
{
    public class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
    {
        public GetCompanyByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Company ID must be greater than 0");
        }
    }
}
