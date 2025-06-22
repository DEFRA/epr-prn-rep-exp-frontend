namespace Epr.Reprocessor.Exporter.UI.Mapper;

/// <summary>
/// Defines a contract for mapping requests to the backend services.
/// </summary>
public interface IRequestMapper
{
    /// <summary>
    /// Maps the objects to a <see cref="CreateRegistrationDto"/> for creating a new registration.
    /// </summary>
    /// <returns>A <see cref="CreateRegistrationDto"/> instance.</returns>
    Task<CreateRegistrationDto> MapForCreate();

    /// <summary>
    /// Maps the object to a <see cref="RegistrationDto"/> for updating an existing registration.
    /// </summary>
    /// <returns>A <see cref="RegistrationDto"/> instance.</returns>
    Task<UpdateRegistrationRequestDto> MapForUpdate();

    Task<List<AuthorisationTypes>> MapAuthorisationTypes(List<MaterialsPermitTypeDto> permitTypes,
           IStringLocalizer<SelectAuthorisationType> localizer,
           string? ukNation = null);
}