using AutoMapper;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Profiles
{
	public class ExportJourneyProfile : Profile
	{
		public ExportJourneyProfile()
		{
			CreateMap<OtherPermitsViewModel, OtherPermitsDto>();
		}
	}
}
