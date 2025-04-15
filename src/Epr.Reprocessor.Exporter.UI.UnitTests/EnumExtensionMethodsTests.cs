using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Extensions;

namespace Epr.Reprocessor.Exporter.UI.UnitTests
{
	[TestClass]
	public class EnumExtensionMethodsTests
	{
		[TestMethod]
		public void GetDescription_ReturnsCorrectDescription_WhenDescriptionAttributeExists()
		{
			// Arrange
			var status = TaskListStatus.CannotStartYet;
			// Act
			var description = status.GetDescription();
			// Assert
			Assert.AreEqual("CANNOT START YET", description);
		}
		
		[TestMethod]
		public void GetDescription_ReturnsEnumName_WhenDescriptionAttributeDoesNotExist()
		{
			// Arrange
			var status = (TaskListStatus)999; // An enum value without a Description attribute
			// Act
			var description = status.GetDescription();
			// Assert
			Assert.AreEqual("999", description);
		}
	}
}