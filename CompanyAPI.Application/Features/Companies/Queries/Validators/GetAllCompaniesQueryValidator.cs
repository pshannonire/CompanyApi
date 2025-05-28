using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application.Features.Companies.Queries.Validators
{
    public class GetAllCompaniesQueryValidator : AbstractValidator<GetAllCompaniesQuery>
    {
        private static readonly string[] ValidSortFields = { "name", "ticker", "exchange", "isin" };

        public GetAllCompaniesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size cannot exceed 100");

            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage($"Sort field must be one of: {string.Join(", ", ValidSortFields)}");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters");
        }

        private static bool BeValidSortField(string? sortBy)
        {
            return string.IsNullOrWhiteSpace(sortBy) ||
                   ValidSortFields.Contains(sortBy.ToLower());
        }
    }
}
