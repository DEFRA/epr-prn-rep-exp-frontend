namespace Epr.Reprocessor.Exporter.UI.UnitTests.Extensions;

[TestClass]
public class DecimalExtensionMethodsTests
{
    [TestMethod]
    [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
    public void ToStringWithOutDecimalPlaces_ReturnsExpectedString(decimal value, int decimalPlaces, string expected)
    {
        var result = value.ToStringWithOutDecimalPlaces(decimalPlaces);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ToStringWithOutDecimalPlaces_NegativeDecimalPlaces_ThrowsArgumentOutOfRangeException()
    {
        decimal value = 1.23m;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => value.ToStringWithOutDecimalPlaces(-1));
    }

    private static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { 123.456m, 0, "123" };
        yield return new object[] { 123.456m, 1, "123.5" };
        yield return new object[] { 123.456m, 2, "123.46" };
        yield return new object[] { 123.456m, 3, "123.456" };
        yield return new object[] { 0m, 0, "0" };
        yield return new object[] { -123.456m, 2, "-123.46" };
    }
}
