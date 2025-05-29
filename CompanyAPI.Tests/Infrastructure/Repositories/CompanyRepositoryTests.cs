using CompanyAPI.Domain.Entities;
using CompanyAPI.Infrastructure.Data;
using CompanyAPI.Infrastructure.Repositories.Companies;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Tests.Infrastructure.Repositories
{
    public class CompanyRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CompanyRepository _repository;
        private readonly Mock<IMediator> _mediatorMock;

        public CompanyRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mediatorMock = new Mock<IMediator>();
            _context = new ApplicationDbContext(options, _mediatorMock.Object);
            _repository = new CompanyRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ValidCompany_AddsToDatabase()
        {
            // Arrange
            var company = new Company("Test Company", "TEST", "NYSE", "US1234567890");

            // Act
            await _repository.AddAsync(company);
            await _context.SaveChangesAsync();

            // Assert
            var savedCompany = await _context.Companies.FirstOrDefaultAsync();
            savedCompany.Should().NotBeNull();
            savedCompany.Name.Should().Be("Test Company");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCompany_ReturnsCompany()
        {
            // Arrange
            var company = new Company("Test Company", "TEST", "NYSE", "US1234567890");
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(company.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(company.Id);
            result.Name.Should().Be("Test Company");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentCompany_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIsinAsync_ExistingCompany_ReturnsCompany()
        {
            // Arrange
            var company = new Company("Test Company", "TEST", "NYSE", "US1234567890");
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIsinAsync("US1234567890");

            // Assert
            result.Should().NotBeNull();
            result.Isin.Should().Be("US1234567890");
        }

        [Fact]
        public async Task ExistsByIsinAsync_ExistingIsin_ReturnsTrue()
        {
            // Arrange
            var company = new Company("Test Company", "TEST", "NYSE", "US1234567890");
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsByIsinAsync("US1234567890");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByIsinAsync_NonExistentIsin_ReturnsFalse()
        {
            // Act
            var result = await _repository.ExistsByIsinAsync("US9999999999");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                var company = new Company($"Company {i}", $"T{i}", "NYSE", $"US{i:D10}");
                _context.Companies.Add(company);
            }
            await _context.SaveChangesAsync();

            // Act
            var (items, totalCount) = await _repository.GetAllAsync(2, 5, null, false, null);

            // Assert
            items.Should().HaveCount(5);
            totalCount.Should().Be(15);
            items.First().Name.Should().Be("Company 6"); // Second page starts with 6th item
        }

        [Fact]
        public async Task GetAllAsync_WithSearchTerm_FiltersResults()
        {
            // Arrange
            var companies = new[]
            {
                new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005"),
                new Company("Microsoft Corp.", "MSFT", "NASDAQ", "US5949181045"),
                new Company("Apple Hospitality", "APLE", "NYSE", "US0378331006")
            };
            _context.Companies.AddRange(companies);
            await _context.SaveChangesAsync();

            // Act
            var (items, totalCount) = await _repository.GetAllAsync(1, 10, null, false, "Apple");

            // Assert
            items.Should().HaveCount(2);
            totalCount.Should().Be(2);
            items.All(c => c.Name.Contains("Apple")).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_WithSorting_ReturnsSortedResults()
        {
            // Arrange
            var companies = new[]
            {
                new Company("Zebra Tech", "ZBRA", "NASDAQ", "US9892071054"),
                new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005"),
                new Company("Microsoft Corp.", "MSFT", "NASDAQ", "US5949181045")
            };
            _context.Companies.AddRange(companies);
            await _context.SaveChangesAsync();

            // Act
            var (items, _) = await _repository.GetAllAsync(1, 10, "name", false, null);

            // Assert
            items.First().Name.Should().Be("Apple Inc.");
            items.Last().Name.Should().Be("Zebra Tech");
        }

        [Fact]
        public async Task GetAllAsync_WithSortingDescending_ReturnsSortedResults()
        {
            // Arrange
            var companies = new[]
            {
                new Company("Apple Inc.", "AAPL", "NASDAQ", "US0378331005"),
                new Company("Zebra Tech", "ZBRA", "NASDAQ", "US9892071054"),
                new Company("Microsoft Corp.", "MSFT", "NASDAQ", "US5949181045")
            };
            _context.Companies.AddRange(companies);
            await _context.SaveChangesAsync();

            // Act
            var (items, _) = await _repository.GetAllAsync(1, 10, "name", true, null);

            // Assert
            items.First().Name.Should().Be("Zebra Tech");
            items.Last().Name.Should().Be("Apple Inc.");
        }

        [Fact]
        public async Task Update_ExistingCompany_UpdatesInDatabase()
        {
            // Arrange
            var company = new Company("Old Name", "OLD", "NYSE", "US1234567890");
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Act
            company.Update("New Name", "NEW", "NASDAQ", "US1234567890", "http://new.com");
            await _repository.Update(company);
            await _context.SaveChangesAsync();

            // Assert
            var updatedCompany = await _context.Companies.FirstAsync();
            updatedCompany.Name.Should().Be("New Name");
            updatedCompany.Ticker.Should().Be("NEW");
            updatedCompany.Exchange.Should().Be("NASDAQ");
            updatedCompany.Website.Should().Be("http://new.com");
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
