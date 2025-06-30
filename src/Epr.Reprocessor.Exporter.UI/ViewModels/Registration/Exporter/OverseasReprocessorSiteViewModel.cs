using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

public class OverseasReprocessorSiteViewModel
{
    [BindNever, ValidateNever]
    public bool IsFirstSite { get; set; }
    [BindNever, ValidateNever]
    public IEnumerable<string> Countries { get; set; }

    public string Country { get; set; }
    public string OrganisationName { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string CityorTown { get; set; }
    public string? StateProvince { get; set; }
    public string? Postcode { get; set; }
    public string SiteCoordinates { get; set; }
    public string ContactFullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
