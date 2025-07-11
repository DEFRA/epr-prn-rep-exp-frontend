namespace Epr.Reprocessor.Exporter.UI.UnitTests.ViewModels.Registration;

[TestClass]
public class CheckYourAnswersWasteDetailsViewModelUnitTests
{
    [TestMethod]
    public void SetPackagingWasteDetails_EnsureSectionsCorrectlySet()
    {
        // Arrange
        var exemptions = new List<Exemption>
        {
            new() { ReferenceNumber = "reference1" },
            new() { ReferenceNumber = "reference2" }
        };
        var sut = new CheckYourAnswersWasteDetailsViewModel();

        // Act
        sut.SetPackagingWasteDetails(new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = Material.Aluminium,
                    PermitType = PermitType.InstallationPermit,
                    PermitNumber = "123",
                    WeightInTonnes = 10,
                    PermitPeriod = PermitPeriod.PerMonth,
                    MaxCapableWeightInTonnes = 20,
                    MaxCapableWeightPeriodDuration = PeriodDuration.PerYear
                },

                new()
                {
                    Id = Guid.Empty,
                    Name = Material.Glass,
                    PermitType = PermitType.PollutionPreventionAndControlPermit,
                    PermitNumber = "456",
                    WeightInTonnes = 20,
                    PermitPeriod = PermitPeriod.PerWeek,
                    MaxCapableWeightInTonnes = 30,
                    MaxCapableWeightPeriodDuration = PeriodDuration.PerMonth
                },

                new()
                {
                    Id = Guid.Empty,
                    Name = Material.Plastic,
                    PermitType = PermitType.WasteExemption,
                    MaxCapableWeightInTonnes = 30,
                    MaxCapableWeightPeriodDuration = PeriodDuration.PerMonth,
                    Exemptions = exemptions
                }
            ]
        });

        // Assert
        sut.Materials.Should().BeEquivalentTo(new SummaryListModel
        {
            Rows =
            [
                new()
                {
                    Key = "Packaging waste the site has a permit or exemption to accept and recycle",
                    Value = "Aluminium, Glass, Plastic",
                    ChangeLinkUrl = PagePaths.WastePermitExemptions,
                    ChangeLinkHiddenAccessibleText = "the selected materials"
                }
            ]
        });

        sut.MaterialPermits.Should().BeEquivalentTo(new List<SummaryListModel>
        {
            new ()
            {
                Heading = "Permit for accepting and reprocessing Aluminium",
                Rows =
                [
                    new()
                    {
                        Key = "Installation permit",
                        Value = "123",
                        ChangeLinkUrl = PagePaths.PermitForRecycleWaste,
                        ChangeLinkHiddenAccessibleText = "the permit details"
                    },
                    new()
                    {
                        Key = "The maximum weight the permit authorises the site to accept and recycle (tonnes)",
                        Value = "10",
                        ChangeLinkUrl = PagePaths.InstallationPermit,
                        ChangeLinkHiddenAccessibleText = "the permit weight for the material"
                    },
                    new()
                    {
                        Key = "Per year, month or week",
                        Value = "Per month",
                        ChangeLinkUrl = PagePaths.InstallationPermit,
                        ChangeLinkHiddenAccessibleText = "the permit period for the material"
                    },
                    new()
                    {
                        Key = "Maximum weight the site is currently capable of reprocessing (tonnes)",
                        Value = "20",
                        ChangeLinkUrl = PagePaths.MaximumWeightSiteCanReprocess,
                        ChangeLinkHiddenAccessibleText = "the maximum weight the site can reprocess for the material"
                    },
                    new()
                    {
                        Key = "Per year, month or week",
                        Value = "Per year",
                        ChangeLinkUrl = PagePaths.MaximumWeightSiteCanReprocess,
                        ChangeLinkHiddenAccessibleText = "the maximum weight period for the material"
                    }
                ]
            },
            new ()
            {
                Heading = "Permit for accepting and reprocessing Glass",
                Rows =
                [
                    new()
                    {
                        Key = "Pollution, Prevention and Control (PPC) permit",
                        Value = "456",
                        ChangeLinkUrl = PagePaths.PermitForRecycleWaste,
                        ChangeLinkHiddenAccessibleText = "the permit details"
                    },
                    new()
                    {
                        Key = "The maximum weight the permit authorises the site to accept and recycle (tonnes)",
                        Value = "20",
                        ChangeLinkUrl = PagePaths.PpcPermit,
                        ChangeLinkHiddenAccessibleText = "the permit weight for the material"
                    },
                    new()
                    {
                        Key = "Per year, month or week",
                        Value = "Per week",
                        ChangeLinkUrl = PagePaths.PpcPermit,
                        ChangeLinkHiddenAccessibleText = "the permit period for the material"
                    },
                    new()
                    {
                        Key = "Maximum weight the site is currently capable of reprocessing (tonnes)",
                        Value = "30",
                        ChangeLinkUrl = PagePaths.MaximumWeightSiteCanReprocess,
                        ChangeLinkHiddenAccessibleText = "the maximum weight the site can reprocess for the material"
                    },
                    new()
                    {
                        Key = "Per year, month or week",
                        Value = "Per month",
                        ChangeLinkUrl = PagePaths.MaximumWeightSiteCanReprocess,
                        ChangeLinkHiddenAccessibleText = "the maximum weight period for the material"
                    }
                ]
            },
            new ()
            {
                Heading = "Permit for accepting and reprocessing Plastic",
                Rows =
                [
                    new()
                    {
                        Key = "Waste exemption",
                        Value = string.Join(Environment.NewLine, exemptions.Select(o => o.ReferenceNumber)),
                        ChangeLinkUrl = PagePaths.ExemptionReferences,
                        ChangeLinkHiddenAccessibleText = "the exemption references"
                    },
                    new()
                    {
                        Key = "Maximum weight the site is currently capable of reprocessing (tonnes)",
                        Value = "30",
                        ChangeLinkUrl = PagePaths.MaximumWeightSiteCanReprocess,
                        ChangeLinkHiddenAccessibleText = "the maximum weight the site can reprocess for the material"
                    },
                    new()
                    {
                        Key = "Per year, month or week",
                        Value = "Per month",
                        ChangeLinkUrl = PagePaths.MaximumWeightSiteCanReprocess,
                        ChangeLinkHiddenAccessibleText = "the maximum weight period for the material"
                    }
                ]
            }
        });
    }
}