using CompanyAPI.Application.Features.Companies.Commands.UpdateCompany;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Tests.Application.Features.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandValidatorTests
    {
        private readonly UpdateCompanyCommandValidator _validator = new();

        [Fact]
        public void ValidCommand_PassesValidation()
        {
            // Arrange
            var command = new UpdateCompanyCommand
            {
                Id = 1,
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
            // Arrange - Test all validation rules at once
            var command = new UpdateCompanyCommand
            {
                Id = 0,                        // Invalid: must be > 0
                Name = "",                     // Invalid: required
                Ticker = "VERYLONGTICKER",     // Invalid: > 10 chars
                Exchange = new string('X', 101), // Invalid: > 100 chars
                Isin = "INVALID",              // Invalid: wrong format
                Website = "not-a-url"          // Invalid: not valid URL
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Ticker);
            result.ShouldHaveValidationErrorFor(x => x.Exchange);
            result.ShouldHaveValidationErrorFor(x => x.Isin);
            result.ShouldHaveValidationErrorFor(x => x.Website);
        }

        [Theory]
        [InlineData("", "")]                       // Empty website is valid
        [InlineData("http://test.com", "")]        // Valid URL
        [InlineData("invalid-url", "Website must be a valid URL")] // Invalid URL
        public void Website_Validation(string website, string expectedError)
        {
            // Arrange
            var command = new UpdateCompanyCommand
            {
                Id = 1,
                Name = "Test",
                Ticker = "TEST",
                Exchange = "NYSE",
                Isin = "US1234567890",
                Website = website
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            if (string.IsNullOrEmpty(expectedError))
                result.ShouldNotHaveValidationErrorFor(x => x.Website);
            else
                result.ShouldHaveValidationErrorFor(x => x.Website).WithErrorMessage(expectedError);
        }
    }
}
