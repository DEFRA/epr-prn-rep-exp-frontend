using Epr.Reprocessor.Exporter.UI.UnitTests.Builders;
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

            var homeSettings = new HomeViewModel
            {
                ApplyForRegistration = "/apply-for-registration",
                ViewApplications = "/view-applications"
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

            _controller = new HomeController(_mockLogger.Object, _mockOptions.Object, _mockReprocessorService.Object,
                _mockSessionManagerMock.Object, _mockOrganisationAccessor.Object,
                _mockFrontEndAccountCreationOptions.Object, _mockExternalUrlOptions.Object);

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
        public void ManageOrganisation_ReturnsViewResult()
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
            var result = _controller.ManageOrganisation();

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
                OrganisationNumber = _userData.Organisations[0].OrganisationNumber!
            });
        }

        [TestMethod]
        public void ManageOrganisation_RedirectToIndex_If_NoOrgInUserData()
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
            var result = _controller.ManageOrganisation();

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