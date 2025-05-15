using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using EPR.Common.Authorization.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services
{
    [TestClass]
    public class AccreditationServiceTests
    {
        private Mock<IUserAccountService> _userAccountServiceMock;
        private AccreditationService _sut;

        [TestInitialize]
        public void Init()
        {
            _userAccountServiceMock = new Mock<IUserAccountService>();
            _sut = new AccreditationService(_userAccountServiceMock.Object);
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ByOrganisationAndRole_ReturnsUsers()
        {
            // Arrange
            var organisation = new Organisation { Id = Guid.NewGuid(), Name = "Test Org" };
            var serviceRoleId = 1;
            var users = new List<ManageUserDto>
            {
                new ManageUserDto { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com" }
            };

            _userAccountServiceMock
                .Setup(x => x.GetUsersForOrganisationAsync(organisation.Id.ToString(), serviceRoleId))
                .ReturnsAsync(users);

            // Act
            var result = await _sut.GetOrganisationUsers(organisation, serviceRoleId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().ContainSingle(u => u.FirstName == "Alice" && u.Email == "alice@example.com");
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ByUserData_ReturnsUsers()
        {
            // Arrange
            var organisation = new Organisation { Id = Guid.NewGuid(), Name = "Test Org" };
            var userData = new UserData
            {
                Organisations = new List<Organisation> { organisation }
            };
            var users = new List<ManageUserDto>
            {
                new ManageUserDto { FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" }
            };

            // Assume the service uses the first organisation and a default role id (e.g., 1)
            _userAccountServiceMock
                .Setup(x => x.GetUsersForOrganisationAsync(organisation.Id.ToString(), It.IsAny<int>()))
                .ReturnsAsync(users);

            // Act
            var result = await _sut.GetOrganisationUsers(userData);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().ContainSingle(u => u.FirstName == "Bob" && u.Email == "bob@example.com");
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ByOrganisationAndRole_WhenThrows_ThrowsException()
        {
            // Arrange
            var organisation = new Organisation { Id = Guid.NewGuid(), Name = "Test Org" };
            var serviceRoleId = 1;

            _userAccountServiceMock
                .Setup(x => x.GetUsersForOrganisationAsync(organisation.Id.ToString(), serviceRoleId))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(organisation, serviceRoleId);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ByUserData_WhenThrows_ThrowsException()
        {
            // Arrange
            var organisation = new Organisation { Id = Guid.NewGuid(), Name = "Test Org" };
            var userData = new UserData
            {
                Organisations = new List<Organisation> { organisation }
            };

            _userAccountServiceMock
                .Setup(x => x.GetUsersForOrganisationAsync(organisation.Id.ToString(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(userData);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
