namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class UploadEvidenceOfEquivalentStandardsViewModel
{
    private static string EU_Countries = "Austria, Belgium, Bulgaria, Croatia, Republic of Cyprus, Czech Republic, Denmark, Estonia, Finland, France, Germany, " +
            "Greece, Hungary, Ireland, Italy, Latvia, Lithuania, Luxembourg, Malta, Netherlands, Poland, Portugal, Romania, Slovakia, Slovenia, Spain, Sweden";

    private static string OECD_Countries = "Australia, Austria, Belgium, Canada, Chile, Columbia, Costa Rica, Czech Republic, Denmark, Estonia, Finland, France, Germany" +
            "Greece, Hungary, Iceland, Ireland, Israel, Italy, Japan, Korea, Latvia, Lithuania, Luxembourg, Mexico, Netherlands, New Zealand, Norway, Poland, Portugal," +
            "Slovakia, Slovenia, Spain, Sweden, Switzerland, Turkey, United Kingdom, United States";

    private static bool IsCountryInEU(string country) => EU_Countries.Contains(country);

    private static bool IsCountryInOECD(string country) => OECD_Countries.Contains(country);

    public bool IsSiteOutsideEU_OECD
    {
        get => OverseasSites.Exists(s => !IsCountryInEU(s.Country) && !IsCountryInOECD(s.Country));
    }

    public string MaterialName { get; set; } = string.Empty;

    public bool IsMetallicMaterial => MaterialName.ToLower() is "aluminium" or "steel";

    public List<OverseasReprocessingSite> OverseasSites { get; set; }
}
