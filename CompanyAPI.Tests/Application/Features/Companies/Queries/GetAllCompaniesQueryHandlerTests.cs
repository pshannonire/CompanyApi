using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Features.Companies.Queries;
using CompanyAPI.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Tests.Application.Features.Companies.Queries
{
    public class GetAllCompaniesQueryHandlerTests
    {
        private readonly Mock<ICompanyRepository> _repositoryMock;
        private readonly Mock<ILogger<GetAllCompaniesQueryHandler>> _loggerMock;
        private readonly GetAllCompaniesQueryHandler _handler;

        public GetAllCompaniesQueryHandlerTests()
        {
            _repositoryMock = new Mock<ICompanyRepository>();
            _loggerMock = new Mock<ILogger<GetAllCompaniesQueryHandler>>();
            _handler = new GetAllCompaniesQueryHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_DefaultQuery_ReturnsPaginatedList()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005"),
                new Company("Microsoft Corp.", "MSFT", "NASDAQ", "US5949181045")
            };

            var query = new GetAllCompaniesQuery();

            _repositoryMock.Setup(x => x.GetAllAsync(1, 10, null, false, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((companies, 2));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(2);
            result.Value.PageNumber.Should().Be(1);
            result.Value.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task Handle_WithSearchTerm_PassesSearchTermToRepository()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005")
            };

            var query = new GetAllCompaniesQuery { SearchTerm = "Apple" };

            _repositoryMock.Setup(x => x.GetAllAsync(1, 10, null, false, "Apple", It.IsAny<CancellationToken>()))
                .ReturnsAsync((companies, 1));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            _repositoryMock.Verify(x => x.GetAllAsync(1, 10, null, false, "Apple", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithSorting_PassesSortingToRepository()
        {
            // Arrange
            var companies = new List<Company>();
            var query = new GetAllCompaniesQuery
            {
                SortBy = "name",
                SortDescending = true
            };

            _repositoryMock.Setup(x => x.GetAllAsync(1, 10, "name", true, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((companies, 0));

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAllAsync(1, 10, "name", true, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_LargeDataset_CalculatesPageCountCorrectly()
        {
            // Arrange
            var companies = Enumerable.Range(1, 10)
                .Select(i => new Company($"Company {i}", $"T{i}", "NYSE", $"US{i:D10}"))
                .ToList();

            var query = new GetAllCompaniesQuery { Page = 2, PageSize = 10 };

            _repositoryMock.Setup(x => x.GetAllAsync(2, 10, null, false, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((companies, 25)); 

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.PageNumber.Should().Be(2);
            result.Value.TotalPages.Should().Be(3); 
            result.Value.HasPreviousPage.Should().BeTrue();
            result.Value.HasNextPage.Should().BeTrue();
        }
    }
}
