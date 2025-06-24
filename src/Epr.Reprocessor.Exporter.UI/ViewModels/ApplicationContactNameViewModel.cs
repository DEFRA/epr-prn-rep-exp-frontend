using Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

public class ApplicationContactNameViewModel
{
	public string? MaterialName { get; set; }

    public List<KeyValuePair<Guid, string>> ApplicationContacts { get; set; } = new();

    public Guid? SelectedContact { get; set; }

    public void MapForView(RegistrationMaterialDto material, IEnumerable<OrganisationPerson> organisationUsers)
    {
        this.MaterialName = material.MaterialLookup.Name.GetDisplayName();

        // TODO: Add in the correct id here for the user/person
        this.ApplicationContacts = organisationUsers
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new KeyValuePair<Guid, string>(Guid.NewGuid(), string.Join(" ", u.FirstName, u.LastName)))
            .ToList();
    }
}