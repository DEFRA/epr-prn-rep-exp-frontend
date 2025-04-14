using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.Net.Http.Headers;

namespace Epr.Reprocessor.Exporter.UI.App.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpClientExtensions
{
    public static void AddHeaderAuthorization(this HttpClient httpClient, string token)
    {
        httpClient.AddDefaultRequestHeaderIfDoesNotContain(HeaderNames.Authorization, token);
    }

    public static void AddHeaderAcceptJson(this HttpClient httpClient)
    {
        httpClient.AddDefaultRequestHeaderIfDoesNotContain(HeaderNames.Accept, MediaTypeNames.Application.Json);
    }

    public static void AddHeaderUserAgent(this HttpClient httpClient, string userAgent)
    {
        httpClient.AddDefaultRequestHeaderIfDoesNotContain(HeaderNames.UserAgent, userAgent);
    }

    private static void AddDefaultRequestHeaderIfDoesNotContain(this HttpClient httpClient, string name, string value)
    {
        if (!httpClient.DefaultRequestHeaders.Contains(name))
        {
            httpClient.DefaultRequestHeaders.Add(name, value);
        }
    }
}