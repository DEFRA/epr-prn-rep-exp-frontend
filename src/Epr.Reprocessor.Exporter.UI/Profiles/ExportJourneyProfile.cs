using AutoMapper;
using Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney;
using Epr.Reprocessor.Exporter.UI.App.DTOs.ExporterJourney;
using ManualAddressForServiceOfNoticesViewModel = Epr.Reprocessor.Exporter.UI.ViewModels.ExporterJourney.ManualAddressForServiceOfNoticesViewModel;

namespace Epr.Reprocessor.Exporter.UI.Profiles
{
	[ExcludeFromCodeCoverage(Justification = "To be fixed after QA testing")]
	public class ExportJourneyProfile : Profile
	{
		public ExportJourneyProfile()
		{
			CreateMap<OtherPermitsViewModel, OtherPermitsDto>().ReverseMap();

			CreateMap<WasteCarrierBrokerDealerRefDto, WasteCarrierBrokerDealerRefViewModel>()
			.ForMember(dest => dest.CarrierBrokerDealerPermitId, opt => opt.MapFrom(src => src.CarrierBrokerDealerPermitId))
			.ForMember(dest => dest.RegistrationId, opt => opt.MapFrom(src => src.RegistrationId))
			.ForMember(dest => dest.WasteCarrierBrokerDealerRegistration, opt => opt.MapFrom(src => src.WasteCarrierBrokerDealerRegistration)).ReverseMap();

            CreateMap<AddressDto, ManualAddressForServiceOfNoticesViewModel>()
			.ForMember(dest => dest.TownOrCity, opt => opt.MapFrom(src => src.TownCity))
			.ForMember(dest => dest.Postcode, opt => opt.MapFrom(src => src.PostCode));

            CreateMap<ManualAddressForServiceOfNoticesViewModel, AddressDto>()
			.ForMember(dest => dest.TownCity, opt => opt.MapFrom(src => src.TownOrCity))
            .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.Postcode));

			CreateMap<Organisation, AddressViewModel>()
				.ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.BuildingName + " " + src.Street))
				.ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.Locality))
				.ForMember(dest => dest.TownOrCity, opt => opt.MapFrom(src => src.Town ?? string.Empty))
				.ForMember(dest => dest.County, opt => opt.MapFrom(src => src.County ?? string.Empty))
				.ForMember(dest => dest.Postcode, opt => opt.MapFrom(src => src.Postcode ?? string.Empty));

            CreateMap<ReprocessingSite, AddressViewModel>()
                .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.Address.AddressLine1 ?? string.Empty))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.Address.AddressLine2 ?? string.Empty))
                .ForMember(dest => dest.TownOrCity, opt => opt.MapFrom(src => src.Address.Town ?? string.Empty))
                .ForMember(dest => dest.County, opt => opt.MapFrom(src => src.Address.County ?? string.Empty))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(src => src.Address.Postcode ?? string.Empty));
			}
	}
}
