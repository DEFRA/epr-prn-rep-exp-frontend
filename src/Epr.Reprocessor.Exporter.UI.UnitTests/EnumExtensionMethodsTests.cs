using TaskStatus = Epr.Reprocessor.Exporter.UI.App.Enums.TaskStatus;

namespace Epr.Reprocessor.Exporter.UI.UnitTests
{
	[TestClass]
	public class EnumExtensionMethodsTests
	{
		[TestMethod]
		public void GetDescription_ReturnsCorrectDescription_WhenDescriptionAttributeExists()
		{
			// Arrange
			var status = TaskStatus.CannotStartYet;
			// Act
			var description = status.GetDescription();
			// Assert
			Assert.AreEqual("CANNOT START YET", description);
		}
		
		[TestMethod]
		public void GetDescription_ReturnsEnumName_WhenDescriptionAttributeDoesNotExist()
		{
			// Arrange
			var status = (TaskStatus)999; // An enum value without a Description attribute
			// Act
			var description = status.GetDescription();
			// Assert
			Assert.AreEqual("999", description);
		}
	}
}