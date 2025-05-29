using CompanyAPI.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Tests.Domain.Entities
{
    public class CompanyTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesCompany()
        {
            // Arrange
            var name = "Apple Inc.";
            var ticker = "AAPL";
            var exchange = "NASDAQ";
            var isin = "US0378331005";
            var website = "http://www.apple.com";

            // Act
            var company = new Company(name, ticker, exchange, isin, website);

            // Assert
            company.Name.Should().Be(name);
            company.Ticker.Should().Be("AAPL"); // Should be uppercase
            company.Exchange.Should().Be(exchange);
            company.Isin.Should().Be("US0378331005"); // Should be uppercase
            company.Website.Should().Be(website);
            company.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            company.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange
            var name = "  Apple Inc.  ";
            var ticker = "  aapl  ";
            var exchange = "  NASDAQ  ";
            var isin = "  us0378331005  ";

            // Act
            var company = new Company(name, ticker, exchange, isin);

            // Assert
            company.Name.Should().Be("Apple Inc.");
            company.Ticker.Should().Be("AAPL");
            company.Exchange.Should().Be("NASDAQ");
            company.Isin.Should().Be("US0378331005");
        }

        [Fact]
        public void Constructor_NullWebsite_SetsNullWebsite()
        {
            // Act
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005", null);

            // Assert
            company.Website.Should().BeNull();
        }

        [Fact]
        public void Update_ValidParameters_UpdatesCompany()
        {
            // Arrange
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005");
            var originalCreatedAt = company.CreatedAt;
            var originalUpdatedAt = company.UpdatedAt;

            // Wait a bit to ensure UpdatedAt changes
            System.Threading.Thread.Sleep(10);

            // Act
            company.Update("Apple Corporation", "APPL", "NYSE", "US0378331006", "http://www.apple.com");

            // Assert
            company.Name.Should().Be("Apple Corporation");
            company.Ticker.Should().Be("APPL");
            company.Exchange.Should().Be("NYSE");
            company.Isin.Should().Be("US0378331006");
            company.Website.Should().Be("http://www.apple.com");
            company.CreatedAt.Should().Be(originalCreatedAt);
            company.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

        [Fact]
        public void Update_TrimsAndUppercasesValues()
        {
            // Arrange
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005");

            // Act
            company.Update("  Apple Corporation  ", "  appl  ", "  NYSE  ", "  us0378331006  ", "  http://www.apple.com  ");

            // Assert
            company.Name.Should().Be("Apple Corporation");
            company.Ticker.Should().Be("APPL");
            company.Exchange.Should().Be("NYSE");
            company.Isin.Should().Be("US0378331006");
            company.Website.Should().Be("http://www.apple.com");
        }

        [Fact]
        public void BaseEntity_Id_DefaultsToZero()
        {
            // Act
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005");

            // Assert
            company.Id.Should().Be(0);
        }
    }
}
