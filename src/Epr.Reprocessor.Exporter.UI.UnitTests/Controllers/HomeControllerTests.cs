using Epr.Reprocessor.Exporter.UI.UnitTests.Builders;
using Epr.Reprocessor.Exporter.UI.ViewModels.Team;
using EPR.Common.Authorization.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    //[Ignore("Need to come back to this as some funky threading issues are occuring.")]
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<IOptions<HomeViewModel>> _mockOptions = null!;
        private Mock<IReprocessorService> _mockReprocessorService = null!;
        private Mock<ISessionManager<ReprocessorRegistrationSession>> _mockSessionManagerMock = null!;
		private Mock<ISessionManager<JourneySession>> _mockJourneySessionManager = null!;
		private Mock<IOrganisationAccessor> _mockOrganisationAccessor = null!;
        private HomeController _controller = null!;
        private UserData _userData = NewUserData.Build();
        private Mock<HttpContext> _mockHttpContext = null!;
        private Mock<IOptions<FrontEndAccountCreationOptions>> _mockFrontEndAccountCreationOptions;
        private Mock<IOptions<ExternalUrlOptions>> _mockExternalUrlOptions;
        private Mock<IAccountServiceApiClient> _mockAccountServiceApiClient = null!;
        private Mock<IOptions<FrontEndAccountManagementOptions>> _mockFrontEndAccountManagementOptions = null!;
        private FrontEndAccountManagementOptions _frontendAccountManagementOptions = new FrontEndAccountManagementOptions()
        {
            BaseUrl = "https://localhost:7054/manage-account/reex"
        };

        [TestInitialize]
        public void Setup()
        {
            _mockOptions = new Mock<IOptions<HomeViewModel>>();
            _mockReprocessorService = new Mock<IReprocessorService>();
            _mockSessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
			_mockJourneySessionManager = new Mock<ISessionManager<JourneySession>>();
			_mockOrganisationAccessor = new Mock<IOrganisationAccessor>();
            _mockExternalUrlOptions = new Mock<IOptions<ExternalUrlOptions>>();
            _mockFrontEndAccountCreationOptions = new Mock<IOptions<FrontEndAccountCreationOptions>>();
            _mockAccountServiceApiClient = new Mock<IAccountServiceApiClient>();
            _mockFrontEndAccountManagementOptions = new Mock<IOptions<FrontEndAccountManagementOptions>>();

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

            var frontendAccountCreationOptions = new FrontEndAccountCreationOptions()
            {
                AddOrganisation = "AddOrganisaion",
                CreateUser = "CreateUser"
            };
            _mockFrontEndAccountCreationOptions.Setup(x => x.Value).Returns(frontendAccountCreationOptions);

            _mockFrontEndAccountManagementOptions.Setup(x => x.Value).Returns(_frontendAccountManagementOptions);

            var externalUrlsOptions = new ExternalUrlOptions()
            {
                ReadMoreAboutApprovedPerson = "govuk"
            };

            _mockExternalUrlOptions.Setup(x => x.Value).Returns(externalUrlsOptions);
            _mockOptions.Setup(x => x.Value).Returns(homeSettings);

            _controller = new HomeController(
                _mockOptions.Object,
                _mockReprocessorService.Object,
                _mockSessionManagerMock.Object,
                _mockJourneySessionManager.Object,
				_mockOrganisationAccessor.Object,
                _mockFrontEndAccountCreationOptions.Object,
                _mockFrontEndAccountManagementOptions.Object,
                _mockExternalUrlOptions.Object,
                 _mockAccountServiceApiClient.Object);

            //var claimsPrincipal = CreateClaimsPrincipal();

            //// Assign the fake user to controller context
            _mockHttpContext = new Mock<HttpContext>();

            //httpContext.Setup(c => c.User).Returns(claimsPrincipal);
            //_mockReprocessorService.Setup(o => o.Registrations).Returns(new Mock<IRegistrationService>().Object);
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
                            Address = new("Test Street", "Test Street 2", null, "Test Town", "County", "Country", "CV12TT")
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
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };

            // Expectations
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty))
                .ReturnsAsync(existingRegistration);
            _mockSessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
            session.Should().BeEquivalentTo(expectedSession);
        }

        [TestMethod]
		public async Task Index_redirects_to_ManageOrganisation_WhenOnlyOne_Organisation_Exists()
		{
			// Arrange
			var id = Guid.NewGuid();
			var userData = new UserDataBuilder().Build();
            userData.NumberOfOrganisations = userData.Organisations.Count;
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
                            Address = new("Test Street", "Test Street 2", null, "Test Town", "County", "Country", "CV12TT")
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
            };

            _controller.ControllerContext = new ControllerContext
			{
				HttpContext = _mockHttpContext.Object
			};

			// Expectations
			_mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
			_mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty))
                .ReturnsAsync(existingRegistration);
            _mockSessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.Index();

			// Assert
			var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
			redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
		}


		[TestMethod]
        public async Task Index_redirects_to_AddOrganisationIf_UserExistsButNo_Organisation()
        {
			// Expectations
			var userData = new UserDataBuilder().Build();
			userData.Organisations.RemoveAt(0);
			_mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));

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
                HttpContext = _mockHttpContext.Object
            };
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)MaterialItem.Plastic,
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
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());

			var journeySession = new JourneySession
			{
				UserData = userData,
				SelectedOrganisationId = organisationId
			};
			_mockJourneySessionManager.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
				.ReturnsAsync(journeySession);

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
            model.RegistrationData[0].Material.Should().Be((MaterialItem)(int)MaterialItem.Plastic);
            model.RegistrationData[0].ApplicationType.Should().Be(ApplicationType.Reprocessor);
            model.RegistrationData[0].SiteAddress.Should().Be("123 Test St, Test City");
            model.RegistrationData[0].RegistrationStatus.Should().Be(RegistrationStatus.InProgress);
            model.RegistrationData[0].Year.Should().Be(2024);
            model.AccreditationData[0].Material.Should().Be((MaterialItem)(int)MaterialItem.Plastic);
            model.AccreditationData[0].ApplicationType.Should().Be(ApplicationType.Reprocessor);
            model.AccreditationData[0].SiteAddress.Should().Be("123 Test St,Test City");
            model.AccreditationData[0].AccreditationStatus.Should().Be(Enums.AccreditationStatus.NotAccredited);
            model.AccreditationData[0].Year.Should().Be(2024);
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_With_No_Registration_AccreditationData()
        {
            // Arrange
            var organisationId = _userData.Organisations[0].Id;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
			};

            var registrationData = new List<RegistrationDto>
            {
            };

            var accreditationData = new List<RegistrationDto>
            {
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(_userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(_userData.Organisations);
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
                .ReturnsAsync(registrationData.ToList());

			var journeySession = new JourneySession
			{
				UserData = _userData,
				SelectedOrganisationId = organisationId
			};
			_mockJourneySessionManager.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
				.ReturnsAsync(journeySession);

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
                HttpContext = _mockHttpContext.Object
			};

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
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
                HttpContext = _mockHttpContext.Object
            };

			_mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
			// Act
			var result = await _controller.ManageOrganisation();

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirect.ActionName.Should().Be(nameof(HomeController.Index));

		}

		[TestMethod]
		public async Task ManageOrganisation_RedirectToSelectOrganisation_If_NoSelectedOrganisationInSession_When_MultipleOrganisation()
		{
			var userData = new UserDataBuilder().Build();
            userData.Organisations.Add(new Organisation
            {
                Id = Guid.NewGuid(),
                OrganisationNumber = "1234"
            });
            userData.NumberOfOrganisations = userData.Organisations.Count;

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = _mockHttpContext.Object
			};

			_mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
			// Act
			var result = await _controller.ManageOrganisation();

			var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;

			redirect.ActionName.Should().Be(nameof(HomeController.SelectOrganisation));
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

            Assert.IsInstanceOfType(viewResult.Model, typeof(AddOrganisationViewModel));

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
            _userData.Organisations = new List<Organisation>()
            {
                new () { Id = Guid.NewGuid(), OrganisationNumber = "1234" },
                new () { Id = Guid.NewGuid(), OrganisationNumber = "4321" }
            };
            _userData.NumberOfOrganisations = _userData.Organisations.Count;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };

			// Expectations
			_mockOrganisationAccessor.Setup(o => o.Organisations).Returns(_userData.Organisations);
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(_userData));
            _mockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty))
                .ReturnsAsync((RegistrationDto?)null);
            _mockJourneySessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync((JourneySession)null);

			// Act
			var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.SelectOrganisation));
        }

        [TestMethod]
        public async Task SelectOrganisation_ReturnsViewResultWithCorrectModel()
        {
            // Arrange
            _userData.Organisations = new List<Organisation>
            {
				new () { Id = Guid.NewGuid(), OrganisationNumber = "1234" },
				new () { Id = Guid.NewGuid(), OrganisationNumber = "4321" }
			};

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = _mockHttpContext.Object
			};


			// Expectations
			_mockOrganisationAccessor.Setup(o => o.Organisations).Returns(_userData.Organisations);
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(_userData));

            var journeySession = new JourneySession()
            {
                UserData = _userData,
                SelectedOrganisationId = _userData.Organisations[0].Id
            };
			_mockJourneySessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(journeySession);

			var result = await _controller.SelectOrganisation();

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.Model.Should().BeOfType<SelectOrganisationViewModel>();

            var model = viewResult.Model as SelectOrganisationViewModel;
            model.Organisations.Should().HaveCount(_userData.Organisations.Count);
            model.Organisations[0].Name.Should().Be(_userData.Organisations[0].Name);
            model.Organisations[0].OrganisationNumber.Should().Be(_userData.Organisations[0].OrganisationNumber);
        }

		[TestMethod]
		public async Task SelectOrganisation_InvalidModelState_ReturnsViewWithModel()
		{
			// Arrange
			_userData.Organisations = new List<Organisation>
			{
				new () { Id = Guid.NewGuid(), OrganisationNumber = "1234" },
				new () { Id = Guid.NewGuid(), OrganisationNumber = "4321" }
			};

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = _mockHttpContext.Object
			};
			_controller.ModelState.AddModelError("SelectedOrganisationId", "error");

			_mockOrganisationAccessor.Setup(o => o.Organisations).Returns(_userData.Organisations);
			_mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(_userData));

			var journeySession = new JourneySession()
			{
				UserData = _userData,
				SelectedOrganisationId = _userData.Organisations[0].Id
			};
			_mockJourneySessionManager.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(journeySession);

			var model = new SelectOrganisationViewModel();

			// Act
			var result = await _controller.SelectOrganisation(model);

			// Assert
			result.Should().BeOfType<ViewResult>();
			var viewResult = result as ViewResult;
			var returnedModel = viewResult!.Model as SelectOrganisationViewModel;

			Assert.AreEqual(_userData.Organisations.Count, returnedModel!.Organisations.Count);
            Assert.AreEqual(_userData.Organisations[0].Name, returnedModel.Organisations[0].Name);
		}

		[TestMethod]
		public async Task SelectOrganisation_ValidModelState_SavesSessionAndRedirects()
		{
			// Arrange
			var model = new SelectOrganisationViewModel
			{
				SelectedOrganisationId = Guid.NewGuid()
			};

			var journeySession = new JourneySession();
			_mockJourneySessionManager
				.Setup(m => m.GetSessionAsync(It.IsAny<ISession>()))
				.ReturnsAsync(journeySession);

			_mockJourneySessionManager
				.Setup(m => m.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<JourneySession>()))
				.Returns(Task.CompletedTask);
            
            _controller.ControllerContext = new ControllerContext
				{
					HttpContext = _mockHttpContext.Object
				};

			// Act
			var result = await _controller.SelectOrganisation(model);

			// Assert
			var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
			Assert.AreEqual("ManageOrganisation", redirect!.ActionName);
		}


		[TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_WithMultipleRegistrationAndAccreditationTypes()
        {
            // Arrange
            var userData = new UserDataBuilder().Build();
            var organisationId = userData.Organisations[0].Id;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
			};
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)MaterialItem.Plastic,
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
                    MaterialId = (int)MaterialItem.Glass,
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
                    MaterialId = (int)MaterialItem.Steel,
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
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
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
                HttpContext = _mockHttpContext.Object
			};
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)MaterialItem.Paper,
                    ApplicationTypeId = ApplicationType.Reprocessor,
                    ReprocessingSiteAddress = null, // Null address
                    RegistrationStatus = (int)RegistrationStatus.InProgress,
                    Year = 2024,
                    Id = Guid.NewGuid()
                }
            };
            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
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
                HttpContext = _mockHttpContext.Object
            };
            var registrationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)MaterialItem.Paper,
                    ApplicationTypeId = ApplicationType.Exporter,
                    ReprocessingSiteAddress = new AddressDto { AddressLine1 = "ExportAcc St", TownCity = "ExportAcc City" },
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.Submitted,
                    Year = 2027,
                    Id = Guid.NewGuid(),
                    ReprocessingSiteId = 5
                },
                new()
                {
                    MaterialId = (int)MaterialItem.Steel,
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
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
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

		[TestMethod]
		public async Task ManageOrganisation_ReturnsViewResult_WithMultipleTeamMembers()
		{
			// Arrange
            var userData = new UserDataBuilder().Build();
			var orgId = userData.Organisations[0].Id;

			var userGuid1 = Guid.NewGuid();
			var userGuid2 = Guid.NewGuid();

			var userModels = new List<UserModel>
			{
				new UserModel
				{
					FirstName = "John",
					LastName = "Doe",
					PersonId = userGuid1,
					ServiceRoleKey = "Approved Person"
				},
				new UserModel
				{
					FirstName = "Jane",
					LastName = "Smith",
					PersonId = userGuid2,
					ServiceRoleKey = "Administrator"
				}
			};

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = _mockHttpContext.Object
			};

			_mockAccountServiceApiClient.Setup(x =>
					x.GetUsersForOrganisationAsync(orgId.ToString(), userData.ServiceRoleId))
				.ReturnsAsync(userModels);

			_mockOrganisationAccessor.Setup(x => x.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
			_mockOrganisationAccessor.Setup(x => x.Organisations).Returns(userData.Organisations);

			_mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(orgId))
				.ReturnsAsync(new List<RegistrationDto>());

			// Act
			var result = await _controller.ManageOrganisation();

			// Assert
			var viewResult = result.Should().BeOfType<ViewResult>().Which;
			var model = viewResult.Model.Should().BeOfType<HomeViewModel>().Which;
			var url1 = new Uri($"{_frontendAccountManagementOptions.BaseUrl}/organisation/{orgId}/person/{model.TeamViewModel.TeamMembers[0].PersonId}", uriKind: UriKind.Absolute);
			var url2 = new Uri($"{_frontendAccountManagementOptions.BaseUrl}/organisation/{orgId}/person/{model.TeamViewModel.TeamMembers[1].PersonId}", uriKind: UriKind.Absolute);

			model.TeamViewModel.TeamMembers.Should().HaveCount(2);
			model.TeamViewModel.TeamMembers.Should().Contain(x => x.FullName == "John Doe" && x.RoleKey == "Approved Person");
			model.TeamViewModel.TeamMembers.Should().Contain(x => x.FullName == "Jane Smith" && x.RoleKey == "Administrator");
			model.TeamViewModel.TeamMembers[0].ViewDetails.Should().Be(url1);
			model.TeamViewModel.TeamMembers[1].ViewDetails.Should().Be(url2);
		}

		[TestMethod]
		public async Task ManageOrganisation_ReturnsMultipleUserServiceRoles_FromEnrolments()
		{
			// Arrange
			var userData = new UserDataBuilder().Build();
			var organisation = userData.Organisations[0];

			organisation.Enrolments = new List<EPR.Common.Authorization.Models.Enrolment>
			{
				new()
				{
					ServiceRole = "Approved Person",
					Service = "Exporter",
					EnrolmentId = 1,
					ServiceRoleId = 101
				},
				new()
				{
					ServiceRole = "Delegated Person",
					Service = "Reprocessor",
					EnrolmentId = 2,
					ServiceRoleId = 102
				}
			};

			var orgId = organisation.Id;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
			};

			_mockOrganisationAccessor.Setup(x => x.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(x => x.Organisations).Returns(userData.Organisations);

			_mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(orgId))
				.ReturnsAsync(new List<RegistrationDto>());

			_mockAccountServiceApiClient.Setup(x =>
					x.GetUsersForOrganisationAsync(orgId.ToString(), userData.ServiceRoleId))
				.ReturnsAsync(new List<UserModel>());

            _mockFrontEndAccountManagementOptions.Setup(x => x.Value).Returns(_frontendAccountManagementOptions);

			var journeySession = new JourneySession
			{
				UserData = userData
			};
			_mockJourneySessionManager.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
				.ReturnsAsync(journeySession);

			// Act
			var result = await _controller.ManageOrganisation();

			// Assert
			var viewResult = result.Should().BeOfType<ViewResult>().Which;
			var model = viewResult.Model.Should().BeOfType<HomeViewModel>().Which;

			model.TeamViewModel.UserServiceRoles.Should().Contain("Approved Person");
			model.TeamViewModel.UserServiceRoles.Should().Contain("Delegated Person");
			model.TeamViewModel.UserServiceRoles.Should().HaveCount(2);
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