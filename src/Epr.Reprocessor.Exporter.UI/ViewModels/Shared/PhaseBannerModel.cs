﻿using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

[ExcludeFromCodeCoverage]
public class PhaseBannerModel
{
    public string Status { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public bool ShowBanner { get; set; }
}