using CompanyAPI.Application.Features.Companies.Queries;
using CompanyAPI.Application.Features.Companies.Queries.Validators;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Tests.Application.Features.Companies.Queries.Validators
{
    public class QueryValidatorTests
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void GetCompanyByIdQuery_Validation(int id, bool shouldPass)
        {
            // Arrange
            var validator = new GetCompanyByIdQueryValidator();
            var query = new GetCompanyByIdQuery(id);

            // Act
            var result = validator.TestValidate(query);

            // Assert
            if (shouldPass)
                result.ShouldNotHaveAnyValidationErrors();
            else
                result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData("US0378331005", true)]
        [InlineData("INVALID", false)]
        [InlineData("", false)]
        public void GetCompanyByIsinQuery_Validation(string isin, bool shouldPass)
        {
            // Arrange
            var validator = new GetCompanyByIsinQueryValidator();
            var query = new GetCompanyByIsinQuery(isin);

            // Act
            var result = validator.TestValidate(query);

            // Assert
            if (shouldPass)
                result.ShouldNotHaveAnyValidationErrors();
            else
                result.ShouldHaveValidationErrorFor(x => x.Isin);
        }

        [Fact]
        public void GetAllCompaniesQuery_ValidQuery_Passes()
        {
            // Arrange
            var validator = new GetAllCompaniesQueryValidator();
            var query = new GetAllCompaniesQuery
            {
                Page = 1,
                PageSize = 50,
                SortBy = "name",
                SearchTerm = "Apple"
            };

            // Act & Assert
            validator.TestValidate(query).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void GetAllCompaniesQuery_InvalidQuery_Fails()
        {
            // Arrange
            var validator = new GetAllCompaniesQueryValidator();
            var query = new GetAllCompaniesQuery
            {
                Page = 0,                              // Invalid: must be > 0
                PageSize = 101,                        // Invalid: max 100
                SortBy = "invalid",                    // Invalid: not in allowed list
                SearchTerm = new string('A', 101)      // Invalid: > 100 chars
            };

            // Act
            var result = validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Page);
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
            result.ShouldHaveValidationErrorFor(x => x.SortBy);
            result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
        }
    }
}
