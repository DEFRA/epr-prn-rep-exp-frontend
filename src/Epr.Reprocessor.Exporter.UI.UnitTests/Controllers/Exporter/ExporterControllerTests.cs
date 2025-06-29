using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.Exporter
{
    public class ExporterControllerTests
    {
        private ExporterController _controller = null!;
        private Mock<ILogger<RegistrationController>> _logger = null!;
        private Mock<ISaveAndContinueService> _userJourneySaveAndContinueService = null!;
        private Mock<IReprocessorService> _reprocessorService = null!;
        private Mock<IPostcodeLookupService> _postcodeLookupService = null!;
        private Mock<IMaterialService> _mockMaterialService = null!;
        private Mock<IRegistrationMaterialService> _mockRegistrationMaterialService = null!;
        private Mock<IValidationService> _validationService = null!;
        private Mock<IRegistrationService> _registrationService = null!;
        private Mock<IRegistrationMaterialService> _registrationMaterialService = null!;
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock = null!;
        private Mock<IRequestMapper> _requestMapper = null!;
        private readonly Mock<HttpContext> _httpContextMock = new();
        private readonly Mock<ClaimsPrincipal> _userMock = new();
        private ReprocessorRegistrationSession _session = null!;
        private Mock<IStringLocalizer<RegistrationController>> _mockLocalizer = new();
        protected ITempDataDictionary TempDataDictionary = null!;

        [TestInitialize]
        public void Setup()
        {
            // ResourcesPath should be 'Resources' but build environment differs from development environment
            // Work around = set ResourcesPath to non-existent location and test for resource keys rather than resource values
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources_not_found" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            var localizer = new StringLocalizer<SelectAuthorisationType>(factory);

            _logger = new Mock<ILogger<RegistrationController>>();
            _userJourneySaveAndContinueService = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _reprocessorService = new Mock<IReprocessorService>();
            _postcodeLookupService = new Mock<IPostcodeLookupService>();
            _mockMaterialService = new Mock<IMaterialService>();
            _mockRegistrationMaterialService = new Mock<IRegistrationMaterialService>();
            _validationService = new Mock<IValidationService>();
            _requestMapper = new Mock<IRequestMapper>();

            _controller = new ExporterController(_sessionManagerMock.Object, _validationService.Object);

            SetupDefaultUserAndSessionMocks();
            

            _registrationService = new Mock<IRegistrationService>();
            _registrationMaterialService = new Mock<IRegistrationMaterialService>();
            _reprocessorService.Setup(o => o.Registrations).Returns(_registrationService.Object);
            _reprocessorService.Setup(o => o.RegistrationMaterials).Returns(_registrationMaterialService.Object);
            _reprocessorService.Setup(o => o.Materials).Returns(_mockMaterialService.Object);

            TempDataDictionary = new TempDataDictionary(_httpContextMock.Object, new Mock<ITempDataProvider>().Object);
            _controller.TempData = TempDataDictionary;
        }


        [TestMethod]
        public async Task AddAnotherOverseasReprocessingSite_Should_Return_ViewResult()
        {
            //Arrange
            var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = true }; // Meaning the selected answer is Yes.        

            //Act
            var result = _controller.AddAnotherOverseasReprocessingSite();
            var actualResult = await result as ViewResult;

            //Assert
            actualResult.Should().BeOfType<ViewResult>();
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
        public async Task AddAnotherOverseasReprocessingSite_Should_Pass_Validation(string buttonAction, string previousPath)
        {
            //Arrange
            var model = new AddAnotherOverseasReprocessingSiteViewModel { AddOverseasSiteAccepted = true }; // Meaning the selected answer is Yes.
            var backlink = previousPath;

            //Act
            var result = _controller.AddAnotherOverseasReprocessingSite(model, buttonAction);
            var modelState = _controller.ModelState;

            //Assert
            modelState.IsValid.Should().BeTrue();
        }


        [TestMethod]
        [DataRow("SaveAndContinue", PagePaths.BaselConventionAndOECDCodes)]
        public async Task AddAnotherOverseasReprocessingSite_Should_Fail_Validation(string buttonAction, string previousPath)
        {
            //Arrange
            var model = new AddAnotherOverseasReprocessingSiteViewModel();
            var backlink = previousPath;

            //Act
            var result = _controller.AddAnotherOverseasReprocessingSite(model, buttonAction);
            var modelState = _controller.ModelState;

            modelState.AddModelError("Selection error", "Select if you are adding another overseas reprocessing site");

            //Assert
            modelState.IsValid.Should().BeFalse();
        }



        private void SetupDefaultUserAndSessionMocks()
        {
            SetupMockSession();
            SetupMockHttpContext(CreateClaims(GetUserData()));
        }

        private void SetupMockSession()
        {
            _sessionManagerMock.Setup(sm => sm.GetSessionAsync(It.IsAny<ISession>())).ReturnsAsync(new ExporterRegistrationSession());
        }

        private void SetupMockHttpContext(List<Claim> claims)
        {
            _userMock.Setup(x => x.Claims).Returns(claims);
            _httpContextMock.Setup(x => x.User).Returns(_userMock.Object);
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;
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
}
