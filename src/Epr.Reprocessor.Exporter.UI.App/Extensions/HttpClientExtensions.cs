using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Epr.Reprocessor.Exporter.UI.App.Enums;
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

    public static void AddHeaderFileName(this HttpClient httpClient, string fileName)
    {
        httpClient.AddDefaultRequestHeaderIfDoesNotContain("FileName", fileName);
    }

    public static void AddHeaderSubmissionType(this HttpClient httpClient, SubmissionType submissionType)
    {
        httpClient.AddDefaultRequestHeaderIfDoesNotContain("SubmissionType", submissionType.ToString());
    }

    public static void AddHeaderSubmissionIdIfNotNull(this HttpClient httpClient, Guid? submissionId)
    {
        if (submissionId.HasValue)
        {
            httpClient.AddDefaultRequestHeaderIfDoesNotContain("SubmissionId", submissionId.Value.ToString());
        }
    }

    private static void AddDefaultRequestHeaderIfDoesNotContain(this HttpClient httpClient, string name, string value)
    {
        if (!httpClient.DefaultRequestHeaders.Contains(name))
        {
            httpClient.DefaultRequestHeaders.Add(name, value);
        }
    }
}