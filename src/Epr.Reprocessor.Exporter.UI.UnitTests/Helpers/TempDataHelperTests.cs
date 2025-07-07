using Epr.Reprocessor.Exporter.UI.Helpers;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Helpers;

[TestClass]
public class TempDataHelperTests
{
    private Mock<ITempDataDictionary> _mockTempData;

    [TestInitialize]
    public void Setup()
    {
        _mockTempData = new Mock<ITempDataDictionary>();
    }

    [TestMethod]
    public void Set_ShouldSerialize_And_Store_Data()
    {
        // Arrange
        var key = "TestKey";
        var value = new NotificationBannerModel { Message = "Success" };
        var expectedJson = System.Text.Json.JsonSerializer.Serialize(value);

        var tempData = new Dictionary<string, object>();
        _mockTempData.SetupSet(t => t[key] = It.IsAny<object>())
            .Callback<string, object>((k, v) => tempData[k] = v);

        // Act
        TempDataHelper.Set(_mockTempData.Object, key, value);

        // Assert
        tempData.ContainsKey(key).Should().BeTrue();
        expectedJson.Should().Be(tempData[key] as string);
    }

    [TestMethod]
    public void Get_Should_Return_DeSerialized_Data_When_Exists()
    {
        // Arrange
        var key = "TestKey";
        var value = new NotificationBannerModel { Message = "Success" };
        var expectedJson = System.Text.Json.JsonSerializer.Serialize(value);

        var tempData = new Dictionary<string, object>();
        _mockTempData.Setup(t => t.TryGetValue(key, out It.Ref<object>.IsAny))
            .Returns((string k, out object value) =>
            {
                value = expectedJson;
                return true;
            });

        // Act
        var result = TempDataHelper.Get<NotificationBannerModel>(_mockTempData.Object, key);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Be(value.Message);
    }

    [TestMethod]
    public void Get_Should_Return_Default_When_KeyNotFound()
    {
        // Arrange
        var key = "TestKey";

        var tempData = new Dictionary<string, object>();
        _mockTempData.Setup(t => t.TryGetValue(key, out It.Ref<object>.IsAny))
            .Returns((string k, out object value) =>
            {
                value = null;
                return true;
            });

        // Act
        var result = TempDataHelper.Get<NotificationBannerModel>(_mockTempData.Object, key);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void Get_Should_Return_Default_When_TempData_IsNull()
    {
        // Arrange
        var key = "TestKey";

        // Act
        var result = TempDataHelper.Get<NotificationBannerModel>(null, key);

        // Assert
        result.Should().BeNull();
    }
}
