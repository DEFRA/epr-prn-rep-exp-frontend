using Microsoft.AspNetCore.Routing;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class ReprocessedMaterialOutputSummaryModelTests
{
    
    [TestMethod]
    public void TotalOutputTonnes_Should_Return_Zero_When_All_Values_Are_Null()
    {
        var model = new ReprocessedMaterialOutputSummaryModel();

        Assert.AreEqual(0, model.TotalOutputTonnes );
    }

    [TestMethod]
    public void TotalOutputTonnes_Should_Sum_SentToOtherSiteTonnes_ContaminantTonnes_ProcessLossTonnes()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "10",
            ContaminantTonnes = "5",
            ProcessLossTonnes = "2"
        };

        Assert.AreEqual(17,model.TotalOutputTonnes);
    }

    [TestMethod]
    public void TotalOutputTonnes_Should_Include_Sum_Of_ReprocessedMaterialsRawData()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "10",
            ContaminantTonnes = "5",
            ProcessLossTonnes = "2",
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>
            {
                new ReprocessedMaterialRawDataModel { MaterialOrProductName = "ProductA", ReprocessedTonnes = "3" },
                new ReprocessedMaterialRawDataModel { MaterialOrProductName = "ProductB", ReprocessedTonnes = "4" }
            }
        };

        Assert.AreEqual(24,model.TotalOutputTonnes); // 10 + 5 + 2 + 3 + 4 = 24
    }

    [TestMethod]
    public void TotalOutputTonnes_Should_Ignore_Entries_With_Empty_ProductName()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "10",
            ContaminantTonnes = "5",
            ProcessLossTonnes = "2",
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>
            {
                new ReprocessedMaterialRawDataModel { MaterialOrProductName = "", ReprocessedTonnes = "3" },
                new ReprocessedMaterialRawDataModel { MaterialOrProductName = "ProductB", ReprocessedTonnes = "4" }
            }
        };
        Assert.AreEqual(21, model.TotalOutputTonnes); // 10 + 5 + 2 + 4 = 21
    }

    [TestMethod]
    public void TotalOutputTonnes_Should_Handle_Null_ReprocessedTonnes_As_Zero()
    {
        var model = new ReprocessedMaterialOutputSummaryModel
        {
            SentToOtherSiteTonnes = "10",
            ContaminantTonnes = "5",
            ProcessLossTonnes = "2",
            ReprocessedMaterialsRawData = new List<ReprocessedMaterialRawDataModel>
            {
                new ReprocessedMaterialRawDataModel { MaterialOrProductName = "ProductA", ReprocessedTonnes = null },
                new ReprocessedMaterialRawDataModel { MaterialOrProductName = "ProductB", ReprocessedTonnes = "4" }
            }
        };

        Assert.AreEqual(21, model.TotalOutputTonnes); // 10 + 5 + 2 + 0 + 4 = 21
    }
}