using CompanyAPI.Application.Common.Interfaces;
using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Features.Companies.Commands.CreateCompany;
using CompanyAPI.Domain.Entities;
using CompanyAPI.Domain.Exceptions.Company;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CompanyAPI.Tests.Application.Features.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandHandlerTests
    {
        private readonly Mock<ICompanyRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateCompanyCommandHandler>> _loggerMock;
        private readonly CreateCompanyCommandHandler _handler;

        public CreateCompanyCommandHandlerTests()
        {
            _repositoryMock = new Mock<ICompanyRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateCompanyCommandHandler>>();
            _handler = new CreateCompanyCommandHandler(_repositoryMock.Object, _loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesCompanySuccessfully()
        {
            // Arrange
            var command = new CreateCompanyCommand
            {
                Name = "Test Company",
                Ticker = "TEST",
                Exchange = "NASDAQ",
                Isin = "US1234567890",
                Website = "http://test.com"
            };

            _repositoryMock.Setup(x => x.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Test Company");
            result.Value.Ticker.Should().Be("TEST");
            result.Value.Exchange.Should().Be("NASDAQ");
            result.Value.Isin.Should().Be("US1234567890");
            result.Value.Website.Should().Be("http://test.com");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DuplicateIsin_ReturnsFailure()
        {
            // Arrange
            var command = new CreateCompanyCommand
            {
                Name = "Test Company",
                Ticker = "TEST",
                Exchange = "NASDAQ",
                Isin = "US1234567890"
            };

            _repositoryMock.Setup(x => x.ExistsByIsinAsync("US1234567890", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("A company with ISIN US1234567890 already exists");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidIsin_ThrowsException()
        {
            // Arrange
            var command = new CreateCompanyCommand
            {
                Name = "Test Company",
                Ticker = "TEST",
                Exchange = "NASDAQ",
                Isin = "INVALID"
            };

            _repositoryMock.Setup(x => x.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<CompanyDomainException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NoWebsite_CreatesCompanyWithNullWebsite()
        {
            // Arrange
            var command = new CreateCompanyCommand
            {
                Name = "Test Company",
                Ticker = "TEST",
                Exchange = "NASDAQ",
                Isin = "US1234567890",
                Website = null
            };

            _repositoryMock.Setup(x => x.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Website.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ConvertsTickerToUppercase()
        {
            // Arrange
            var command = new CreateCompanyCommand
            {
                Name = "Test Company",
                Ticker = "test",
                Exchange = "NASDAQ",
                Isin = "US1234567890"
            };

            _repositoryMock.Setup(x => x.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            Company savedCompany = null;
            _repositoryMock.Setup(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
                .Callback<Company, CancellationToken>((company, _) => savedCompany = company)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Ticker.Should().Be("TEST");
            savedCompany.Should().NotBeNull();
        }
    }
}
