using Epr.Reprocessor.Exporter.UI.App.Enums.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

/// <summary>
/// Defines a contract to manage a registration.
/// </summary>
/// <remarks>If you need to manage registration materials then use the <see cref="RegistrationMaterialService"/> as this handles materials.</remarks>
/// <param name="client">The underlying http client that will call the facade.</param>
/// <param name="logger">The logger to log to.</param>
[ExcludeFromCodeCoverage]
public class RegistrationService(
    IEprFacadeServiceApiClient client,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    /// <inheritdoc/>
    public async Task<CreateRegistrationResponseDto> CreateAsync(CreateRegistrationDto request)
    {
        try
        {
            var uri = Endpoints.Registration.CreateRegistration;

            var result = await client.SendPostRequest(uri, request);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return (await result.Content.ReadFromJsonAsync<CreateRegistrationResponseDto>(options))!;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to create registration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<RegistrationDto?> GetAsync(Guid registrationId)
    {
        try
        {
            var result = await client.SendGetRequest(string.Format(Endpoints.Registration.GetRegistration, registrationId.ToString()));
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<RegistrationDto>(options);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get registration");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<RegistrationDto?> GetByOrganisationAsync(int applicationTypeId, Guid organisationId)
    {
        try
        {
            var result = await client.SendGetRequest(string .Format(Endpoints.Registration.GetByOrganisation, applicationTypeId, organisationId));
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<RegistrationDto>(options);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get registration by organisation");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Guid registrationId, UpdateRegistrationRequestDto request)
    {
        try
        {
            var result = await client.SendPostRequest(string.Format(Endpoints.Registration.UpdateRegistration, registrationId), request);
            result.EnsureSuccessStatusCode();

        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not get registration");
            throw;
        }
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = " This method need to connect to facade once api is developed till that time UI to work with stub data and have no logic")]
    public Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid? organisationId)
    {
        if (organisationId == Guid.Empty)
        {
            return Task.FromResult<IEnumerable<RegistrationDto>>(new List<RegistrationDto>());
        }

            {
                new(){
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.InProgress,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.NotAccredited,
                    ApplicationType = "Reprocessor",
                    Year = 2025,
                    ApplicationTypeId = 1,
                    MaterialId = 1,
                    Material = "Steel",
                    ReprocessingSiteId = 1,
                    RegistrationMaterialId = 1,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 1,
                        AddressLine1 = "12 Leylands Road",
                        AddressLine2 = "Downing Street",
                        TownCity = "Leeds"
                    }
                },
                new(){
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.InProgress,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.NotAccredited,
                    ApplicationType = "Reprocessor",
                    Year = 2025,
                    ApplicationTypeId = 1,
                    MaterialId = 1,
                    Material = "Steel",
                    ReprocessingSiteId = 2, // Changed to a unique ID
                    RegistrationMaterialId = 2, // Changed to a unique ID
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 2, // Changed to a unique ID
                        AddressLine1 = "25 Oak Avenue", // Changed for uniqueness
                        AddressLine2 = "Maple Lane",
                        TownCity = "Manchester"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Granted,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Started,
                    ApplicationType = "Exporter", // Changed to Exporter
                    Year = 2025,
                    ApplicationTypeId = 4, // Assuming a new ID for Exporter
                    MaterialId = 2,
                    Material = "Glass",
                    ReprocessingSiteId = 3, // Changed to a unique ID
                    RegistrationMaterialId =3,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 3, // Changed to a unique ID
                        AddressLine1 = "1 Lees Road",
                        AddressLine2 = "Paragon Street",
                        TownCity = "Uxbridge"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Submitted,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Submitted,
                    ApplicationType = "Reprocessor", // Changed to Reprocessor
                    Year = 2024,
                    ApplicationTypeId = 1,
                    MaterialId = 3,
                    Material = "Plastic",
                    ReprocessingSiteId = 4, // Changed to a unique ID
                    RegistrationMaterialId =4,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 4, // Changed to a unique ID
                        AddressLine1 = "50 High Street",
                        AddressLine2 = "City Centre", // Added for uniqueness
                        TownCity = "Birmingham" // Changed for uniqueness
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.RegulatorReviewing,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Accepted,
                    ApplicationType = "Exporter", // Changed to Exporter
                    Year = 2024,
                    ApplicationTypeId = 4, // Assuming a new ID for Exporter
                    MaterialId = 4,
                    Material = "Textile",
                    ReprocessingSiteId = 5, // Changed to a unique ID
                    RegistrationMaterialId =5,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 5, // Changed to a unique ID
                        AddressLine1 = "The Green",
                        AddressLine2 = "Industrial Estate",
                        TownCity = "Bristol"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Queried,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Queried,
                    ApplicationType = "Reprocessor",
                    Year = 2025,
                    ApplicationTypeId = 1,
                    MaterialId = 5,
                    Material = "Aluminium",
                    ReprocessingSiteId = 6, // Changed to a unique ID
                    RegistrationMaterialId =6,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 6, // Changed to a unique ID
                        AddressLine1 = "Unit 7",
                        AddressLine2 = "Riverside Park",
                        TownCity = "Glasgow"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Updated,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Updated,
                    ApplicationType = "Exporter", // Changed to Exporter
                    Year = 2023,
                    ApplicationTypeId = 4, // Assuming a new ID for Exporter
                    MaterialId = 6,
                    Material = "Paper",
                    ReprocessingSiteId = 7, // Changed to a unique ID
                    RegistrationMaterialId =7,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 7, // Changed to a unique ID
                        AddressLine1 = "10 Downing Road",
                        AddressLine2 = "Whitehall", // Added for uniqueness
                        TownCity = "London"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Refused,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Refused,
                    ApplicationType = "Reprocessor", // Changed to Reprocessor
                    Year = 2024,
                    ApplicationTypeId = 1,
                    MaterialId = 7,
                    Material = "Wood",
                    ReprocessingSiteId = 8, // Changed to a unique ID
                    RegistrationMaterialId =8,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 8, // Changed to a unique ID
                        AddressLine1 = "Industrial Way",
                        AddressLine2 = "Factory Lane", // Added for uniqueness
                        TownCity = "Newcastle" // Changed for uniqueness
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.RenewalInProgress,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Granted,
                    ApplicationType = "Reprocessor",
                    Year = 2025,
                    ApplicationTypeId = 1,
                    MaterialId = 8,
                    Material = "Tyres",
                    ReprocessingSiteId = 9, // Changed to a unique ID
                    RegistrationMaterialId =9,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 9, // Changed to a unique ID
                        AddressLine1 = "Grove Lane",
                        AddressLine2 = "Unit 1",
                        TownCity = "Sheffield"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Suspended,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Suspended,
                    ApplicationType = "Exporter", // Changed to Exporter
                    Year = 2023,
                    ApplicationTypeId = 4, // Assuming a new ID for Exporter
                    MaterialId = 9,
                    Material = "Chemicals",
                    ReprocessingSiteId = 10, // Changed to a unique ID
                    RegistrationMaterialId =10,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 10, // Changed to a unique ID
                        AddressLine1 = "Park Road",
                        AddressLine2 = "Science Park", // Added for uniqueness
                        TownCity = "Liverpool"
                    }
                },
                new()
                {
                    RegistrationId = Guid.NewGuid(),
                    RegistrationStatus = Enums.Registration.RegistrationStatus.Cancelled,
                    AccreditationStatus = Enums.Accreditation.AccreditationStatus.Cancelled,
                    ApplicationType = "Reprocessor", // Changed to Reprocessor
                    Year = 2024,
                    ApplicationTypeId = 1,
                    MaterialId = 10,
                    Material = "Electronics",
                    ReprocessingSiteId = 11, // Changed to a unique ID
                    RegistrationMaterialId =11,
                    ReprocessingSiteAddress = new DTOs.AddressDto() {
                        Id = 11, // Changed to a unique ID
                        AddressLine1 = "Newgate Street",
                        AddressLine2 = "Building C",
                        TownCity = "Edinburgh"
                    }
                }
            };
        return Task.FromResult(registrations.AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task UpdateRegistrationSiteAddressAsync(Guid registrationId, UpdateRegistrationSiteAddressDto model)
    {
        try
        {
            var uri = Endpoints.Registration.UpdateRegistrationSiteAddress.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendPostRequest(uri, model);
            if (result.StatusCode is HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Registration not found");
            }

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update registration site address and contact details - registrationId: {RegistrationId}", registrationId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateRegistrationTaskStatusAsync(Guid registrationId, UpdateRegistrationTaskStatusDto model)
    {
        try
        {
            var uri = Endpoints.Registration.UpdateRegistrationTaskStatus.Replace("{registrationId}", registrationId.ToString());

            var result = await client.SendPostRequest(uri, model);
            if (result.StatusCode is HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Registration not found");
            }

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update registration task status - registrationId: {RegistrationId}", registrationId);
            throw;
        }
    }
}