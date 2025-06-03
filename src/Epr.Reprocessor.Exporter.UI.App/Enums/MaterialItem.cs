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
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    Paper,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "glass_display_text")]
    Glass,

    [MaterialVisibility(IsVisible = false)]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    GlassRemelt,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "aluminium_display_text")]
    Aluminium,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "steel_display_text")]
    Steel,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "plastic_display_text")]
    Plastic,

    [MaterialVisibility]
    [Display(ResourceType = typeof(Materials), Name = "wood_display_text")]
    Wood,

    [MaterialVisibility(IsVisible = false)]
    [Display(ResourceType = typeof(Materials), Name = "paper_display_text")]
    FibreComposite
}