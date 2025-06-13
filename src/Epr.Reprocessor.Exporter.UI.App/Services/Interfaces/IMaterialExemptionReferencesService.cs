using System;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

public interface IMaterialExemptionReferencesService
{
    Task<bool> CreateMaterialExemptionReferences(List<MaterialExemptionReferenceDto> exemptions);
}
