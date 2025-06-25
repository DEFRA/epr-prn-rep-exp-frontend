using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

public class ApplicationContactNameViewModel
{
    public string MaterialName { get; set; } = string.Empty;

    public Guid? CurrentUserId { get; set; }

    public List<KeyValuePair<Guid, string>> OtherContacts { get; set; } = new();

    [Required(ErrorMessage = "Please select an option")]
    public Guid? SelectedContact { get; set; }

    public void MapForView(Guid? currentUserId, RegistrationMaterialDto material, IEnumerable<OrganisationPerson> organisationUsers)
    {
        this.CurrentUserId = currentUserId;
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();

        // TODO: Add in the correct id here for the user/person
        this.OtherContacts = organisationUsers
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new KeyValuePair<Guid, string>(Guid.NewGuid(), string.Join(" ", u.FirstName, u.LastName)))
            .ToList();
    }
}