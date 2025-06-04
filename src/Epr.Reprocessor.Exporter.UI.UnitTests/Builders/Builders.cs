namespace Epr.Reprocessor.Exporter.UI.UnitTests.Builders;

/// <summary>
/// Defines static properties to access common builders.
/// </summary>
public static class Builders
{
    /// <summary>
    /// Creates a default <see cref="UserData"/> builder.
    /// </summary>
    public static UserDataBuilder NewUserData { get; set; } = new();

    /// <summary>
    /// Creates a default <see cref="EPR.Common.Authorization.Models.Organisation"/> builder.
    /// </summary>
    public static OrganisationBuilder NewOrganisation { get; set; } = new();
}