using Organisation = EPR.Common.Authorization.Models.Organisation;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Builders;

/// <summary>
/// Creates a default <see cref="UserData"/> using the builder.
/// </summary>
public class UserDataBuilder : Builder<UserData>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public UserDataBuilder() 
        : base(new())
    {
        Set(o => o.FirstName, "first");
        Set(o => o.LastName, "last");
        Set(o => o.Email, "email");
        Set(o => o.EnrolmentStatus, "enrolled");
        Set(o => o.RoleInOrganisation, "admin");
        Set(o => o.Service, "service");
        Set(o => o.ServiceRole, "role");
        Set(o => o.ServiceRoleId, 1);
        Set(o => o.Id, Guid.Empty);

        var organisation = new OrganisationBuilder();

        Set(o => o.Organisations, new List<Organisation> { organisation.Build() });
    }
}