using AutoMapper;

namespace Epr.Reprocessor.Exporter.UI.Profiles;

public class RegistrationProfile : Profile
{
    public RegistrationProfile()
    {
        // AddressViewModel => AddressDto
        CreateMap<AddressViewModel, AddressDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GridReference, opt => opt.Ignore())
            .ForMember(dest => dest.NationId, opt => opt.Ignore())
            .ForMember(dest => dest.Country, opt => opt.Ignore())
            .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.Postcode))
            .ForMember(dest => dest.TownCity, opt => opt.MapFrom(src => src.TownOrCity));

        // CheckAnswersViewModel => UpdateRegistrationSiteAddressDto
        CreateMap<CheckAnswersViewModel, UpdateRegistrationSiteAddressDto>()
            .ForMember(dest => dest.ReprocessingSiteAddress, opt => opt.MapFrom(src => src.ReprocessingSiteAddress));
    }
}