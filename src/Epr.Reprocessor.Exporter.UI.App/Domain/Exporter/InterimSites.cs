using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage]
public class InterimSites
{
    public bool? HasInterimSites { get; set; }
    public List<OverseasMaterialReprocessingSite> OverseasMaterialReprocessingSites { get; set; } = new();
}