using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Attributes;
using Epr.Reprocessor.Exporter.UI.App.Resources.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

/// <summary>
/// Defines list of material items.
/// </summary>
public enum MaterialItem
{
    [MaterialLookup(IsVisible = false)]
    None = 0,

    [MaterialLookup]
    [Display(ResourceType = typeof(Materials), Name = "plastic_display_text")]
    Plastic = 1,

    [MaterialLookup]
    [Display(ResourceType = typeof(Materials), Name = "wood_display_text")]
    Wood = 2,

    [MaterialLookup]
    [Display(ResourceType = typeof(Materials), Name = "aluminium_display_text")]
    Aluminium = 3,

    [MaterialLookup]
    [Display(ResourceType = typeof(Materials), Name = "steel_display_text")]
    Steel = 4,

    [MaterialLookup(Value = "Paper/Board")]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    Paper = 5,

    [MaterialLookup]
    [Display(ResourceType = typeof(Materials), Name = "glass_display_text")]
    Glass = 6,

    [MaterialLookup(IsVisible = false)]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    GlassRemelt = 7,

    [MaterialLookup(IsVisible = false)]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    FibreComposite = 8
}