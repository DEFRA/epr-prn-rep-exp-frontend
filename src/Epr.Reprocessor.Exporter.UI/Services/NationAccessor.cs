namespace Epr.Reprocessor.Exporter.UI.Services;

/// <summary>
/// Implementation for <see cref="INationAccessor"/>.
/// </summary>
/// <param name="session">Provides access to the session manager,</param>
/// <param name="httpContextAccessor">Provides access to the HttpContext.</param>
public class NationAccessor(
    ISessionManager<ReprocessorRegistrationSession> session,
    IHttpContextAccessor httpContextAccessor)
    : INationAccessor
{
    private readonly ISessionManager<ReprocessorRegistrationSession> _session = session ?? throw new ArgumentNullException(nameof(session));
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    /// <inheritdoc />
    public async Task<UkNation?> GetNation()
    {
        var sessionData = await _session.GetSessionAsync(_httpContextAccessor.HttpContext!.Session);

        var reprocessingSite = sessionData?.RegistrationApplicationSession.ReprocessingSite;
        if (reprocessingSite is null)
        {
            return null;
        }

        var userAddressTypes = new List<AddressOptions> 
        { 
            AddressOptions.BusinessAddress, 
            AddressOptions.RegisteredAddress 
        };
        
        if (reprocessingSite.TypeOfAddress.HasValue && userAddressTypes.Contains(reprocessingSite.TypeOfAddress.Value))
        {
            return _httpContextAccessor.HttpContext.User.GetNationId();
        }

        return reprocessingSite.Nation ?? _httpContextAccessor.HttpContext.User.GetNationId();
    }
}