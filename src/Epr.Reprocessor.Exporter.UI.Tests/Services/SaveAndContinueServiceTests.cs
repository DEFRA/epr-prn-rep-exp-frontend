using Epr.Reprocessor.Exporter.UI.Services;
using FluentAssertions;

namespace Epr.Reprocessor.Exporter.UI.Tests.Services
{
    [TestClass]
    public class SaveAndContinueServiceTests
    {
        private SaveAndContinueService sut;

        [TestInitialize]
        public void Setup()
        {
            sut = new SaveAndContinueService();
        }

        [TestMethod]
        public async Task AddAsync_ShouldThrowException() {

            var registrationId = 1;
            var action = "Action";
            var controller = "Controller";
            var area = "Area";
            var parameters = "";

            // Act
            Func<Task> act = () => sut.AddAsync(registrationId,action, controller, area, parameters);

            // Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }

    }
}
