using CompanyAPI.Application.Features.Companies.Commands.CreateCompany;
using FluentValidation.TestHelper;

namespace CompanyAPI.Tests.Application.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidatorTests
    {
        private readonly CreateCompanyCommandValidator _validator = new();

        [Fact]
        public void ValidCommand_PassesValidation()
        {
            // Arrange
            var command = new CreateCompanyCommand
            {
                Name = "Apple Inc.",
                Ticker = "AAPL",
                Exchange = "NASDAQ",
                Isin = "US0378331005",
                Website = "http://www.apple.com"
            };

            // Act & Assert
            _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void InvalidCommand_FailsValidation()
        {
            // Arrange - Hit all validation rules
            var command = new CreateCompanyCommand
            {
                Name = new string('A', 101),   // > 100 chars
                Ticker = "",                   // Required
                Exchange = "",                 // Required
                Isin = "US123",               // Wrong length
                Website = "ftp://test.com"     // Wrong protocol
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert - All fields should have errors
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Ticker);
            result.ShouldHaveValidationErrorFor(x => x.Exchange);
            result.ShouldHaveValidationErrorFor(x => x.Isin);
            result.ShouldHaveValidationErrorFor(x => x.Website);
        }
    }
}
