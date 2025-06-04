using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Builders;

/// <summary>
/// Creates a default <see cref="Organisation"/> using the builder.
/// </summary>
public class OrganisationBuilder : Builder<Organisation>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public OrganisationBuilder()
        : base(new())
    {
        Set(o => o.Id, Guid.Empty);
        Set(o => o.Name, "name");
        Set(o => o.OrganisationRole, "producer");
        Set(o => o.Town, "town");
        Set(o => o.BuildingName, "building name");
        Set(o => o.BuildingNumber, "building number");
        Set(o => o.Street, "street");
        Set(o => o.Locality, "locality");
        Set(o => o.County, "county");
        Set(o => o.Postcode, "postcode");
        Set(o => o.CompaniesHouseNumber, "companies house number");
        Set(o => o.OrganisationType, "organisation type");
        Set(o => o.Country, "country");
        Set(o => o.NationId, 1);
        Set(o => o.DependentLocality, "dependent locality");
        Set(o => o.JobTitle, "job title");
        Set(o => o.SubBuildingName, "sub building name");
    }
}