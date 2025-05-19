using System.Diagnostics.CodeAnalysis;
using EPR.Common.Authorization.Interfaces;
using EPR.Common.Authorization.Models;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Enums;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

[ExcludeFromCodeCoverage]
public class ReprocessorExporterRegistrationSession : IHasUserData
{
    public UserData UserData { get; set; } = new();
    
    public List<string> Journey { get; set; } = new();

    public int? RegistrationId { get; set; }

    public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new();

    //TODO: Check this session in RPD and confirm if we can base our session on it
    //public RegistrationApplicationSession RegistrationApplicationSession { get; set; } = new ();
}

/// <summary>
/// Represents a session for the registration application.
/// </summary>
public class RegistrationApplicationSession
{
    /// <summary>
    /// Details of the reprocessing site.
    /// </summary>
    public ReprocessingSite? ReprocessingSite { get; set; } = new();
}

/// <summary>
/// Represents the details of a reprocessing site.
/// </summary>
public class ReprocessingSite
{
    /// <summary>
    /// The address of the reprocessing site.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// The type of address provided.
    /// </summary>
    public AddressOptions? TypeOfAddress { get; set; }

    public ReprocessingSite SetReprocessingSite(Address? address, AddressOptions? typeOfAddress)
    {
        Address = address;
        TypeOfAddress = typeOfAddress;

        return this;
    }
}

/// <summary>
/// Represents an address object.
/// </summary>
/// <param name="AddressLine1">The first line of the address.</param>
/// <param name="AddressLine2">The second line of the address.</param>
/// <param name="Locality">The locality of the address.</param>
/// <param name="Town">The town of the address.</param>
/// <param name="County">The county of the address.</param>
/// <param name="Country">The country of the address.</param>
/// <param name="Postcode">The postcode of the address.</param>
/// <param name="Nation">The nation the address falls within.</param>
public record Address(
    string AddressLine1,
    string? AddressLine2,
    string? Locality,
    string Town,
    string? County,
    string? Country,
    string Postcode,
    UkNation Nation);