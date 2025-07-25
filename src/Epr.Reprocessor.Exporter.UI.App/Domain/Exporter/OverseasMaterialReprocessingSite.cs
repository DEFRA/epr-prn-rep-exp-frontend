﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

[ExcludeFromCodeCoverage(Justification = "Fields will be used later in interim Journey")]
public class OverseasMaterialReprocessingSite
{
    public bool IsActive { get; set; }
    public Guid Id { get; init; }
    public Guid OverseasAddressId { get; init; }
    public required OverseasAddress OverseasAddress { get; init; }
    public List<InterimSiteAddress>? InterimSiteAddresses { get; set; } = new();
}

