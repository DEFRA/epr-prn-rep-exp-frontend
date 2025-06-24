using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

public class MaterialsPermitTypeDto : IdNamePairDto
{
    public bool HasPermitNumber => ((MaterialPermitType)Id != MaterialPermitType.WasteExemption && (MaterialPermitType)Id != MaterialPermitType.None);
}
