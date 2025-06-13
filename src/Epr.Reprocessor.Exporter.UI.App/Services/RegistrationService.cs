using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Epr.Reprocessor.Exporter.UI.App.Services;

[ExcludeFromCodeCoverage]
public class RegistrationService(
    IEprFacadeServiceApiClient client,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    [ExcludeFromCodeCoverage(Justification = "TODO: Unit tests to be added as part of create registration user story")]
    public async Task<int> CreateRegistrationAsync(CreateRegistrationDto model)
    {
        try
        {
            var uri = Endpoints.CreateRegistration;

            var result = await client.SendPostRequest(uri, model);
            result.EnsureSuccessStatusCode();

            if (result.StatusCode == HttpStatusCode.NoContent)
                return default;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return await result.Content.ReadFromJsonAsync<int>(options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create registration");
            throw;
        }
    }

    [ExcludeFromCodeCoverage(Justification = " This method need to connect to facade once api is developed till that time UI to work with stub data and have no logic")]
    public Task<IEnumerable<RegistrationDto>> GetRegistrationAndAccreditationAsync(Guid? organisationId)
    {
        if (organisationId == Guid.Empty)
        {
            return Task.FromResult<IEnumerable<RegistrationDto>>(new List<RegistrationDto>());
        }

        var registrations = new List<RegistrationDto>()
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
                    AddressLine1 = "12 leylands Road",
                    AddressLine2 = "Downing street",
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
                ReprocessingSiteId = 1,
                RegistrationMaterialId = 1,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 1,
                    AddressLine1 = "12 Leylands Road",
                    AddressLine2 = "Downing Street",
                    TownCity = "Leeds"
                }
            },
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.Granted,
                AccreditationStatus = Enums.Accreditation.AccreditationStatus.Started,
                ApplicationType = "Reprocessor",
                Year = 2025,
                ApplicationTypeId = 1,
                MaterialId = 2,
                Material = "Glass",
                ReprocessingSiteId = 2,
                RegistrationMaterialId =2,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 2,
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
                ApplicationType = "Remanufacturer",
                Year = 2024,
                ApplicationTypeId = 2,
                MaterialId = 3,
                Material = "Plastic",
                ReprocessingSiteId = 3,
                RegistrationMaterialId =3,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 3,
                    AddressLine1 = "50 High Street",
                    AddressLine2 = "",
                    TownCity = "Manchester"
                }
            },
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.RegulatorReviewing,
                AccreditationStatus = Enums.Accreditation.AccreditationStatus.Accepted,
                ApplicationType = "Waste Manager",
                Year = 2024,
                ApplicationTypeId = 3,
                MaterialId = 4,
                Material = "Textile",
                ReprocessingSiteId = 4,
                RegistrationMaterialId =4,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 4,
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
                ReprocessingSiteId = 5,
                RegistrationMaterialId =5,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 5,
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
                ApplicationType = "Remanufacturer",
                Year = 2023,
                ApplicationTypeId = 2,
                MaterialId = 6,
                Material = "Paper",
                ReprocessingSiteId = 6,
                RegistrationMaterialId =6,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 6,
                    AddressLine1 = "10 Downing Road",
                    AddressLine2 = "",
                    TownCity = "London"
                }
            },
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.Refused,
                AccreditationStatus = Enums.Accreditation.AccreditationStatus.Refused,
                ApplicationType = "Waste Manager",
                Year = 2024,
                ApplicationTypeId = 3,
                MaterialId = 7,
                Material = "Wood",
                ReprocessingSiteId = 7,
                RegistrationMaterialId =7,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 7,
                    AddressLine1 = "Industrial Way",
                    AddressLine2 = "",
                    TownCity = "Birmingham"
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
                ReprocessingSiteId = 8,
                RegistrationMaterialId =8,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 8,
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
                ApplicationType = "Remanufacturer",
                Year = 2023,
                ApplicationTypeId = 2,
                MaterialId = 9,
                Material = "Chemicals",
                ReprocessingSiteId = 9,
                RegistrationMaterialId =9,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 9,
                    AddressLine1 = "Park Road",
                    AddressLine2 = "",
                    TownCity = "Liverpool"
                }
            },
            new()
            {
                RegistrationId = Guid.NewGuid(),
                RegistrationStatus = Enums.Registration.RegistrationStatus.Cancelled,
                AccreditationStatus = Enums.Accreditation.AccreditationStatus.Cancelled,
                ApplicationType = "Waste Manager",
                Year = 2024,
                ApplicationTypeId = 3,
                MaterialId = 10,
                Material = "Electronics",
                ReprocessingSiteId = 10,
                RegistrationMaterialId =10,
                ReprocessingSiteAddress = new DTOs.AddressDto() {
                    Id = 10,
                    AddressLine1 = "Newgate Street",
                    AddressLine2 = "Building C",
                    TownCity = "Edinburgh"
                }
            }
        };
        return Task.FromResult(registrations.AsEnumerable());
    }

    public async Task UpdateRegistrationSiteAddressAsync(int registrationId, UpdateRegistrationSiteAddressDto model)
    {
        try
        {
            var uri = Endpoints.UpdateRegistrationSiteAddress.Replace("{registrationId}", registrationId.ToString());

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

    public async Task UpdateRegistrationTaskStatusAsync(int registrationId, UpdateRegistrationTaskStatusDto model)
    {
        try
        {
            var uri = Endpoints.UpdateRegistrationTaskStatus.Replace("{registrationId}", registrationId.ToString());

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
