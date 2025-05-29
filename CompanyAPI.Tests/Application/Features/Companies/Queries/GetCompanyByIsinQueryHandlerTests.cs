using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Features.Companies.Queries;
using CompanyAPI.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CompanyAPI.Tests.Application.Features.Companies.Queries
{
    public class GetCompanyByIsinQueryHandlerTests
    {
        private readonly Mock<ICompanyRepository> _repositoryMock;
        private readonly Mock<ILogger<GetCompanyByIsinQueryHandler>> _loggerMock;
        private readonly GetCompanyByIsinQueryHandler _handler;

        public GetCompanyByIsinQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICompanyRepository>();
            _loggerMock = new Mock<ILogger<GetCompanyByIsinQueryHandler>>();
            _handler = new GetCompanyByIsinQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ExistingCompany_ReturnsCompanyDto()
        {
            // Arrange
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005");
            var query = new GetCompanyByIsinQuery("US0378331005");

            _repositoryMock.Setup(x => x.GetByIsinAsync("US0378331005", It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Isin.Should().Be("US0378331005");
        }

        [Fact]
        public async Task Handle_TrimsIsinInput()
        {
            // Arrange
            var company = new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005");
            var query = new GetCompanyByIsinQuery("  US0378331005  ");

            _repositoryMock.Setup(x => x.GetByIsinAsync("US0378331005", It.IsAny<CancellationToken>()))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(x => x.GetByIsinAsync("US0378331005", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentCompany_ReturnsFailure()
        {
            // Arrange
            var query = new GetCompanyByIsinQuery("US9999999999");

            _repositoryMock.Setup(x => x.GetByIsinAsync("US9999999999", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Company with ISIN US9999999999 not found");
        }
    }
}
