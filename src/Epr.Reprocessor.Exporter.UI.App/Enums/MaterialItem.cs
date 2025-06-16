using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Attributes;
using Epr.Reprocessor.Exporter.UI.App.Resources.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

/// <summary>
/// Defines list of material items.
/// </summary>
public enum MaterialItem
{
    [MaterialVisibility(IsVisible = false)]
    None = 0,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "plastic_display_text")]
    Plastic = 1,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "wood_display_text")]
    Wood = 2,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "aluminium_display_text")]
    Aluminium = 3,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "steel_display_text")]
    Steel = 4,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    Paper = 5,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "glass_display_text")]
    Glass = 6,

    [MaterialVisibility(IsVisible = false)]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    GlassRemelt = 7,
          
    [MaterialVisibility(IsVisible = false)]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    FibreComposite = 8
}