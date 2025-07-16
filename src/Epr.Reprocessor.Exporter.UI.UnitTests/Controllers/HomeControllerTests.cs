using Epr.Reprocessor.Exporter.UI.UnitTests.Builders;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    [TestClass]
    public class HomeControllerTests : ControllerTests<HomeController>
    {
        private Mock<IOptions<HomeViewModel>> _mockOptions = null!;
        private Mock<ISessionManager<ReprocessorRegistrationSession>> _mockSessionManagerMock = null!;
        private Mock<IOrganisationAccessor> _mockOrganisationAccessor = null!;
        private HomeController _controller = null!;
        private UserData _userData = NewUserData().Build();
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<IOptions<FrontEndAccountCreationOptions>> _mockFrontEndAccountCreationOptions = null!;
        private Mock<IOptions<ExternalUrlOptions>> _mockExternalUrlOptions = null!;

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
            
            _mockOptions = new Mock<IOptions<HomeViewModel>>();
            _mockSessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
            _mockOrganisationAccessor = new Mock<IOrganisationAccessor>();
            _mockExternalUrlOptions = new Mock<IOptions<ExternalUrlOptions>>();
            _mockFrontEndAccountCreationOptions = new Mock<IOptions<FrontEndAccountCreationOptions>>();

            var homeSettings = new HomeViewModel
            {
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications",
                RegistrationReprocessorContinueLink = "/RegistrationReprocessorContinueLink",
                RegistrationExporterContinueLink = "/RegistrationExporterContinueLink",
                AccreditationStartLink = "/AccreditationStartLink",
                AccreditationReprocessorContinueLink = "/AccreditationReprocessorContinueLink",
                AccreditationExporterContinueLink = "/AccreditationExporterContinueLink"
            };

            _mockOptions.Setup(x => x.Value).Returns(homeSettings);

            var frontendOptions = new FrontEndAccountCreationOptions()
            {
                AddOrganisation = "AddOrganisaion",
                CreateUser = "CreateUser"
            };
            _mockFrontEndAccountCreationOptions.Setup(x => x.Value).Returns(frontendOptions);

            var externalUrlsOptions = new ExternalUrlOptions()
            {
                ReadMoreAboutApprovedPerson = "govuk"
            };

            _mockExternalUrlOptions.Setup(x => x.Value).Returns(externalUrlsOptions);
            _mockOptions.Setup(x => x.Value).Returns(homeSettings);

            _controller = new HomeController(_mockOptions.Object, MockReprocessorService.Object,
                _mockSessionManagerMock.Object, _mockOrganisationAccessor.Object,
                _mockFrontEndAccountCreationOptions.Object, _mockExternalUrlOptions.Object);
            
            // Assign the fake user to controller context
            _mockHttpContext = new Mock<HttpContext>();
        }

        [TestMethod]
        public async Task Index_redirects_to_ManageOrganisationIf_Organisation_Exists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userData = new UserDataBuilder().Build();
            var session = new ReprocessorRegistrationSession
            {
                RegistrationId = id
            };

            var expectedSession = new ReprocessorRegistrationSession
            {
                RegistrationId = id,
                RegistrationApplicationSession = new()
                {
                    ReprocessingSite = new()
                    {
                        Address = new("Test Street", "Test Street 2", null, "Test Town", "County", "Country", "CV12TT"),
                        ServiceOfNotice = new()
                        {
                            Address = new("Test Street", "Test Street 2", null, "Test Town", "County", "Country", "CV12TT"),
                            TypeOfAddress = AddressOptions.SiteAddress
                        }
                    },
                }
            };

            var existingRegistration = new RegistrationDto
            {
                Id = id,
                OrganisationId = Guid.Empty,
                ReprocessingSiteAddress = new AddressDto
                {
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Street 2",
                    TownCity = "Test Town",
                    County = "County",
                    Country = "Country",
                    PostCode = "CV12TT",
                },
                LegalDocumentAddress = new AddressDto
                {
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Street 2",
                    TownCity = "Test Town",
                    County = "County",
                    Country = "Country",
                    PostCode = "CV12TT",
                }
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };

            // Expectations
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty)).ReturnsAsync(existingRegistration);
            _mockSessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
            session.Should().BeEquivalentTo(expectedSession);
        }

        [TestMethod] public async Task Index_redirects_to_ManageOrganisationIf_Organisation_Exists_SessionIsNull_ShouldCreate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userData = new UserDataBuilder().Build();
            var session = new ReprocessorRegistrationSession
            {
                RegistrationId = id
            };

            var expectedSession = new ReprocessorRegistrationSession
            {
                RegistrationId = id,
                RegistrationApplicationSession = new()
                {
                    ReprocessingSite = new()
                    {
                        Address = new("Test Street", "Test Street 2", null, "Test Town", "County", "Country", "CV12TT"),
                        ServiceOfNotice = new()
                        {
                            Address = new("Test Street", "Test Street 2", null, "Test Town", "County", "Country", "CV12TT"),
                            TypeOfAddress = AddressOptions.SiteAddress
                        }
                    },
                }
            };

            var existingRegistration = new RegistrationDto
            {
                Id = id,
                OrganisationId = Guid.Empty,
                ReprocessingSiteAddress = new AddressDto
                {
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Street 2",
                    TownCity = "Test Town",
                    County = "County",
                    Country = "Country",
                    PostCode = "CV12TT",
                },
                LegalDocumentAddress = new AddressDto
                {
                    AddressLine1 = "Test Street",
                    AddressLine2 = "Test Street 2",
                    TownCity = "Test Town",
                    County = "County",
                    Country = "Country",
                    PostCode = "CV12TT",
                }
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };

            // Expectations
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty)).ReturnsAsync(existingRegistration);
            _mockSessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync((ReprocessorRegistrationSession?)null);

            _mockSessionManagerMock.Setup(o => o.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ReprocessorRegistrationSession>())).Returns(Task.CompletedTask).Verifiable(Times.Exactly(1));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
            _mockSessionManagerMock.Verify();
        }

        [TestMethod]
        public async Task Index_redirects_to_AddOrganisationIf_UserExistsButNo_Organisation()
        {
            // Expectations
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns((ClaimsPrincipal?)null);
            //_mockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty))
            //    .ReturnsAsync(new RegistrationDto());

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.AddOrganisation));
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)Material.Plastic,
                    ApplicationTypeId = ApplicationType.Reprocessor,
                    ReprocessingSiteAddress = new AddressDto
                    {
                        AddressLine1 = "123 Test St",
                        TownCity = "Test City"
                    },
                    RegistrationStatus = (int)RegistrationStatus.InProgress,
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.NotAccredited,
                    Year = 2024,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 1
                }
            };
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());
            // Act
            var result = await _controller.ManageOrganisation();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult!.Model, typeof(HomeViewModel));
            var model = viewResult.Model as HomeViewModel;
            var expectedRegistrationLink = $"/RegistrationReprocessorContinueLink/{registrationData[0].Id}/{registrationData[0].MaterialId}";
            var expectedAccreditationStartLink = $"/AccreditationStartLink/{registrationData[0].ReprocessingSiteId}/{registrationData[0].MaterialId}";
            var expectedAccreditationContinueLink = $"/AccreditationReprocessorContinueLink/{registrationData[0].ReprocessingSiteId}/{registrationData[0].MaterialId}";
            model.RegistrationData[0].RegistrationContinueLink.Should().Be(expectedRegistrationLink);
            model.AccreditationData[0].AccreditationStartLink.Should().Be(expectedAccreditationStartLink);
            model.AccreditationData[0].AccreditationContinueLink.Should().Be(expectedAccreditationContinueLink);
            model.RegistrationData[0].Material.Should().Be((Material)(int)Material.Plastic);
            model.RegistrationData[0].ApplicationType.Should().Be(ApplicationType.Reprocessor);
            model.RegistrationData[0].SiteAddress.Should().Be("123 Test St, Test City");
            model.RegistrationData[0].RegistrationStatus.Should().Be(RegistrationStatus.InProgress);
            model.RegistrationData[0].Year.Should().Be(2024);
            model.AccreditationData[0].Material.Should().Be((Material)(int)Material.Plastic);
            model.AccreditationData[0].ApplicationType.Should().Be(ApplicationType.Reprocessor);
            model.AccreditationData[0].SiteAddress.Should().Be("123 Test St,Test City");
            model.AccreditationData[0].AccreditationStatus.Should().Be(Enums.AccreditationStatus.NotAccredited);
            model.AccreditationData[0].Year.Should().Be(2024);
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_With_No_Registration_AccreditationData()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            var registrationData = new List<RegistrationDto>();

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult!.Model, typeof(HomeViewModel));

            var model = viewResult.Model as HomeViewModel;
            Assert.IsNotNull(model); // Ensure model is not null

            // Assert individual fields of HomeViewModel
            Assert.AreEqual(_userData.FirstName, model.FirstName);
            Assert.AreEqual(_userData.LastName, model.LastName);
            Assert.AreEqual("/apply-for-registration", model.ApplyForRegistration);
            Assert.AreEqual("/view-applications", model.ViewApplications);
            Assert.AreEqual(_userData.Organisations[0].Name!, model.OrganisationName);
            Assert.AreEqual(_userData.Organisations[0].OrganisationNumber!, model.OrganisationNumber);
            Assert.AreEqual(0, model.RegistrationData.Count);
            Assert.AreEqual(0, model.AccreditationData.Count);
        }

        [TestMethod]
        public async Task ManageOrganisation_WithNoRegistrationData_ReturnsEmptyLists()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(new List<RegistrationDto>());

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult!.Model as HomeViewModel;
            
            model!.RegistrationData.Should().BeEmpty();
            model.AccreditationData.Should().BeEmpty();
        }

        [TestMethod]
        public async Task ManageOrganisation_RedirectToIndex_If_NoOrgInUserDataAsync()
        {
            var userData = new UserDataBuilder().Build();
            userData.Organisations.RemoveAt(0);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            // Act
            var result = await _controller.ManageOrganisation();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirect.ActionName.Should().Be(nameof(HomeController.Index));

        }

        [TestMethod]
        public void AddOrganisation_ReturnsViewResult()
        {
            var userData = new UserDataBuilder().Build();
            userData.Organisations.RemoveAt(0);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));

            // Act
            var result = _controller.AddOrganisation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;

            Assert.IsInstanceOfType(viewResult!.Model, typeof(AddOrganisationViewModel));

            var model = viewResult.Model as AddOrganisationViewModel;
            model.Should().BeEquivalentTo(new AddOrganisationViewModel()
            {
                FirstName = "first",
                LastName = "last",
                AddOrganisationLink = _mockFrontEndAccountCreationOptions.Object.Value.AddOrganisation,
                ReadMoreAboutApprovedPersonLink = _mockExternalUrlOptions.Object.Value.ReadMoreAboutApprovedPerson
            });
            model.FullName.Should().Be("first last");
        }

        [TestMethod]
        public void AddOrganisation_RedirectToIndex_If_OrgInUserData()
        {
            var userData = new UserDataBuilder().Build();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);

            // Act
            var result = _controller.AddOrganisation();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirect.ActionName.Should().Be(nameof(HomeController.Index));

        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange
            var activity = new Activity("test");
            activity.Start();
            Activity.Current = activity;

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ErrorViewModel));
            var model = result.Model as ErrorViewModel;
            Assert.AreEqual(activity.Id, model!.RequestId);
        }

        [TestMethod]
        public async Task Index_redirects_to_SelectOrganisationIf_Multiple_Organisations_Exist()
        {
            // Arrange
            var userData = NewUserData().Build();
            userData.Organisations.Add(new Organisation() { OrganisationNumber = "1234" });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            // Expectations
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            MockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty))
                .ReturnsAsync((RegistrationDto?)null);

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.SelectOrganisation));
        }

        [TestMethod]
        public void SelectOrganisation_ReturnsViewResultWithCorrectModel()
        {
            // Arrange
            var userData = NewUserData().Build();
            userData.Organisations.Add(new Organisation() { OrganisationNumber = "1234" });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            // Expectations
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));

            var result = _controller.SelectOrganisation();

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.Model.Should().BeOfType<SelectOrganisationViewModel>();

            var model = viewResult.Model as SelectOrganisationViewModel;
            model!.FirstName.Should().Be(_userData.FirstName);
            model.LastName.Should().Be(_userData.LastName);
            model.Organisations.Should().HaveCount(userData.Organisations.Count);
            model.Organisations[0].OrganisationName.Should().Be(_userData.Organisations[0].Name);
            model.Organisations[0].OrganisationNumber.Should().Be(_userData.Organisations[0].OrganisationNumber);
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_WithMultipleRegistrationAndAccreditationTypes()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)Material.Plastic,
                    ApplicationTypeId = ApplicationType.Reprocessor,
                    ReprocessingSiteAddress = new AddressDto { AddressLine1 = "Reproc St", TownCity = "Reproc City" },
                    RegistrationStatus = (int)RegistrationStatus.InProgress,
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.NotAccredited,
                    Year = 2024,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 10
                },
                new()
                {
                    MaterialId = (int)Material.Glass,
                    ApplicationTypeId = ApplicationType.Exporter,
                    ReprocessingSiteAddress = new AddressDto { AddressLine1 = "Export St", TownCity = "Export City" },
                    RegistrationStatus = (App.Enums.Registration.RegistrationStatus)RegistrationStatus.Submitted,
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.Accepted,
                    Year = 2025,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 20
                },
                new()
                {
                    MaterialId = (int)Material.Steel,
                    ApplicationTypeId = (ApplicationType)999,
                    ReprocessingSiteAddress = new AddressDto { AddressLine1 = "Unknown St", TownCity = "Unknown City" },
                    RegistrationStatus = (App.Enums.Registration.RegistrationStatus)RegistrationStatus.Refused,
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.Refused,
                    Year = 2026,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 30
                }
            };
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());
            // Act
            var result = await _controller.ManageOrganisation();
            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult!.Model as HomeViewModel;
            model!.RegistrationData.Should().HaveCount(3);
            model.RegistrationData[0].ApplicationType.Should().Be(ApplicationType.Reprocessor);
            model.RegistrationData[1].ApplicationType.Should().Be(ApplicationType.Exporter);
            model.RegistrationData[2].ApplicationType.Should().Be((ApplicationType)999);
            // Check links
            var expectedLink0 = $"/RegistrationReprocessorContinueLink/{registrationData[0].Id}/{registrationData[0].MaterialId}";
            var expectedLink1 = $"/RegistrationExporterContinueLink/{registrationData[1].Id}/{registrationData[1].MaterialId}";
            model.RegistrationData[0].RegistrationContinueLink.Should().Be(expectedLink0);
            model.RegistrationData[1].RegistrationContinueLink.Should().Be(expectedLink1);
            model.RegistrationData[2].RegistrationContinueLink.Should().Be("");
            // AccreditationData links
            var expectedAccreditationStartLink0 = $"/AccreditationStartLink/{registrationData[0].ReprocessingSiteId}/{registrationData[0].MaterialId}";
            var expectedAccreditationContinueLink0 = $"/AccreditationReprocessorContinueLink/{registrationData[0].ReprocessingSiteId}/{registrationData[0].MaterialId}";
            var expectedAccreditationStartLink1 = $"/AccreditationStartLink/{registrationData[1].MaterialId}";
            var expectedAccreditationContinueLink1 = $"/AccreditationExporterContinueLink/{registrationData[1].MaterialId}";
            model.AccreditationData[0].AccreditationStartLink.Should().Be(expectedAccreditationStartLink0);
            model.AccreditationData[0].AccreditationContinueLink.Should().Be(expectedAccreditationContinueLink0);
            model.AccreditationData[1].AccreditationStartLink.Should().Be(expectedAccreditationStartLink1);
            model.AccreditationData[1].AccreditationContinueLink.Should().Be(expectedAccreditationContinueLink1);
            model.AccreditationData[2].AccreditationStartLink.Should().Be("");
            model.AccreditationData[2].AccreditationContinueLink.Should().Be("");
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_WithNullAddressFields()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)Material.Paper,
                    ApplicationTypeId = ApplicationType.Reprocessor,
                    ReprocessingSiteAddress = null, // Null address
                    RegistrationStatus = (int)RegistrationStatus.InProgress,
                    Year = 2024,
                    Id = Guid.NewGuid()
                }
            };
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());
            // Act
            var result = await _controller.ManageOrganisation();
            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult!.Model as HomeViewModel;
            model!.RegistrationData[0].SiteAddress.Should().Be(", "); // Should handle null gracefully
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_AccreditationData_ExporterAndUnknownTypes()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)Material.Paper,
                    ApplicationTypeId = ApplicationType.Exporter,
                    ReprocessingSiteAddress = new AddressDto { AddressLine1 = "ExportAcc St", TownCity = "ExportAcc City" },
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.Submitted,
                    Year = 2027,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 5
                },
                new()
                {
                    MaterialId = (int)Material.Steel,
                    ApplicationTypeId = (ApplicationType)999,
                    ReprocessingSiteAddress = new AddressDto { AddressLine1 = "UnknownAcc St", TownCity = "UnknownAcc City" },
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.Refused,
                    Year = 2028,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 6
                }
            };
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            MockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());
            // Act
            var result = await _controller.ManageOrganisation();
            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult!.Model as HomeViewModel;
            model!.AccreditationData.Should().HaveCount(2);
            // Exporter type
            var expectedStartLink = $"/AccreditationStartLink/{registrationData[0].MaterialId}";
            var expectedContinueLink = $"/AccreditationExporterContinueLink/{registrationData[0].MaterialId}";
            model.AccreditationData[0].ApplicationType.Should().Be(ApplicationType.Exporter);
            model.AccreditationData[0].AccreditationStartLink.Should().Be(expectedStartLink);
            model.AccreditationData[0].AccreditationContinueLink.Should().Be(expectedContinueLink);
            // Unknown type
            model.AccreditationData[1].ApplicationType.Should().Be((ApplicationType)999);
            model.AccreditationData[1].AccreditationStartLink.Should().Be("");
            model.AccreditationData[1].AccreditationContinueLink.Should().Be("");
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(UserData userData)
        {
            var jsonUserData = JsonSerializer.Serialize(userData);
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, jsonUserData)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }

    }
}