using CompanyAPI.Application.Common.Interfaces;
using CompanyAPI.Application.Common.Interfaces.Companies;
using CompanyAPI.Application.Features.Companies.Commands.UpdateCompany;
using CompanyAPI.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Tests.Application.Features.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandlerTests
    {
        private readonly Mock<ICompanyRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateCompanyCommandHandler>> _loggerMock;
        private readonly UpdateCompanyCommandHandler _handler;

        public UpdateCompanyCommandHandlerTests()
        {
            _repositoryMock = new Mock<ICompanyRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateCompanyCommandHandler>>();
            _handler = new UpdateCompanyCommandHandler(_repositoryMock.Object, _loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUpdate_Success()
        {
            // Arrange
            var existingCompany = new Company("Old Name", "OLD", "NYSE", "US1234567890");
            var command = new UpdateCompanyCommand
            {
                Id = 1,
                Name = "New Name",
                Ticker = "NEW",
                Exchange = "NASDAQ",
                Isin = "US1234567890",
                Website = "http://new.com"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCompany);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("New Name");
            result.Value.Ticker.Should().Be("NEW");
            result.Value.Exchange.Should().Be("NASDAQ");
            result.Value.Website.Should().Be("http://new.com");

            _repositoryMock.Verify(x => x.Update(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CompanyNotFound_Failure()
        {
            // Arrange
            var command = new UpdateCompanyCommand { Id = 999, Isin = "AB1234567890" };
            _repositoryMock.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }

        [Fact]
        public async Task Handle_DuplicateIsin_Failure()
        {
            // Arrange
            var existingCompany = new Company("Name", "TICK", "NYSE", "US1111111111");
            var command = new UpdateCompanyCommand
            {
                Id = 1,
                Name = "Name",
                Ticker = "TICK",
                Exchange = "NYSE",
                Isin = "US2222222222" 
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCompany);
            _repositoryMock.Setup(x => x.ExistsByIsinAsync("US2222222222", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("already exists");
        }

        [Fact]
        public async Task Handle_SameIsin_SkipsDuplicateCheck()
        {
            // Arrange
            var existingCompany = new Company("Name", "TICK", "NYSE", "US1234567890");
            var command = new UpdateCompanyCommand
            {
                Id = 1,
                Name = "New Name",
                Ticker = "TICK",
                Exchange = "NYSE",
                Isin = "US1234567890" 
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCompany);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(x => x.ExistsByIsinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
