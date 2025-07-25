﻿namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents the address details for notices related to the reprocessing site/ notice.
/// </summary>
[ExcludeFromCodeCoverage]
public class Notice
{
    /// <summary>
    /// The address of the reprocessing site.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// The type of address provided.
    /// </summary>
    public AddressOptions? TypeOfAddress { get; set; }

    /// <summary>
    /// Sets the address for the notice.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="typeOfAddress"></param>
    /// <returns></returns>
    public Notice SetNoticeAddress(Address? address, AddressOptions? typeOfAddress)
    {
        Address = address;
        TypeOfAddress = typeOfAddress;

        return this;
    }
}
