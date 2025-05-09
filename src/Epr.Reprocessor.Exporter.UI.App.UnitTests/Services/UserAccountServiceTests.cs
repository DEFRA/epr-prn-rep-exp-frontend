using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
using Epr.Reprocessor.Exporter.UI.App.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.App.UnitTests.Services
{
    [TestClass]
    public class UserAccountServiceTests
    {
        private Mock<IAccountServiceApiClient> _userAccountServiceApiClientMock;
        private UserAccountService _sut;

        [TestInitialize]
        public void Init()
        {
            _userAccountServiceApiClientMock = new Mock<IAccountServiceApiClient>();
            _sut = new UserAccountService(_userAccountServiceApiClientMock.Object, new NullLogger<UserAccountService>());
        }

        private static HttpContent ToJsonContent<T>(T obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        [TestMethod]
        public async Task GetUserAccount_ReturnsAccount()
        {
            // Arrange
            var userAcc = new UserAccountDto
            {
                User = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "Email"
                }
            };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = ToJsonContent(userAcc)
            };
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var res = await _sut.GetUserAccount();

            // Assert
            res.Should().BeOfType<UserAccountDto>();
            res.User.Email.Should().Be("Email");
        }

        [TestMethod]
        public async Task GetUserAccount_WhenClientThrowsException_ThrowsException()
        {
            // Arrange
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Client error"));

            // Act & Assert
            Func<Task> act = async () => await _sut.GetUserAccount();
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task GetUserAccount_WhenUserNotFound_ReturnsNull()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.GetUserAccount();

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetPersonByUserId_ReturnsAccount()
        {
            // Arrange
            var person = new PersonDto
            {
                FirstName = "Test"
            };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = ToJsonContent(person)
            };
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var res = await _sut.GetPersonByUserId(Guid.NewGuid());

            // Assert
            res.Should().BeOfType<PersonDto>();
            res.FirstName.Should().Be("Test");
        }

        [TestMethod]
        public async Task GetPersonByUserId_WhenClientThrowsException_ThrowsException()
        {
            // Arrange
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            // Act & Assert
            Func<Task> act = async () => await _sut.GetPersonByUserId(Guid.NewGuid());
            await act.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task GetAllPersonByUserId_ReturnsAccount()
        {
            // Arrange
            var person = new PersonDto
            {
                FirstName = "Test"
            };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = ToJsonContent(person)
            };
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var res = await _sut.GetAllPersonByUserId(Guid.NewGuid());

            // Assert
            res.Should().BeOfType<PersonDto>();
            res.FirstName.Should().Be("Test");
        }

        [TestMethod]
        public async Task GetAllPersonByUserId_WhenClientThrowsException_ThrowsException()
        {
            // Arrange
            _userAccountServiceApiClientMock.Setup(x => x.SendGetRequest(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            // Act & Assert
            Func<Task> act = async () => await _sut.GetAllPersonByUserId(Guid.NewGuid());
            await act.Should().ThrowAsync<Exception>();
        }
    }
}
