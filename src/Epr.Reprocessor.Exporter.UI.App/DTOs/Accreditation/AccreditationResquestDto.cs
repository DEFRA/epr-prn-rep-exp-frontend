using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class AccreditationResponseDto
{
    public int? InfrastructurePercentage { get; set; }
    public int? PackagingWastePercentage { get; set; }
    public int? BusinessCollectionsPercentage { get; set; }
    public int? CommunicationsPercentage { get; set; }
    public int? NewMarketsPercentage { get; set; }
    public int? NewUsesPercentage { get; set; }
}
