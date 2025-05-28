namespace Epr.Reprocessor.Exporter.UI.App.DTOs.AddressLookup;

public class AddressLookupResponse
{
    public AddressLookupResponseHeader Header { get; set; } = default!;
    
    public AddressLookupResponseResult[] Results { get; set; } = default!;
}