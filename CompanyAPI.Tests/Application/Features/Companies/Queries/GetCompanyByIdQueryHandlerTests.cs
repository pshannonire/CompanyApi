using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Features.Companies.Queries;
using CompanyAPI.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CompanyAPI.Tests.Application.Features.Companies.Queries
{
    public class GetCompanyByIdQueryHandlerTests
    {
        private readonly Mock<ICompanyRepository> _repositoryMock;
        private readonly Mock<ILogger<GetCompanyByIdQueryHandler>> _loggerMock;
        private readonly GetCompanyByIdQueryHandler _handler;

        public GetCompanyByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICompanyRepository>();
            _loggerMock = new Mock<ILogger<GetCompanyByIdQueryHandler>>();
            _handler = new GetCompanyByIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ExistingCompany_ReturnsCompanyDto()
        {
            // Arrange
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005", "http://www.apple.com");
            var query = new GetCompanyByIdQuery(1);

            _repositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Apple Inc.");
            result.Value.Ticker.Should().Be("AAPL");
            result.Value.Exchange.Should().Be("NASDAQ");
            result.Value.Isin.Should().Be("US0378331005");
            result.Value.Website.Should().Be("http://www.apple.com");
        }

        [Fact]
        public async Task Handle_NonExistentCompany_ReturnsFailure()
        {
            // Arrange
            var query = new GetCompanyByIdQuery(999);

            _repositoryMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Company with ID 999 not found");
        }
    }
}

