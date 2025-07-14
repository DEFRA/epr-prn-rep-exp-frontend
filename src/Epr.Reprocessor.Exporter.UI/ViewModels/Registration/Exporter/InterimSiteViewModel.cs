using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

public class InterimSiteViewModel
{
    public string OverseasSiteOrganisationName { get; set; }
    public string OverseasSiteAddressLine1 { get; set; }

    [BindNever, ValidateNever]
    public IEnumerable<string> Countries { get; set; }

    public string CountryName { get; set; }
    public string OrganisationName { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string CityOrTown { get; set; }
    public string? StateProvince { get; set; }
    public string? Postcode { get; set; }
    public string ContactFullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
