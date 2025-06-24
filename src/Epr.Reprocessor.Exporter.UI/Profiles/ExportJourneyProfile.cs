using AutoMapper;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;

namespace Epr.Reprocessor.Exporter.UI.Profiles
{
	public class ExportJourneyProfile : Profile
	{
		public ExportJourneyProfile()
		{
			CreateMap<OtherPermitsViewModel, OtherPermitsDto>().ReverseMap();

			CreateMap<WasteCarrierBrokerDealerRefDto, WasteCarrierBrokerDealerRefViewModel>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.RegistrationId, opt => opt.MapFrom(src => src.RegistrationId))
			.ForMember(dest => dest.WasteCarrierBrokerDealerRegistration, opt => opt.MapFrom(src => src.WasteCarrierBrokerDealerRegistration)).ReverseMap();
			}
	}
}
