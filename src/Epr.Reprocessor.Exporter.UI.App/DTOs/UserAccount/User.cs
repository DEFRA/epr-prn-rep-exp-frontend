﻿namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;

 
public class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string? RoleInOrganisation { get; set; }

    public string? EnrolmentStatus { get; set; }

    public string? ServiceRole { get; set; }

    public int? ServiceRoleId { get; set; }

    public string? Telephone { get; set; }

    public string? Service { get; set; }

    public int NumberOfOrganisations { get; set; }

    public List<Organisation>? Organisations { get; set; }
}