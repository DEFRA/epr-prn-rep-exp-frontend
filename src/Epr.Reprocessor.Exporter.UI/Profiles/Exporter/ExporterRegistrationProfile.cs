using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Profiles.Exporter;

public class ExporterRegistrationProfile : Profile
{
    public ExporterRegistrationProfile()
    {
        CreateMap<OverseasAddress, OverseasReprocessorSiteViewModel>()
            .ForMember(dest => dest.ContactFullName, opt => opt.MapFrom(src => GetContactDetail(src, c => c.FullName)))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => GetContactDetail(src, c => c.Email)))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => GetContactDetail(src, c => c.PhoneNumber)))
            .ReverseMap()
            .ForMember(dest => dest.OverseasAddressContacts, opt => opt.MapFrom(src =>
                new List<OverseasAddressContact>
                {
                    new OverseasAddressContact
                    {
                        FullName = src.ContactFullName,
                        Email = src.Email,
                        PhoneNumber = src.PhoneNumber
                    }
                }));

        CreateMap<ExporterRegistrationApplicationSession, OverseasAddressRequestDto>()
            .ForMember(dest => dest.RegistrationMaterialId, opt => opt.MapFrom(src => src.RegistrationMaterialId))
            .ForMember(dest => dest.OverseasAddresses, opt => opt.MapFrom(src => src.OverseasReprocessingSites != null ? src.OverseasReprocessingSites.OverseasAddresses : null));

        CreateMap<OverseasAddress, OverseasAddressDto>()
            .ForMember(dest => dest.OverseasAddressContacts, opt => opt.MapFrom(src => src.OverseasAddressContacts))
            .ForMember(dest => dest.OverseasAddressWasteCodes, opt => opt.MapFrom(src => src.OverseasAddressWasteCodes));

        CreateMap<CheckOverseasReprocessingSitesAnswersViewModel, OverseasAddressRequestDto>()
            .ForMember(dest => dest.RegistrationMaterialId, opt => opt.MapFrom(src => src.RegistrationMaterialId))
            .ForMember(dest => dest.OverseasAddresses, opt => opt.MapFrom(src => src.OverseasAddresses));

        CreateMap<OverseasMaterialReprocessingSite, OverseasMaterialReprocessingSiteDto>().ReverseMap();
        CreateMap<OverseasAddressBase, OverseasAddressBaseDto>().IncludeAllDerived().ReverseMap();
        CreateMap<OverseasAddress, OverseasAddressBaseDto>().ReverseMap();
        CreateMap<InterimSiteAddress, InterimSiteAddressDto>().ReverseMap();
        CreateMap<OverseasAddressContactDto, OverseasAddressContact>().ReverseMap();

        CreateMap<ExporterRegistrationApplicationSession, SaveInterimSitesRequestDto>()
            .ForMember(dest => dest.RegistrationMaterialId, opt => opt.MapFrom(src => src.RegistrationMaterialId.HasValue ? src.RegistrationMaterialId.Value : Guid.Empty))
            .ForMember(dest => dest.OverseasMaterialReprocessingSites, opt => opt.MapFrom(src => src.InterimSites != null ? src.InterimSites.OverseasMaterialReprocessingSites : new List<OverseasMaterialReprocessingSite>()));

        CreateMap<OverseasAddressDto, OverseasAddress>();
        CreateMap<OverseasAddressContactDto, OverseasAddressContact>();
        CreateMap<OverseasAddressWasteCodesDto, OverseasAddressWasteCodes>();

    }

    private static string GetContactDetail(OverseasAddress src, Func<OverseasAddressContact, string> selector)
    {
        return src.OverseasAddressContacts?.FirstOrDefault() is { } contact ? selector(contact) : string.Empty;
    }
}