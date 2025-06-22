namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Defines possible values for the status of the material registration.
/// </summary>
public enum MaterialStatus
{
    None = 0,
    Granted = 1,
    Refused = 2,
    Started = 3,
    Submitted = 4,
    RegulatorReviewing = 5,
    Queried = 6,

    // Note that it does jump from 6 to 8, not sure why but that is what the backend data is set as.
    // This could be a mistake in backend, but for now leave this as it is until we confirm.

    Withdrawn = 8,
    Suspended = 9,
    Cancelled = 10,
    ReadyToSubmit = 11
}