namespace Epr.Reprocessor.Exporter.UI.UnitTests.Domain;

[TestClass]
public class PackagingWasteUnitTests
{
    [TestMethod]
    public void CurrentMaterialApplyingFor_MultipleMaterials_OneApplied_ReturnFirstInAlphabeticalOrder()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = false,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        var currentMaterial = sut.CurrentMaterialApplyingFor;

        // Assert
        currentMaterial.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Name = Material.Aluminium,
            Applied = false
        });
    }

    [TestMethod]
    public void CurrentMaterialApplyingFor_MultipleMaterials_AllApplied_ReturnNull()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        var currentMaterial = sut.CurrentMaterialApplyingFor;

        // Assert
        currentMaterial.Should().BeNull();
    }

    [TestMethod]
    public void PackagingWaste_SetInstallationPermit()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        sut.SetInstallationPermit(10, (int)PermitPeriod.PerMonth);

        // Assert
        sut.CurrentMaterialApplyingFor.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Applied = false,
            Name = Material.Aluminium,
            PermitType = PermitType.InstallationPermit,
            WeightInTonnes = 10,
            PermitPeriod = PermitPeriod.PerMonth
        });
    }

    [TestMethod]
    public void PackagingWaste_SetPpcPermit()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        sut.SetPPCPermit(10, (int)PermitPeriod.PerMonth);

        // Assert
        sut.CurrentMaterialApplyingFor.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Applied = false,
            Name = Material.Aluminium,
            PermitType = PermitType.PollutionPreventionAndControlPermit,
            WeightInTonnes = 10,
            PermitPeriod = PermitPeriod.PerMonth
        });
    }

    [TestMethod]
    public void PackagingWaste_SetEnvironmentalPermit()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        sut.SetEnvironmentalPermitOrWasteManagementLicence(10, (int)PermitPeriod.PerMonth);

        // Assert
        sut.CurrentMaterialApplyingFor.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Applied = false,
            Name = Material.Aluminium,
            PermitType = PermitType.EnvironmentalPermitOrWasteManagementLicence,
            WeightInTonnes = 10,
            PermitPeriod = PermitPeriod.PerMonth
        });
    }

    [TestMethod]
    public void PackagingWaste_SetWasteManagementLicence()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        sut.SetWasteManagementLicence(10, (int)PermitPeriod.PerMonth);

        // Assert
        sut.CurrentMaterialApplyingFor.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Applied = false,
            Name = Material.Aluminium,
            PermitType = PermitType.WasteManagementLicence,
            WeightInTonnes = 10,
            PermitPeriod = PermitPeriod.PerMonth
        });
    }

    [TestMethod]
    public void PackagingWaste_SetSelectedAuthorisation_PermitTypeDifferentToStored()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                    PermitType = PermitType.WasteManagementLicence,
                    PermitNumber = "123",
                    PermitPeriod = PermitPeriod.PerMonth,
                    WeightInTonnes = 10
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        sut.SetSelectedAuthorisation(PermitType.InstallationPermit, "345");

        // Assert
        sut.CurrentMaterialApplyingFor.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Applied = false,
            Name = Material.Aluminium,
            PermitType = PermitType.InstallationPermit,
            WeightInTonnes = 0,
            PermitPeriod = null,
            PermitNumber = "345"
        });
    }

    [TestMethod]
    public void PackagingWaste_SetSelectedAuthorisation_PermitTypeNotDifferentToStored()
    {
        // Arrange
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Name = Material.Glass,
                    Applied = true,
                },

                new()
                {
                    Name = Material.Aluminium,
                    Applied = false,
                    PermitType = PermitType.WasteManagementLicence,
                    PermitNumber = "123",
                    PermitPeriod = PermitPeriod.PerMonth,
                    WeightInTonnes = 10
                },

                new()
                {
                    Name = Material.Paper,
                    Applied = true,
                }
            ]
        };

        // Act
        sut.SetSelectedAuthorisation(PermitType.WasteManagementLicence, "345");

        // Assert
        sut.CurrentMaterialApplyingFor.Should().BeEquivalentTo(new RegistrationMaterial
        {
            Applied = false,
            Name = Material.Aluminium,
            PermitType = PermitType.WasteManagementLicence,
            WeightInTonnes = 10,
            PermitPeriod = PermitPeriod.PerMonth,
            PermitNumber = "345"
        });
    }
}