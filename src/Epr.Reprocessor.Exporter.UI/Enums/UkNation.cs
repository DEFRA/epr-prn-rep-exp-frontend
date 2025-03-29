using Epr.Reprocessor.Exporter.UI.Resources.Views.Enums;
using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.Enums
{
    public enum UkNation
    {
        None = 0,
        [Display(Name = "England", ResourceType = typeof(UkNationResource))]
        England,

        [Display(Name = "NorthernIreland", ResourceType = typeof(UkNationResource))]
        NorthernIreland,

        [Display(Name = "Scotland", ResourceType = typeof(UkNationResource))]
        Scotland,

        [Display(Name = "Wales", ResourceType = typeof(UkNationResource))]
        Wales
    }
}
