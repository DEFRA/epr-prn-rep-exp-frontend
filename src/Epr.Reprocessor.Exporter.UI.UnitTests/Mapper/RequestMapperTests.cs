namespace Epr.Reprocessor.Exporter.UI.UnitTests.Mapper;

[TestClass]
public class RequestMapperTests
{
    private readonly Mock<ISessionManager<ReprocessorRegistrationSession>> _mockSessionManager = new();
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
    private readonly Mock<HttpContext> _mockHttpContext = new();
    private readonly Mock<ClaimsPrincipal> _userMock = new();
    
    [TestMethod]
    public async Task MapForCreate_NullSession_ShouldThrowException()
    {
        // Arrange
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };
        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession?)null);

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act & Assert
        var act = async () => await sut.MapForCreate();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task MapForCreate_NullOrganisationId_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession()
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupDefaultUserAndSessionMocks();

        // Act & Assert
        var act = async () => await sut.MapForCreate();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task MapForCreate_NullReprocessingSite_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession()
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act & Assert
        var act = async () => await sut.MapForCreate();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task MapForCreate_NullReprocessingSiteAddress_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession
                    {
                        ReprocessingSite = new ReprocessingSite
                        {
                            Address = null
                        }
                    }
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act & Assert
        var act = async () => await sut.MapForCreate();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task MapForCreate_ShouldReturnMappedObject()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };

        var expectedDto = new CreateRegistrationDto
        {
            OrganisationId = Guid.Empty,
            ApplicationTypeId = 1,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1",
                AddressLine2 = "Address line 2",
                Country = "Country",
                County = "County",
                GridReference = "T12345",
                PostCode = "CV12TT",
                TownCity = "Town",
                NationId = 1
            }
        };

        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession
                    {
                        ReprocessingSite = new()
                        {
                            Address = new Address("Address line 1", "Address line 2", "Locality", "Town", "County", "Country", "CV12TT"),
                            SiteGridReference = "T12345",
                            ServiceOfNotice = new ServiceOfNotice
                            {
                                Address = new Address("Address line 1", "Address line 2", "Locality", "Town", "County", "Country", "CV12TT")
                            },
                            Nation = UkNation.England
                        }
                    }
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act
        var result = await sut.MapForCreate();

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task MapForUpdate_NullOrganisationId_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession()
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupDefaultUserAndSessionMocks();

        // Act & Assert
        var act = async () => await sut.MapForUpdate();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task MapForUpdate_NullSession_ShouldThrowException()
    {
        // Arrange
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };
        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync((ReprocessorRegistrationSession?)null);

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act & Assert
        var act = async () => await sut.MapForUpdate();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task MapForUpdate_ShouldReturnMappedObject()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };

        var expectedDto = new UpdateRegistrationRequestDto
        {
            RegistrationId = id,
            OrganisationId = Guid.Empty,
            ApplicationTypeId = ApplicationType.Reprocessor,
            ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = "Address line 1",
                AddressLine2 = "Address line 2",
                Country = "Country",
                County = "County",
                GridReference = "T12345",
                PostCode = "CV12TT",
                TownCity = "Town",
                NationId = 1,
            },
            LegalAddress = new AddressDto
            {
                AddressLine1 = "Address line 1",
                AddressLine2 = "Address line 2",
                Country = "Country",
                County = "County",
                PostCode = "CV12TT",
                TownCity = "Town",
                GridReference = ""
            }
        };

        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession
                    {
                        ReprocessingSite = new ReprocessingSite
                        {
                            Address = new Address("Address line 1", "Address line 2", "Locality", "Town", "County", "Country", "CV12TT"),
                            Nation = UkNation.England,
                            SiteGridReference = "T12345",
                            ServiceOfNotice = new ServiceOfNotice
                            {
                                Address = new Address("Address line 1", "Address line 2", "Locality", "Town", "County", "Country", "CV12TT")
                            }
                        }
                    }
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act
        var result = await sut.MapForUpdate();

        // Assert
        result.Should().BeEquivalentTo(expectedDto, options => options.Excluding(x => x.BusinessAddress));
    }

    [TestMethod]
    public async Task MapForUpdate_ReprocessingSiteANdNoticeIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userData = new UserData
        {
            Id = Guid.NewGuid(),
            Organisations =
            [
                new()
                {
                    Id = Guid.Empty,
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            ]
        };

        var expectedDto = new UpdateRegistrationRequestDto
        {
            RegistrationId = id,
            OrganisationId = Guid.Empty,
            ApplicationTypeId = ApplicationType.Reprocessor
        };

        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Expectations
        _mockSessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>()))
            .ReturnsAsync(
                new ReprocessorRegistrationSession
                {
                    RegistrationId = id,
                    RegistrationApplicationSession = new RegistrationApplicationSession
                    {
                        ReprocessingSite = null
                    }
                });

        _mockHttpContextAccessor.Setup(o => o.HttpContext).Returns(_mockHttpContext.Object);

        SetupMockHttpContext(CreateClaims(userData));

        // Act
        var result = await sut.MapForUpdate();

        // Assert
        result.Should().BeEquivalentTo(expectedDto, options => options.Excluding(x => x.BusinessAddress));
    }

    [TestMethod]
    [DataRow(UkNation.England)]
    [DataRow(UkNation.Wales)]
    public async Task MapAuthorisationTypes_EnsureCorrectPermitReturnedForNationEnglandAndWales(UkNation nation)
    {
        // Arrange
        var permits = new List<MaterialsPermitTypeDto>
        {
            new()
            {
                Id = 1, Name = nameof(PermitType.WasteExemption)
            },
            new()
            {
                Id = 2, Name = nameof(PermitType.PollutionPreventionAndControlPermit)
            },
            new()
            {

                Id = 3, Name = nameof(PermitType.WasteManagementLicence)
            },
            new()
            {

                Id = 4, Name = nameof(PermitType.InstallationPermit)
            },
            new()
            {
                Id = 5, Name = nameof(PermitType.EnvironmentalPermitOrWasteManagementLicence)
            }
        };

        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Act
        var result = await sut.MapAuthorisationTypes(permits, nation.ToString());

        // Assert
        result.Should().BeEquivalentTo(new List<AuthorisationTypes>
        {
            new()
            {
                Id = 5,
                Name = "Environmental permit or waste management licence",
                Label = "Enter permit or licence number",
                NationCodeCategory = [nameof(UkNation.England), nameof(UkNation.Wales)]
            },
            new()
            {
                Id = 4,
                Name = "Installation permit",
                Label = "Enter permit number",
                NationCodeCategory = [nameof(UkNation.England), nameof(UkNation.Wales)]
            },
            new()
            {
                Id = 1,
                Name = "Waste exemption",
                Label = "",
                NationCodeCategory = [nameof(UkNation.England), nameof(UkNation.Wales), nameof(UkNation.NorthernIreland), nameof(UkNation.Scotland)]
            }
        });
    }

    [TestMethod]
    [DataRow(UkNation.Scotland)]
    [DataRow(UkNation.NorthernIreland)]
    public async Task MapAuthorisationTypes_EnsureCorrectPermitReturnedForNationScotlandAndNorthernIreland(UkNation nation)
    {
        // Arrange
        var permits = new List<MaterialsPermitTypeDto>
        {
            new()
            {
                Id = 1, Name = nameof(PermitType.WasteExemption)
            },
            new()
            {
                Id = 2, Name = nameof(PermitType.PollutionPreventionAndControlPermit)
            },
            new()
            {

                Id = 3, Name = nameof(PermitType.WasteManagementLicence)
            },
            new()
            {

                Id = 4, Name = nameof(PermitType.InstallationPermit)
            },
            new()
            {
                Id = 5, Name = nameof(PermitType.EnvironmentalPermitOrWasteManagementLicence)
            }
        };

        var sut = new RequestMapper(_mockSessionManager.Object, _mockHttpContextAccessor.Object);

        // Act
        var result = await sut.MapAuthorisationTypes(permits, nation.ToString());

        // Assert
        result.Should().BeEquivalentTo(new List<AuthorisationTypes>
        {
            new()
            {
                Id = 2,
                Name = "Pollution, Prevention and Control (PPC) permit",
                Label = "Enter permit number",
                NationCodeCategory = [nameof(UkNation.Scotland), nameof(UkNation.NorthernIreland)]
            },
            new()
            {
                Id = 1,
                Name = "Waste exemption",
                Label = "",
                NationCodeCategory = [nameof(UkNation.England), nameof(UkNation.Wales), nameof(UkNation.NorthernIreland), nameof(UkNation.Scotland)]
            },
            new()
            {
                Id = 3,
                Name = "Waste management licence",
                Label = "Enter licence number",
                NationCodeCategory = [nameof(UkNation.Scotland), nameof(UkNation.NorthernIreland)]
            }
        });
    }

    private void SetupDefaultUserAndSessionMocks()
    {
        SetupMockSession();
        SetupMockHttpContext(CreateClaims(GetUserData()));
    }

    private void SetupMockSession()
    {
        _mockSessionManager.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(new ReprocessorRegistrationSession());
    }

    private void SetupMockHttpContext(List<Claim> claims)
    {
        _userMock.Setup(x => x.Claims).Returns(claims);
        _mockHttpContext.Setup(x => x.User).Returns(_userMock.Object);
    }

    private static List<Claim> CreateClaims(UserData? userData)
    {
        var claims = new List<Claim>();
        if (userData != null)
        {
            claims.Add(new(ClaimTypes.UserData, JsonConvert.SerializeObject(userData)));
        }

        return claims;
    }

    private static UserData GetUserData()
    {
        return new UserData
        {
            Id = Guid.NewGuid(),
            Organisations = new()
            {
                new()
                {
                    Name = "Tesr",
                    OrganisationNumber = "123456",
                }
            }
        };
    }
}