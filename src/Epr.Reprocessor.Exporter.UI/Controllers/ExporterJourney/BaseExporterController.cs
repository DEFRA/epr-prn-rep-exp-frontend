using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Epr.Reprocessor.Exporter.UI.Controllers.ExporterJourney
{
    public class BaseExporterController : Controller
    {
        // TODO:    All of the session setup and handling code should be placed in this base controller

        protected IMapper Mapper;

        public BaseExporterController(IMapper mapper)
        {
            Mapper = mapper;
        }

    }
}
