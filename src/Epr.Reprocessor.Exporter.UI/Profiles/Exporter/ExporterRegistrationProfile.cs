using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Profiles.Exporter;

public class ExporterRegistrationProfile : Profile
{
    public ExporterRegistrationProfile()
    {
        CreateMap<OverseasAddress, OverseasReprocessorSiteViewModel>()
            .ForMember(dest => dest.ContactFullName, opt => opt.MapFrom(src => src.OverseasAddressContact.FirstOrDefault() != null ? src.OverseasAddressContact.FirstOrDefault().FullName : string.Empty))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.OverseasAddressContact.FirstOrDefault() != null ? src.OverseasAddressContact.FirstOrDefault().Email : string.Empty))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.OverseasAddressContact.FirstOrDefault() != null ? src.OverseasAddressContact.FirstOrDefault().PhoneNumber : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.OverseasAddressContact, opt => opt.MapFrom(src =>
                new List<OverseasAddressContact>
                {
                    new OverseasAddressContact
                    {
                        FullName = src.ContactFullName,
                        Email = src.Email,
                        PhoneNumber = src.PhoneNumber
                    }
                }
            ));
    }
}
