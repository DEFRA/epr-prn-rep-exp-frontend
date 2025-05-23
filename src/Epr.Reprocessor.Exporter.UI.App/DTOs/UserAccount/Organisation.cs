﻿using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.UserAccount;
 
public class Organisation
{
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string OrganisationName { get; set; }

    public string OrganisationRole { get; set; }

    public string OrganisationType { get; set; }
}