using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage(Justification = "Fields will be used later in interim Journey")]
public class InterimSiteAddress : OverseasAddressBase
{
    public bool IsActive { get; set; }
    public Guid? Id { get; init; }
    public List<OverseasAddressContact> InterimAddressContact { get; set; } = new();
}

