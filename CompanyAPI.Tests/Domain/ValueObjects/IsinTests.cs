using CompanyAPI.Domain.Exceptions.Company;
using CompanyAPI.Domain.ValueObjects;
using FluentAssertions;
namespace CompanyAPI.Tests.Domain.ValueObjects
{
    public class IsinTests
    {
        [Theory]
        [InlineData("US0378331005")] 
        [InlineData("GB0001411924")] 
        [InlineData("NL0000009165")] 
        [InlineData("JP3866800000")] 
        [InlineData("DE000PAH0038")] 
        public void Constructor_ValidIsin_CreatesInstance(string validIsin)
        {
            // Act
            var isin = new Isin(validIsin);

            // Assert
            isin.Value.Should().Be(validIsin.ToUpperInvariant());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Isin_EmptyOrWhitespace_ThrowsException(string invalidIsin)
        {
            var result = () => new Isin(invalidIsin);

            result.Should().Throw<CompanyDomainException>()
                .WithMessage("ISIN cannot be empty");
        }

        [Theory]
        [InlineData("US037833100")]   
        [InlineData("US03783310055")]
        [InlineData("US")]            
        public void Isin_InvalidLength_ThrowsException(string invalidIsin)
        {
            var result = () => new Isin(invalidIsin);

            result.Should().Throw<CompanyDomainException>()
                .WithMessage("ISIN must be exactly 12 characters long");
        }

        [Theory]
        [InlineData("1S0378331005")] 
        [InlineData("U10378331005")] 
        [InlineData("US##78331005")] 
        [InlineData("US 378331005")]
        public void Isin_InvalidFormat_ThrowsException(string invalidIsin)
        {
           
            var result = () => new Isin(invalidIsin);

            result.Should().Throw<CompanyDomainException>()
                .WithMessage("ISIN must start with 2 letters followed by 10 alphanumeric characters");
        }

        [Fact]
        public void Constructor_LowercaseIsin_ConvertsToUppercase()
        {
            // Arrange
            var lowercaseIsin = "us0378331005";

            // Act
            var isin = new Isin(lowercaseIsin);

            // Assert
            isin.Value.Should().Be("US0378331005");
        }
     
    }
}
