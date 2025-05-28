using FluentValidation;

namespace CompanyAPI.Application.Features.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required")
                .GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters");

            RuleFor(x => x.Ticker)
                .NotEmpty().WithMessage("Ticker is required")
                .MaximumLength(10).WithMessage("Ticker must not exceed 10 characters");

            RuleFor(x => x.Exchange)
                .NotEmpty().WithMessage("Exchange is required")
                .MaximumLength(100).WithMessage("Exchange must not exceed 100 characters");

            RuleFor(x => x.Isin)
                .NotEmpty().WithMessage("ISIN is required")
                .Length(12).WithMessage("ISIN must be exactly 12 characters")
                .Matches(@"^[A-Za-z]{2}[A-Za-z0-9]{10}$")
                .WithMessage("ISIN must start with 2 letters followed by 10 alphanumeric characters");

            RuleFor(x => x.Website)
                .MaximumLength(100).WithMessage("Website URL must not exceed 100 characters")
                .Must(BeValidUrl).WithMessage("Website must be a valid URL")
                .When(x => !string.IsNullOrWhiteSpace(x.Website));
        }

        private static bool BeValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }

}
