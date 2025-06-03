using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Profiles.ExporterJourney
{
    public class ExporterJourneyMappings : Profile
    {
        public ExporterJourneyMappings()
        {
            CreateMap<BrokerLicenseViewModel, BrokerLicenseDto>()
                .ReverseMap();
        }
    }
}
