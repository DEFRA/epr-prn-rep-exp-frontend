using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class ApplicationContactNameViewModel
{
    public string MaterialName { get; set; } = string.Empty;

    public Guid? CurrentUserId { get; set; }

    public List<KeyValuePair<Guid, string>> OtherContacts { get; set; } = new();

    [Required(ErrorMessageResourceType = typeof(ApplicationContactName), ErrorMessageResourceName = "errorMessage")]
    public Guid? SelectedContact { get; set; }

    public void MapForView(Guid? currentUserId, RegistrationMaterialDto material, IEnumerable<OrganisationPerson> organisationUsers)
    {
        this.CurrentUserId = currentUserId;
        this.SelectedContact = material.RegistrationMaterialContact?.UserId;
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();

        this.OtherContacts = organisationUsers
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new KeyValuePair<Guid, string>(u.UserId, string.Join(" ", u.FirstName, u.LastName)))
            .ToList();
    }
}
