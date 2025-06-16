namespace Epr.Reprocessor.Exporter.UI.Mapper;

/// <summary>
/// Implementation for <see cref="IRequestMapper"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class RequestMapper : IRequestMapper
{
    private readonly ISessionManager<ReprocessorRegistrationSession> _sessionManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="sessionManager">Provides access to the session object.</param>
    /// <param name="httpContextAccessor">Provides access to the HttpContext.</param>
    /// <exception cref="ArgumentNullException">Throws if the HttpContext is null.</exception>
    public RequestMapper(ISessionManager<ReprocessorRegistrationSession> sessionManager, IHttpContextAccessor httpContextAccessor)
    {
        _sessionManager = sessionManager;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public async Task<CreateRegistrationDto> MapForCreate()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new ArgumentNullException();
        }

        var session = await _sessionManager.GetSessionAsync(_httpContextAccessor.HttpContext!.Session);
        var organisationId = _httpContextAccessor.HttpContext.User.GetOrganisationId();

        if (organisationId is null)
        {
            throw new InvalidOperationException("User is not authenticated or does not have an organisation ID.");
        }

        if (session is null)
        {
            throw new InvalidOperationException("Session cannot be null");
        }

        var request = new CreateRegistrationDto
        {
            OrganisationId = organisationId.Value,
            ApplicationTypeId = (int)ApplicationType.Reprocessor
        };

        if (session.RegistrationApplicationSession.ReprocessingSite?.Address is null)
        {
            throw new InvalidOperationException("Reprocessing site cannot be null in the session.");
        }

        request.ReprocessingSiteAddress = new AddressDto
        {
            AddressLine1 = session.RegistrationApplicationSession.ReprocessingSite.Address.AddressLine1,
            AddressLine2 = session.RegistrationApplicationSession.ReprocessingSite.Address.AddressLine2 ?? string.Empty,
            TownCity = session.RegistrationApplicationSession.ReprocessingSite.Address.Town,
            County = session.RegistrationApplicationSession.ReprocessingSite.Address.County ?? string.Empty,
            Country = session.RegistrationApplicationSession.ReprocessingSite.Address.Country ?? string.Empty,
            PostCode = session.RegistrationApplicationSession.ReprocessingSite.Address.Postcode,
            NationId = (int?)session.RegistrationApplicationSession.ReprocessingSite.Nation ?? 0,
            GridReference = session.RegistrationApplicationSession.ReprocessingSite.SiteGridReference
        };

        return request;
    }

    /// <inheritdoc />
    public async Task<UpdateRegistrationRequestDto> MapForUpdate()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            throw new ArgumentNullException();
        }

        var session = await _sessionManager.GetSessionAsync(_httpContextAccessor.HttpContext!.Session);
        var organisationId = _httpContextAccessor.HttpContext.User.GetOrganisationId();

        if (organisationId is null)
        {
            throw new InvalidOperationException("User is not authenticated or does not have an organisation ID.");
        }

        if (session is null)
        {
            throw new ArgumentNullException();
        }

        var request = new UpdateRegistrationRequestDto
        {
            RegistrationId = session.RegistrationId.GetValueOrDefault(),
            OrganisationId = organisationId.Value,
            ApplicationTypeId = ApplicationType.Reprocessor
        };

        var user = _httpContextAccessor.HttpContext.User.GetUserData();
        var organisation = user.Organisations.FirstOrDefault();

        var businessAddress = new AddressDto
        {
            AddressLine1 = $"{organisation!.BuildingNumber} {organisation.Street}",
            AddressLine2 = organisation.Locality ?? string.Empty,
            TownCity = organisation.Town ?? string.Empty,
            County = organisation.County ?? string.Empty,
            PostCode = organisation.Postcode ?? string.Empty,
            GridReference = string.Empty
        };

        request.BusinessAddress = businessAddress;

        if (session.RegistrationApplicationSession.ReprocessingSite?.Address is not null)
        {
            request.ReprocessingSiteAddress = new AddressDto
            {
                AddressLine1 = session.RegistrationApplicationSession.ReprocessingSite!.Address.AddressLine1,
                AddressLine2 = session.RegistrationApplicationSession.ReprocessingSite.Address.AddressLine2 ?? string.Empty,
                TownCity = session.RegistrationApplicationSession.ReprocessingSite.Address.Town,
                County = session.RegistrationApplicationSession.ReprocessingSite.Address.County,
                Country = session.RegistrationApplicationSession.ReprocessingSite.Address.Country,
                PostCode = session.RegistrationApplicationSession.ReprocessingSite.Address.Postcode,
                NationId = (int?)session.RegistrationApplicationSession.ReprocessingSite.Nation,
                GridReference = session.RegistrationApplicationSession.ReprocessingSite.SiteGridReference ?? string.Empty
            };
        }

        if (session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice?.Address is not null)
        {
            request.LegalAddress = new AddressDto
            {
                AddressLine1 = session.RegistrationApplicationSession.ReprocessingSite!.ServiceOfNotice!.Address.AddressLine1,
                AddressLine2 = session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice!.Address.AddressLine2 ?? string.Empty,
                TownCity = session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice!.Address!.Town!,
                County = session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice!.Address!.County,
                Country = session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice!.Address!.Country,
                PostCode = session.RegistrationApplicationSession.ReprocessingSite?.ServiceOfNotice!.Address!.Postcode!,
                GridReference = string.Empty
            };
        }

        return request;
    }
}