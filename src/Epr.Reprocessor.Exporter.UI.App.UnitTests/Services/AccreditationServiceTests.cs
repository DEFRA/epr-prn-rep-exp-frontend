﻿using Epr.Reprocessor.Exporter.UI.App.DTOs.Accreditation;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using EPR.Common.Authorization.Models;
using Organisation = EPR.Common.Authorization.Models.Organisation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services
{
    [TestClass]
    public class AccreditationServiceTests
    {
        private Mock<IEprFacadeServiceApiClient> _mockClient;
        private Mock<ILogger<AccreditationService>> _mockLogger;
        private Mock<IUserAccountService> _userAccountServiceMock;
        private AccreditationService _sut;

        [TestInitialize]
        public void Init()
        {
            _mockClient = new Mock<IEprFacadeServiceApiClient>();
            _mockLogger = new Mock<ILogger<AccreditationService>>();
            _userAccountServiceMock = new Mock<IUserAccountService>();
            _sut = new AccreditationService(_mockClient.Object, _userAccountServiceMock.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetOrCreateAccreditation_ShouldReturn_AccreditationId_WhenSucessCodeReturnedFromEprClient()
        {
            // Arrange
            var expectedAccreditationId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var materialId = 2;
            var applicationTypeId = 1;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = ToJsonContent(expectedAccreditationId)
            };
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var accreditation = await _sut.GetOrCreateAccreditation(organisationId, materialId, applicationTypeId);

            // Assert
            accreditation.Should().Be(expectedAccreditationId);
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetOrCreateAccreditation_ShouldThrowException_WhenExceptionThrowByRprClient()
        {
            // Arrange
            var materialId = 2;
            var applicationTypeId = 1;

            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _sut.GetOrCreateAccreditation(Guid.NewGuid(), materialId, applicationTypeId);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditation_ShouldReturnDto_WhenSucessCodeReturnedFromEprClient()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK) 
            {
                Content = ToJsonContent(new AccreditationDto
                {
                    AccreditationYear = 2026,
                })
            };
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var accreditation = await _sut.GetAccreditation(Guid.NewGuid());

            // Assert
            accreditation.Should().NotBeNull();
            accreditation.AccreditationYear.Should().Be(2026);
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditation_ShouldReturnNull_When404ReturnedFromEprClient()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var accreditation = await _sut.GetAccreditation(Guid.NewGuid());

            // Assert
            accreditation.Should().BeNull();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditation_ShouldThrowException_WhenNonSuccessCodeReturnedFromEprClient()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            Func<Task> act = async () => await _sut.GetAccreditation(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditation_ShouldThrowException_WhenExceptionThrowByRprClient()
        {
            // Arrange
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _sut.GetAccreditation(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task UpsertAccreditation_ShouldReturnTask_WhenSucessCodeReturnedFromEprClient()
        {
            // Arrange
            var request = new AccreditationRequestDto();
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<AccreditationRequestDto>()))
                       .ReturnsAsync(response);

            // Act
            await _sut.UpsertAccreditation(request);

            // Assert
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task UpsertAccreditation_ShouldThrowException_WhenNonSuccessCodeReturnedFromEprClient()
        {
            // Arrange
            var request = new AccreditationRequestDto();
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<AccreditationRequestDto>()))
                       .ReturnsAsync(response);

            // Act
            Func<Task> act = async () => await _sut.UpsertAccreditation(request);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task UpsertAccreditation_ShouldThrowException_WhenExceptionThrowByEprClient()
        {
            // Arrange
            var request = new AccreditationRequestDto();
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<AccreditationRequestDto>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _sut.UpsertAccreditation(request);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditationPrnIssueAuths_ShouldReturnDtos_WhenSucessCodeReturnedFromEprClient()
        {
            Guid guid1 = new Guid("5F174C8A-991F-454B-BBB9-4EE727E83794");
            Guid guid2 = new Guid("836D4240-2B67-4BBB-B8BA-EBF83AF05177"); 


            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = ToJsonContent(new List<AccreditationPrnIssueAuthDto>
                {
                    new AccreditationPrnIssueAuthDto { PersonExternalId = guid1 },
                    new AccreditationPrnIssueAuthDto { PersonExternalId = guid2 }
                })
            };
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var dtos = await _sut.GetAccreditationPrnIssueAuths(Guid.NewGuid());

            // Assert
            dtos.Should().NotBeNull();
            dtos.Should().HaveCount(2);
            dtos[0].PersonExternalId.Should().Be(guid1);
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditationPrnIssueAuths_ShouldReturnEmptyList_WhenEmptyListReturnedFromEprClient()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = ToJsonContent(new List<AccreditationPrnIssueAuthDto>())
            };
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var dtos = await _sut.GetAccreditationPrnIssueAuths(Guid.NewGuid());

            // Assert
            dtos.Should().NotBeNull();
            dtos.Should().HaveCount(0);
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditationPrnIssueAuths_ShouldReturnNull_When404ReturnedFromEprClient()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            var dtos = await _sut.GetAccreditationPrnIssueAuths(Guid.NewGuid());

            // Assert
            dtos.Should().BeNull();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditationPrnIssueAuths_ShouldThrowException_WhenNonSuccessCodeReturnedFromEprClient()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ReturnsAsync(response);

            // Act
            Func<Task> act = async () => await _sut.GetAccreditationPrnIssueAuths(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAccreditationPrnIssueAuths_ShouldThrowException_WhenExceptionThrowByRprClient()
        {
            // Arrange
            _mockClient.Setup(c => c.SendGetRequest(It.IsAny<string>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _sut.GetAccreditationPrnIssueAuths(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
            _mockClient.Verify(c => c.SendGetRequest(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task ReplaceAccreditationPrnIssueAuths_ShouldReturnTask_WhenSucessCodeReturnedFromEprClient()
        {
            // Arrange
            var request = new List<AccreditationPrnIssueAuthRequestDto>();
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<List<AccreditationPrnIssueAuthRequestDto>>()))
                       .ReturnsAsync(response);

            // Act
            await _sut.ReplaceAccreditationPrnIssueAuths(Guid.NewGuid(), request);

            // Assert
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task ReplaceAccreditationPrnIssueAuths_ShouldThrowException_WhenNonSuccessCodeReturnedFromEprClient()
        {
            // Arrange
            var request = new List<AccreditationPrnIssueAuthRequestDto>();
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<List<AccreditationPrnIssueAuthRequestDto>>()))
                       .ReturnsAsync(response);

            // Act
            Func<Task> act = async () => await _sut.ReplaceAccreditationPrnIssueAuths(Guid.NewGuid(), request);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
        }

        [TestMethod]
        public async Task ReplaceAccreditationPrnIssueAuths_ShouldThrowException_WhenExceptionThrowByEprClient()
        {
            // Arrange
            var request = new List<AccreditationPrnIssueAuthRequestDto>();
            _mockClient.Setup(c => c.SendPostRequest(It.IsAny<string>(), It.IsAny<List<AccreditationPrnIssueAuthRequestDto>>()))
                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _sut.ReplaceAccreditationPrnIssueAuths(Guid.NewGuid(), request);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            _mockClient.Verify(c => c.SendPostRequest(It.IsAny<string>(), request), Times.Once);
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

        [TestMethod]
        public async Task GetOrganisationUsers_ShouldThrowException_WhenUserDataIsNull()
        {
            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers((UserData)null);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ShouldThrowException_WhenOrganisationIsNull()
        {
            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(new UserData());

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task GetOrganisationUsers_ShouldThrowException_WhenNoOrganisation()
        {
            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(new UserData{ Organisations = [] });

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task OverloadedGetOrganisationUsers_ShouldThrowException_WhenOrganisationIdIsNull()
        {
            // Arrange
            var organisation = new Organisation { Id = null };

            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(organisation, 1);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task OverloadedGetOrganisationUsers_ShouldThrowException_WhenEmptyGuidOrganisationId()
        {
            // Arrange
            var organisation = new Organisation { Id = Guid.Empty };

            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(organisation, 1);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task OverloadedGetOrganisationUsers_ShouldThrowException_WhenserviceRoleIdIsZero()
        {
            // Arrange
            var organisation = new Organisation { Id = Guid.NewGuid() };

            // Act
            Func<Task> act = async () => await _sut.GetOrganisationUsers(organisation, 0);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        [DataRow((int)UkNation.England, ApplicationType.Reprocessor, "Aluminium")]
        [DataRow((int)UkNation.Scotland, ApplicationType.Reprocessor, "Steel")]
        [DataRow((int)UkNation.Wales, ApplicationType.Reprocessor, "Glass")]
        [DataRow((int)UkNation.NorthernIreland, ApplicationType.Reprocessor, "Paper")]
        [DataRow((int)UkNation.England, ApplicationType.Exporter, "Aluminium")]
        [DataRow((int)UkNation.Scotland, ApplicationType.Exporter, "Steel")]
        [DataRow((int)UkNation.Wales, ApplicationType.Exporter, "Plastic")]
        [DataRow((int)UkNation.NorthernIreland, ApplicationType.Exporter, "Wood")]
        public async Task CreateApplicationReferenceNumber_ShouldIncludeExpectedComponents(int nationId, ApplicationType appType, string material)
        {
            // Arrange
            string organisationNumber = "123456";

            // Act
            string referenceNumber = _sut.CreateApplicationReferenceNumber("A", nationId, appType, organisationNumber, material);

            // Assert
            switch (nationId)
            {
                case (int)UkNation.England:
                    referenceNumber.Should().Contain("E");
                    break;
                case (int)UkNation.Scotland:
                    referenceNumber.Should().Contain("S");
                    break;
                case (int)UkNation.Wales:
                    referenceNumber.Should().Contain("W");
                    break;
                case (int)UkNation.NorthernIreland:
                    referenceNumber.Should().Contain("N");
                    break;
            }
            switch (appType)
            {
                case ApplicationType.Reprocessor:
                    referenceNumber.Should().Contain("R");
                    break;
                case ApplicationType.Exporter:
                    referenceNumber.Should().Contain("X");
                    break;
                case ApplicationType.Producer:
                    referenceNumber.Should().Contain("P");
                    break;
                case ApplicationType.ComplianceScheme:
                    referenceNumber.Should().Contain("C");
                    break;
            }
            switch (material.ToLower())
            {
                case "aluminium":
                    referenceNumber.Should().EndWith("AL");
                    break;
                case "glass":
                    referenceNumber.Should().EndWith("GL");
                    break;
                case "steel":
                    referenceNumber.Should().EndWith("ST");
                    break;
                case "paper":
                    referenceNumber.Should().EndWith("PA");
                    break;
                case "plastic":
                    referenceNumber.Should().EndWith("PL");
                    break;
                case "wood":
                    referenceNumber.Should().EndWith("WO");
                    break;
            }
        }

        private static HttpContent ToJsonContent<T>(T obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }
    }
}
