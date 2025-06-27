//using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;

//namespace Epr.Reprocessor.Exporter.UI.App.Services;

///// <summary>
///// Defines a contract to manage a registration.
///// </summary>
///// <remarks>If you need to manage registration materials then use the <see cref="RegistrationMaterialService"/> as this handles materials.</remarks>
///// <param name="client">The underlying http client that will call the facade.</param>
///// <param name="logger">The logger to log to.</param>
//[ExcludeFromCodeCoverage]
//public class ExporterRegistrationService(
//    IEprFacadeServiceApiClient client,
//    ILogger<ExporterRegistrationService> logger) : IExporterRegistrationService
//{
//    /// <inheritdoc/>
//    public async Task<CreateRegistrationResponseDto> CreateAsync(CreateRegistrationDto request)
//    {
//        try
//        {
//            var uri = Endpoints.Registration.CreateRegistration;

//            var result = await client.SendPostRequest(uri, request);

//            var options = new JsonSerializerOptions
//            {
//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
//            };

//            return (await result.Content.ReadFromJsonAsync<CreateRegistrationResponseDto>(options))!;
//        }
//        catch (HttpRequestException ex)
//        {
//            logger.LogError(ex, "Failed to create registration");
//            throw;
//        }
//    }
//}