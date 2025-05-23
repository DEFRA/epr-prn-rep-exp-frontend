﻿using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.App.Options;

[ExcludeFromCodeCoverage]
public class HttpClientOptions
{
    public const string ConfigSection = "HttpClient";

    public int RetryCount { get; set; }

    public int RetryDelaySeconds { get; set; }

    public int TimeoutSeconds { get; set; }

    public string UserAgent { get; set; }

    public string AppServiceUrl { get; set; }
}