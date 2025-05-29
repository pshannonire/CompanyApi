using CompanyAPI.Application.Common.Models;
using CompanyAPI.Application.Features.Authentication.DTOs;
using CompanyAPI.Application.Features.Companies.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CompanyAPI.IntegrationTests
{
    public class CompaniesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CompaniesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            var token = GetJwtTokenAsync(_client).GetAwaiter().GetResult();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetJwtTokenAsync(HttpClient client)
        {
            var login = new { Username = "admin", Password = "password123" }; 
            var response = await client.PostAsJsonAsync("/api/auth/login", login);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result?.Token!;
        }

        [Fact]
        public async Task GetAllCompanies_Returns100Companies()
        {
            //Act
            var response = await _client.GetAsync("/api/companies?pageNumber=1&pageSize=100");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paginated = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            paginated.Should().NotBeNull();
            paginated!.Items.Should().HaveCount(100);

            paginated.Items.Select(c => c.Isin).Should().OnlyHaveUniqueItems();
            paginated.Items.Should().Contain(c => c.Name == "Test Company 1" && c.Ticker == "TST1" && c.Isin == "US0000000001");
            paginated.Items.Should().Contain(c => c.Name == "Test Company 50" && c.Ticker == "TST50" && c.Isin == "US0000000050");
            paginated.Items.Should().Contain(c => c.Name == "Test Company 100" && c.Ticker == "TST100" && c.Isin == "US0000000100");
        }

        [Theory]
        [InlineData(1, "Test Company 1", "TST1", "Test Exchange", "US0000000001")]
        [InlineData(50, "Test Company 50", "TST50", "Test Exchange", "US0000000050")]
        [InlineData(100, "Test Company 100", "TST100", "Test Exchange", "US0000000100")]
        public async Task GetCompanyById_ReturnsCorrectCompany(int id, string expectedName, string expectedTicker, string expectedExchange, string expectedIsin)
        {
            // Act
            var response = await _client.GetAsync($"/api/companies/{id}");
            //Assert   
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
            company.Should().NotBeNull();
            company!.Id.Should().Be(id);
            company.Name.Should().Be(expectedName);
            company.Ticker.Should().Be(expectedTicker);
            company.Exchange.Should().Be(expectedExchange);
            company.Isin.Should().Be(expectedIsin);
        }

        [Theory]
        [InlineData("US0000000001", "Test Company 1", "TST1")]
        [InlineData("US0000000050", "Test Company 50", "TST50")]
        [InlineData("US0000000100", "Test Company 100", "TST100")]
        public async Task GetCompanyByIsin_ReturnsCorrectCompany(string isin, string expectedName, string expectedTicker)
        {
            //Act
            var response = await _client.GetAsync($"/api/companies/isin/{isin}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
            company.Should().NotBeNull();
            company!.Isin.Should().Be(isin);
            company.Name.Should().Be(expectedName);
            company.Ticker.Should().Be(expectedTicker);
        }

        [Fact]
        public async Task GetCompanyById_WithNonExistentId_ReturnsNotFound()
        {
            //arrange
            int nonExistentId = 99999;
            //act
            var response = await _client.GetAsync($"/api/companies/{nonExistentId}");
            //assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCompanyByIsin_WithNonExistentIsin_ReturnsNotFound()
        {
            //act
            var response = await _client.GetAsync("/api/companies/isin/US9999999999");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateCompany_AddsNewCompanyToList()
        {
            //arrange
            var createDto = new CreateCompanyDto
            {
                Name = "Integration Co",
                Ticker = "INTEGRATE",
                Exchange = "Test Exchange",
                Isin = "US9999999999",
                Website = "https://integration.example.com"
            };
            //act
            var response = await _client.PostAsJsonAsync("/api/companies", createDto);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<CompanyDto>();
            created.Should().NotBeNull();
            created!.Name.Should().Be("Integration Co");
            created.Ticker.Should().Be("INTEGRATE");
            created.Exchange.Should().Be("Test Exchange");
            created.Isin.Should().Be("US9999999999");
            created.Website.Should().Be("https://integration.example.com");
        }

        [Fact]
        public async Task UpdateCompany_ChangesCompanyDetails()
        {
            // Create a company first
            var createDto = new CreateCompanyDto
            {
                Name = "Original Name",
                Ticker = "ORIG",
                Exchange = "Test Exchange",
                Isin = "US8888888888",
                Website = "https://original.example.com"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/companies", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResponse.Content.ReadFromJsonAsync<CompanyDto>();
            created.Should().NotBeNull();

            // Prepare update DTO
            var updateDto = new CompanyDto
            {
                Id = created!.Id,
                Name = "Updated Name",
                Ticker = "UPDT",
                Exchange = "Test Exchange",
                Isin = "US8888888888", 
                Website = "https://updated.example.com"
            };

            // Perform update
            var updateResponse = await _client.PutAsJsonAsync($"/api/companies/{created.Id}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Fetch again to verify
            var getResponse = await _client.GetAsync($"/api/companies/{created.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated = await getResponse.Content.ReadFromJsonAsync<CompanyDto>();
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Name");
            updated.Ticker.Should().Be("UPDT");
            updated.Isin.Should().Be("US8888888888");
            updated.Website.Should().Be("https://updated.example.com");
        }

        [Fact]
        public async Task Paging_FirstPage_ReturnsFirst10Companies()
        {
            //arrange
            int pageSize = 10, pageNumber = 1;

            //act
            var response = await _client.GetAsync($"/api/companies?pageNumber={pageNumber}&pageSize={pageSize}");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.PageNumber.Should().Be(pageNumber);
            page.PageSize.Should().Be(pageSize);
            page.TotalCount.Should().BeGreaterThanOrEqualTo(100);
            page.TotalPages.Should().BeGreaterThanOrEqualTo(10);
            page.HasPreviousPage.Should().BeFalse();
            page.HasNextPage.Should().BeTrue();
            page.Items.First().Name.Should().Be("Test Company 1");
            page.Items.Last().Name.Should().Be("Test Company 10");
        }

        [Fact]
        public async Task Paging_MiddlePage_ReturnsCorrectCompanies()
        {
            //aarange
            int pageSize = 10, pageNumber = 5;

            //act
            var response = await _client.GetAsync($"/api/companies?pageNumber={pageNumber}&pageSize={pageSize}");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.PageNumber.Should().Be(pageNumber);
            page.HasPreviousPage.Should().BeTrue();
            page.HasNextPage.Should().BeTrue();
            page.Items.First().Name.Should().Be("Test Company 41");
            page.Items.Last().Name.Should().Be("Test Company 50");
        }

        [Fact]
        public async Task Paging_LastPage_ReturnsLast10Companies()
        {
            // Arrange
            int pageSize = 10, pageNumber = 10;

            // Act
            var response = await _client.GetAsync($"/api/companies?pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.PageNumber.Should().Be(pageNumber);
            page.HasPreviousPage.Should().BeTrue();
            page.HasNextPage.Should().BeFalse();
            page.Items.First().Name.Should().Be("Test Company 91");
            page.Items.Last().Name.Should().Be("Test Company 100");
        }

        [Fact]
        public async Task Paging_SingleItemPage_ReturnsCorrectCompany()
        {
            // Arrange
            int pageSize = 1, pageNumber = 42;
            //Act
            var response = await _client.GetAsync($"/api/companies?pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.Items.Should().HaveCount(1);
            page.Items[0].Name.Should().Be("Test Company 42");
            page.Items[0].Isin.Should().Be("US0000000042");
        }

        [Fact]
        public async Task Paging_OversizedPage_ReturnsAllCompanies()
        {
            // Arrange
            int pageSize = 100, pageNumber = 1;
            // Act
            var response = await _client.GetAsync($"/api/companies?pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.PageNumber.Should().Be(pageNumber);
            page.Items.Count.Should().BeGreaterThanOrEqualTo(100); // All seeded companies (plus any created during test)
            page.HasPreviousPage.Should().BeFalse();
            page.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task Paging_OutOfBoundsPage_ReturnsEmpty()
        {
            // Arrange
            int pageSize = 10, pageNumber = 999;
            // Act
            var response = await _client.GetAsync($"/api/companies?pageNumber={pageNumber}&pageSize={pageSize}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.Items.Should().BeEmpty();
            page.PageNumber.Should().Be(pageNumber);
        }

        [Theory]
        [InlineData(0, 10, "name", null, "Page must be greater than 0")]
        [InlineData(1, 0, "name", null, "Page size must be greater than 0")]
        [InlineData(1, 101, "name", null, "Page size cannot exceed 100")]
        [InlineData(1, 10, "notarealfield", null, "Sort field must be one of: name, ticker, exchange, isin")]
        public async Task GetCompanies_InvalidPagingOrSort_ReturnsBadRequest(
            int page, int pageSize, string sortBy, string? searchTerm, string expectedError)
        {
            // Arrange
            var url = $"/api/companies?pageNumber={page}&pageSize={pageSize}&sortBy={sortBy}";
            if (searchTerm != null)
                url += $"&searchTerm={searchTerm}";

            // Act
            var response = await _client.GetAsync(url);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(expectedError);
        }

        [Theory]
        [InlineData("name")]
        [InlineData("ticker")]
        [InlineData("exchange")]
        [InlineData("isin")]
        [InlineData(null)]
        public async Task GetCompanies_ValidSortBy_ShouldReturnOk(string? sortBy)
        {
            // Arrange
            var url = $"/api/companies?pageNumber=1&pageSize=10";
            if (!string.IsNullOrEmpty(sortBy))
                url += $"&sortBy={sortBy}";

            // Act
            var response = await _client.GetAsync(url);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetCompanies_ValidSearchTerm_ShouldReturnOk()
        {
            // Arrange
            string searchTerm = "Test";
            var url = $"/api/companies?pageNumber=1&pageSize=10&searchTerm={searchTerm}";

            // Act
            var response = await _client.GetAsync(url);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var page = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyDto>>();
            page.Should().NotBeNull();
            page!.Items.Should().NotBeEmpty();
        }
    }

}