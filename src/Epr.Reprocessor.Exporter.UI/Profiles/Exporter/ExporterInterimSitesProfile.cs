using AutoMapper;
using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Profiles.Exporter;

public class ExporterInterimSitesProfile : Profile
{
    public ExporterInterimSitesProfile()
    {
        CreateMap<InterimSiteAddress, InterimSiteViewModel>()
            .ForMember(dest => dest.ContactFullName, opt => opt.MapFrom(src => src.InterimAddressContact.FirstOrDefault() != null ? src.InterimAddressContact.FirstOrDefault().FullName : string.Empty))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.InterimAddressContact.FirstOrDefault() != null ? src.InterimAddressContact.FirstOrDefault().Email : string.Empty))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.InterimAddressContact.FirstOrDefault() != null ? src.InterimAddressContact.FirstOrDefault().PhoneNumber : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.InterimAddressContact, opt => opt.MapFrom(src =>
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
