using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Services.ExporterJourney.Interfaces;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    public class BrokerLicenseController : BaseExporterController
    {
        IBrokerLicenseService _brokerLicenseService;

        public BrokerLicenseController(IBrokerLicenseService brokerLicenseService, IMapper mapper) : base(mapper) 
        {
            _brokerLicenseService = brokerLicenseService;
        }

        public IActionResult Get()
        {
            var id = Guid.NewGuid();
            var resultDto = _brokerLicenseService.GetDetails(id);
            var vm = Mapper.Map<BrokerLicenseViewModel>(resultDto);

            return View("BrokerLicense", vm);
        }
    }
}
