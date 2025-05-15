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
        private Mock<IAccreditationService> _accreditationServiceMock;

        [TestInitialize]
        public void Init()
        {
            _accreditationServiceMock = new Mock<IAccreditationService>();
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

            _accreditationServiceMock
                .Setup(x => x.GetOrganisationUsers(organisation, serviceRoleId))
                .ReturnsAsync(users);

            // Act
            var result = await _accreditationServiceMock.Object.GetOrganisationUsers(organisation, serviceRoleId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().ContainSingle(u => u.FirstName == "Alice" && u.Email == "alice@example.com");
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ByUserData_ReturnsUsers()
        {
            // Arrange
            var userData = new UserData
            {
                Organisations = new List<Organisation>
                {
                    new Organisation { Id = Guid.NewGuid(), Name = "Test Org" }
                }
            };
            var users = new List<ManageUserDto>
            {
                new ManageUserDto { FirstName = "Bob", LastName = "Jones", Email = "bob@example.com" }
            };

            _accreditationServiceMock
                .Setup(x => x.GetOrganisationUsers(userData))
                .ReturnsAsync(users);

            // Act
            var result = await _accreditationServiceMock.Object.GetOrganisationUsers(userData);

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

            _accreditationServiceMock
                .Setup(x => x.GetOrganisationUsers(organisation, serviceRoleId))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            Func<Task> act = async () => await _accreditationServiceMock.Object.GetOrganisationUsers(organisation, serviceRoleId);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ByUserData_WhenThrows_ThrowsException()
        {
            // Arrange
            var userData = new UserData();

            _accreditationServiceMock
                .Setup(x => x.GetOrganisationUsers(userData))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            Func<Task> act = async () => await _accreditationServiceMock.Object.GetOrganisationUsers(userData);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
