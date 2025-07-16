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

    [TestMethod]
    public void PackagingWaste_SetExisting()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var sut = new PackagingWaste();
        var registrationMaterials = new List<RegistrationMaterial>
        {
            new()
            {
                Name = Material.Aluminium,
                Applied = false,
                PermitType = PermitType.WasteManagementLicence,
                WeightInTonnes = 10,
                PermitPeriod = PermitPeriod.PerMonth,
                PermitNumber = "345",
                Id = registrationMaterialId,
                Status = MaterialStatus.ReadyToSubmit
            }
        };

        // Act
        sut.SetFromExisting(registrationMaterials);

        // Assert
        sut.SelectedMaterials.Should().BeEquivalentTo(new List<RegistrationMaterial>
        {
            new()
            {
                Applied = false,
                Name = Material.Aluminium,
                PermitType = PermitType.WasteManagementLicence,
                WeightInTonnes = 10,
                PermitPeriod = PermitPeriod.PerMonth,
                PermitNumber = "345",
                Status = MaterialStatus.ReadyToSubmit,
                Id = registrationMaterialId
            }
        });
    }

    [TestMethod]
    public void PackagingWaste_SetExisting_NoMaterials_SetEmptyCollection()
    {
        // Arrange
        var sut = new PackagingWaste();

        // Act
        sut.SetFromExisting(new List<RegistrationMaterial>());

        // Assert
        sut.SelectedMaterials.Should().BeEmpty();
    }

    [TestMethod] 
    public void PackagingWaste_RegistrationMaterialCreated_DoesNotAlreadyExistInSelectedMaterials_AddNewEntry()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var sut = new PackagingWaste();

        // Act
        sut.RegistrationMaterialCreated(new RegistrationMaterial
        {
            Id = registrationMaterialId,
            Name = Material.Aluminium,
            Applied = false,

            // Permit information will not be saved at this point as the journey is not complete.
            // So we do not set these in the assertions as we expect them to be empty/null.
            PermitType = PermitType.WasteManagementLicence,
            PermitNumber = "123",
            PermitPeriod = PermitPeriod.PerMonth,
            WeightInTonnes = 10,
            Exemptions = new List<Exemption>
            {
                new ()
                {
                    ReferenceNumber = "12345"
                }
            }
        });

        // Assert
        sut.SelectedMaterials.Should().BeEquivalentTo(new List<RegistrationMaterial>
        {
            new()
            {
                Id = registrationMaterialId,
                Name = Material.Aluminium,
                Applied = false,
                Exemptions = new List<Exemption>
                {
                    new ()
                    {
                        ReferenceNumber = "12345"
                    }
                }
            }
        });
    }

    [TestMethod]
    public void PackagingWaste_RegistrationMaterialCreated_AlreadyExistsInSelectedMaterials_DoNotAllowNewEntry()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Id = registrationMaterialId,
                    Name = Material.Aluminium,
                    Applied = false,
                    PermitType = PermitType.WasteManagementLicence,
                    PermitNumber = "123",
                    PermitPeriod = PermitPeriod.PerMonth,
                    WeightInTonnes = 10
                }
            ]
        };

        // Act
        sut.RegistrationMaterialCreated(new RegistrationMaterial
        {
            Id = registrationMaterialId,
            Name = Material.Aluminium,
            Applied = false,
            PermitType = PermitType.WasteManagementLicence,
            PermitNumber = "123",
            PermitPeriod = PermitPeriod.PerMonth,
            WeightInTonnes = 10
        });

        // Assert
        sut.SelectedMaterials.Should().BeEquivalentTo(new List<RegistrationMaterial>
        {
            new()
            {
                Id = registrationMaterialId,
                Name = Material.Aluminium,
                Applied = false,
                PermitNumber = "123",
                PermitPeriod = PermitPeriod.PerMonth,
                WeightInTonnes = 10,
                PermitType = PermitType.WasteManagementLicence
            }
        });
    }

    [TestMethod]
    public void PackagingWaste_SetMaterialAsApplied()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Id = registrationMaterialId,
                    Name = Material.Aluminium,
                    Applied = false,
                    PermitType = PermitType.WasteManagementLicence,
                    PermitNumber = "123",
                    PermitPeriod = PermitPeriod.PerMonth,
                    WeightInTonnes = 10
                }
            ]
        };

        // Act
        sut.SetMaterialAsApplied(Material.Aluminium);

        // Assert
        sut.SelectedMaterials.Should().BeEquivalentTo(new List<RegistrationMaterial>
        {
            new()
            {
                Id = registrationMaterialId,
                Name = Material.Aluminium,
                Applied = true,
                PermitNumber = "123",
                PermitPeriod = PermitPeriod.PerMonth,
                WeightInTonnes = 10,
                PermitType = PermitType.WasteManagementLicence
            }
        });
    }

    [TestMethod]
    public void PackagingWaste_SetMaterialAsApplied_NotFound_ThrowException()
    {
        // Arrange
        var registrationMaterialId = Guid.NewGuid();
        var sut = new PackagingWaste
        {
            SelectedMaterials =
            [
                new()
                {
                    Id = registrationMaterialId,
                    Name = Material.Aluminium,
                    Applied = false,
                    PermitType = PermitType.WasteManagementLicence,
                    PermitNumber = "123",
                    PermitPeriod = PermitPeriod.PerMonth,
                    WeightInTonnes = 10
                }
            ]
        };

        // Act & Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => sut.SetMaterialAsApplied(Material.Steel));
    }
}