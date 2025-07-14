using Epr.Reprocessor.Exporter.UI.UnitTests.Builders;
using Epr.Reprocessor.Exporter.UI.ViewModels.Team;
using System.Diagnostics;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers
{
    //[Ignore("Need to come back to this as some funky threading issues are occuring.")]
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger = null!;
        private Mock<IOptions<HomeViewModel>> _mockOptions = null!;
        private Mock<IReprocessorService> _mockReprocessorService = null!;
        private Mock<ISessionManager<ReprocessorRegistrationSession>> _mockSessionManagerMock = null!;
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
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockOptions = new Mock<IOptions<HomeViewModel>>();
            _mockReprocessorService = new Mock<IReprocessorService>();
            _mockSessionManagerMock = new Mock<ISessionManager<ReprocessorRegistrationSession>>();
            _mockOrganisationAccessor = new Mock<IOrganisationAccessor>();
            _mockExternalUrlOptions = new Mock<IOptions<ExternalUrlOptions>>();
            _mockFrontEndAccountCreationOptions = new Mock<IOptions<FrontEndAccountCreationOptions>>();
            _mockAccountServiceApiClient = new Mock<IAccountServiceApiClient>();
            _mockFrontEndAccountManagementOptions = new Mock<IOptions<FrontEndAccountManagementOptions>>();

            var homeSettings = new HomeViewModel
            {
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications"
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

            _controller = new HomeController(_mockLogger.Object, _mockOptions.Object, _mockReprocessorService.Object,
                _mockSessionManagerMock.Object, _mockOrganisationAccessor.Object,
                _mockFrontEndAccountCreationOptions.Object,
                _mockFrontEndAccountManagementOptions.Object,
                _mockExternalUrlOptions.Object,
                 _mockAccountServiceApiClient.Object);

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
            _mockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty)).ReturnsAsync(existingRegistration);
            _mockSessionManagerMock.Setup(o => o.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(session);

            // Act
            var result = await _controller.Index();

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirect.ActionName.Should().Be(nameof(HomeController.ManageOrganisation));
            session.Should().BeEquivalentTo(expectedSession);
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
                    MaterialId = (int)MaterialItem.Plastic,
                    ApplicationTypeId = ApplicationType.Reprocessor,
                    ReprocessingSiteAddress = new AddressDto
                    {
                        AddressLine1 = "123 Test St",
                        TownCity = "Test City"
                    },
                    RegistrationStatus = (int)RegistrationStatus.InProgress,
                    Year = 2024
                }
            };

            var accreditationData = new List<RegistrationDto>
            {
                new()
                {
                    MaterialId = (int)MaterialItem.Plastic,
                    ApplicationTypeId = ApplicationType.Exporter,
                    ReprocessingSiteAddress = new AddressDto
                    {
                        AddressLine1 = "123 Test St",
                        TownCity = "Test City"
                    },
                    AccreditationStatus = (App.Enums.Accreditation.AccreditationStatus)Enums.AccreditationStatus.Started,
                    Year = 2024
                }
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId)).ReturnsAsync(registrationData.ToList());

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult!.Model, typeof(HomeViewModel));

            var model = viewResult.Model as HomeViewModel;
            model.Should().BeEquivalentTo(new HomeViewModel
            {
                FirstName = _userData.FirstName,
                LastName = _userData.LastName,
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications",
                OrganisationName = _userData.Organisations[0].Name!,
                OrganisationNumber = _userData.Organisations[0].OrganisationNumber!,
                RegistrationData = new List<RegistrationDataViewModel>
                {
                    new()
                    {
                        Material = (MaterialItem)(int)MaterialItem.Plastic,
                        ApplicationType = ApplicationType.Reprocessor,
                        SiteAddress = "123 Test St, Test City",
                        RegistrationStatus = RegistrationStatus.InProgress,
                        Year = 2024,
                    }
                },
                AccreditationData = new List<AccreditationDataViewModel>
                {
                    new()
                    {
                        Material = (MaterialItem)(int)MaterialItem.Plastic,
                        ApplicationType = ApplicationType.Reprocessor,
                        SiteAddress = "123 Test St,Test City",
                        AccreditationStatus = Enums.AccreditationStatus.NotAccredited,
                        Year = 2024,
                    }
                },
                TeamViewModel = new TeamViewModel
                {
                    OrganisationName = "name",
                    OrganisationExternalId = Guid.Empty,
                    TeamMembers = [],
                    AddNewUser = new Uri($"{_frontendAccountManagementOptions.BaseUrl}/organisation/{organisationId}", uriKind: UriKind.Absolute),
                }
            });
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

            var registrationData = new List<RegistrationDto>
            {
            };

            var accreditationData = new List<RegistrationDto>
            {
            };

            _mockOrganisationAccessor.Setup(o => o.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(o => o.Organisations).Returns(userData.Organisations);
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(organisationId))
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
            var userData = NewUserData.Build();
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
            _mockReprocessorService.Setup(o => o.Registrations.GetByOrganisationAsync(1, Guid.Empty))
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
            var userData = NewUserData.Build();
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
            model.Organisations.Should().HaveCount(_userData.Organisations.Count);
            model.Organisations[0].OrganisationName.Should().Be(_userData.Organisations[0].Name);
            model.Organisations[0].OrganisationNumber.Should().Be(_userData.Organisations[0].OrganisationNumber);
        }

        [TestMethod]
        public async Task ManageOrganisation_ReturnsViewResult_WithMultipleTeamMembers()
        {
            // Arrange
            var orgId = _userData.Organisations[0].Id;

            var personIdGuid1 = Guid.NewGuid();
            var personIdGuid2 = Guid.NewGuid();
            var connectionId1 = Guid.NewGuid();
            var connectionId2 = Guid.NewGuid();

            var userModels = new List<TeamMembersResponseModel>
            {
                new() {
                    FirstName = "Rex",
                    LastName = "DevTenThree",
                    Email = "ravi.sharma.rexdevthree@eviden.com",
                    PersonId = personIdGuid1,
                    ConnectionId = connectionId1,
                    Enrolments = new List<TeamMemberEnrolments>
                    {
                        new()
                        {
                            ServiceRoleId = 11,
                            EnrolmentStatusId = 1,
                            EnrolmentStatusName = "Enrolled",
                            ServiceRoleKey = "Re-Ex.AdminUser",
                            AddedBy = "Harish DevThree123"
                        },
                        new()
                        {
                            ServiceRoleId = 8,
                            EnrolmentStatusId = 1,
                            EnrolmentStatusName = "Enrolled",
                            ServiceRoleKey = "Re-Ex.ApprovedPerson",
                            AddedBy = "Rex DevTenThree"
                        }
                    }
                },
                new() {
                    FirstName = "Harish",
                    LastName = "DevThree",
                    Email = "Harish.DevThree@atos.net",
                    PersonId = personIdGuid2,
                    ConnectionId = connectionId2,
                    Enrolments = new List<TeamMemberEnrolments>
                    {
                        new()
                        {
                            ServiceRoleId = 12,
                            EnrolmentStatusId = 1,
                            EnrolmentStatusName = "Enrolled",
                            ServiceRoleKey = "Re-Ex.StandardUser",
                            AddedBy = "Harish DevTenThree"
                        }
                    }
                }
            };

            var userData = NewUserData.Build();
            userData.Organisations.Add(new Organisation() { OrganisationNumber = "1234" });
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            // Fix: Use `Returns` instead of `ReturnsAsync` for the mock setup
            _mockAccountServiceApiClient.Setup(x => x.GetTeamMembersForOrganisationAsync(orgId.ToString(), _userData.ServiceRoleId)).ReturnsAsync(userModels);
            _mockOrganisationAccessor.Setup(x => x.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(x => x.Organisations).Returns(_userData.Organisations);
            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(orgId)).ReturnsAsync(new List<RegistrationDto>());

            // Act
            var result = await _controller.ManageOrganisation();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<HomeViewModel>().Which;
            var url1 = new Uri($"{_frontendAccountManagementOptions.BaseUrl}/organisation/{orgId}/person/{personIdGuid1}", uriKind: UriKind.Absolute);
            var url2 = new Uri($"{_frontendAccountManagementOptions.BaseUrl}/organisation/{orgId}/person/{personIdGuid2}", uriKind: UriKind.Absolute);

            model.TeamViewModel.TeamMembers.Should().HaveCount(2);
            model.TeamViewModel.TeamMembers.Should().Contain(x => x.FirstName == "Rex" && x.LastName == "DevTenThree");
            model.TeamViewModel.TeamMembers.Should().Contain(x => x.FirstName == "Harish" && x.LastName == "DevThree");
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
                HttpContext = new DefaultHttpContext
                {
                    User = CreateClaimsPrincipal(userData)
                }
            };

            _mockOrganisationAccessor.Setup(x => x.OrganisationUser).Returns(CreateClaimsPrincipal(userData));
            _mockOrganisationAccessor.Setup(x => x.Organisations).Returns(userData.Organisations);

            _mockReprocessorService.Setup(x => x.Registrations.GetRegistrationAndAccreditationAsync(orgId))
                .ReturnsAsync(new List<RegistrationDto>());

            _mockAccountServiceApiClient.Setup(x =>
                    x.GetUsersForOrganisationAsync(orgId.ToString(), userData.ServiceRoleId))
                .ReturnsAsync(new List<UserModel>());

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