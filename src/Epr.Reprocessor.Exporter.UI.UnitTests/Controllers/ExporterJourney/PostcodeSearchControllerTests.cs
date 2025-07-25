﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;
using Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Moq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Controllers.ExporterJourney
{
    [TestClass]
    public class PostcodeSearchControllerTests
    {
        private Mock<ILogger<PostcodeSearchController>> _loggerMock;
        private Mock<ISaveAndContinueService> _saveAndContinueServiceMock;
        private Mock<ISessionManager<ExporterRegistrationSession>> _sessionManagerMock;
        private Mock<IValidationService> _validationServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IPostcodeLookupService> _postcodeLookupServiceMock;
        private readonly Mock<HttpContext> _httpContextMock = new();
        private readonly Mock<ISession> _session = new();

        private ExporterRegistrationSession _exporterSession;

        private const string CurrentPageViewLocation = "~/Views/ExporterJourney/PostcodeSearch/PostcodeSearch.cshtml";
        private const string SelectAddressForServiceOfNoticesView = "~/Views/ExporterJourney/PostcodeSearch/SelectAddressForServiceOfNotices.cshtml";
        private const string ConfirmNoticesAddressView = "~/Views/ExporterJourney/PostcodeSearch/ConfirmNoticesAddress.cshtml";
        private const string NoAddressFoundView = "~/Views/ExporterJourney/PostcodeSearch/NoAddressFound.cshtml";

        private PostcodeSearchController _controller;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<PostcodeSearchController>>();
            _saveAndContinueServiceMock = new Mock<ISaveAndContinueService>();
            _sessionManagerMock = new Mock<ISessionManager<ExporterRegistrationSession>>();
            _validationServiceMock = new Mock<IValidationService>();
            _mapperMock = new Mock<IMapper>();
            _postcodeLookupServiceMock = new Mock<IPostcodeLookupService>();
            _httpContextMock.Setup(x => x.Session).Returns(_session.Object);

            _exporterSession = new ExporterRegistrationSession
            {
                RegistrationId = Guid.NewGuid()
            };

            _sessionManagerMock.Setup(x => x.GetSessionAsync(It.IsAny<ISession>()))
                .Returns(Task.FromResult(_exporterSession));

            _sessionManagerMock.Setup(x => x.SaveSessionAsync(It.IsAny<ISession>(), It.IsAny<ExporterRegistrationSession>()))
                .Returns(Task.FromResult(true));

            var addressList = new AddressList { Addresses = new List<App.DTOs.AddressLookup.Address>() };
            for (int i = 1; i < 3; i++)
            {
                var address = new App.DTOs.AddressLookup.Address
                {
                    BuildingNumber = $"{i}",
                    Street = "Test Street",
                    County = "Test County",
                    Locality = "Test Locality",
                    Postcode = "T5 0ED",
                    Town = "Test Town"
                };

                addressList.Addresses.Add(address);
            }

            _postcodeLookupServiceMock
                .Setup(x => x.GetAddressListByPostcodeAsync(It.IsAny<string>()))
                .ReturnsAsync(addressList);

            _controller = new PostcodeSearchController(
                _loggerMock.Object,
                _saveAndContinueServiceMock.Object,
                _sessionManagerMock.Object,
                _mapperMock.Object,
                _postcodeLookupServiceMock.Object,
                _validationServiceMock.Object
            );
            _controller.ControllerContext.HttpContext = _httpContextMock.Object;
            var newLookupAddress = new LookupAddress("T5 0ED", addressList, 0);
            _exporterSession.RegistrationApplicationSession.ReprocessingSite.ServiceOfNotice.LookupAddress = newLookupAddress;
        }

        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Get_ReturnsViewWithModel()
        {
           // Act
            var result = await _controller.Get();
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be(CurrentPageViewLocation);
                viewResult.Model.Should().BeOfType<AddressSearchViewModel>();
            }
        }


        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new AddressSearchViewModel();
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
            {
                new()
                {
                     PropertyName = "Test",
                     ErrorMessage = "Test",
                }
            });

            _validationServiceMock.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ExporterPostcodeForServiceOfNotices(model);
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.Model.Should().Be(model);
            }
        }

        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_NoAddressFound_ReturnsViewWithModel()
        {
            var addressList = new AddressList ();          

            _postcodeLookupServiceMock
                .Setup(x => x.GetAddressListByPostcodeAsync(It.IsAny<string>()))
                .ReturnsAsync(addressList);
            // Arrange
            var model = new AddressSearchViewModel();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());          

            // Act
            var result = await _controller.ExporterPostcodeForServiceOfNotices(model);
            var viewResult = result as RedirectToActionResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ActionName.Should().Be("NoAddressFound");
            }
        }       


        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_SaveAndContinue_RedirectsCorrectly()
        {
            // Arrange
            var model = new AddressSearchViewModel();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());           

            // Act
            var result = await _controller.ExporterPostcodeForServiceOfNotices(model);
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.ExporterSelectAddressForServiceOfNotices);
            }
        }

        [TestMethod]
        public async Task PostcodeForServiceOfNotices_Post_SaveAndComeBackLater_RedirectsCorrectly()
        {
            // Arrange
            var model = new AddressSearchViewModel();
            _validationServiceMock.Setup(v => v.ValidateAsync(model, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());         

            // Act
            var result = await _controller.ExporterPostcodeForServiceOfNotices(model);
            var redirectResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                redirectResult.Should().NotBeNull();
                redirectResult.Url.Should().Be(PagePaths.ExporterSelectAddressForServiceOfNotices);
            }
        }


        [TestMethod]
        public async Task SelectAddressForReprocessingSite_Get_ReturnsViewWithModel()
        {   
            // Act
            var result = await _controller.ExporterSelectAddressForServiceOfNotices(0);
            var viewResult = result as RedirectResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.Url.Should().Be(PagePaths.ConfirmNoticesAddress);
            }
        }

        [TestMethod]
        public async Task SelectAddressForReprocessingSite_Get_NoSelectedReturnsViewWithModel()
        {
            // Act
            var result = await _controller.ExporterSelectAddressForServiceOfNotices();
            var viewResult = result as ViewResult;

            // Assert
            using (new AssertionScope())
            {
                viewResult.Should().NotBeNull();
                viewResult.ViewName.Should().Be(SelectAddressForServiceOfNoticesView);
                viewResult.Model.Should().BeOfType<Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.SelectAddressForServiceOfNoticesViewModel>();
            }
        }

        [TestMethod]
        public async Task ConfirmNoticesAddress_ReturnsExpectedViewResult()
        {
            // Act
            var result = await _controller.ConfirmNoticesAddress();
            var viewResult = result as ViewResult;
            // Assert
            using (new AssertionScope())
            {
                Assert.AreSame(typeof(ViewResult), result.GetType(), "Result should be of type ViewResult");
                viewResult!.Model.Should().BeOfType<ConfirmNoticesAddressViewModel>();
            }
        }

        [TestMethod]
        public async Task NoAddressFound_ShouldReturnViewWithModel()
        {
            var result = await _controller.NoAddressFound(AddressLookupType.ReprocessingSite) as ViewResult;
            var model = result!.Model as NoAddressFoundViewModel;

            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
        }

        [TestMethod]
        public async Task NoAddressFound_legalDocumentType_ShouldReturnViewWithModel()
        {
            var result = await _controller.NoAddressFound(AddressLookupType.LegalDocuments) as ViewResult;
            var model = result!.Model as NoAddressFoundViewModel;

            result.Should().BeOfType<ViewResult>();
            model.Should().NotBeNull();
        }
    }
}
