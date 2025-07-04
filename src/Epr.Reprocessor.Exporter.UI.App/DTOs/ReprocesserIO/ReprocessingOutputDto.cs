
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.ReprocesserIO;

public class ReprocessingOutputDto
{
    public Guid RegistrationReprocessingIOId { get; set; }
    public Guid RegistrationMaterialId { get; set; }
    public int SentToOtherSiteTonnes { get; set; }
    public int ContaminantTonnes { get; set; }
    public int ProcessLossTonnes { get; set; }
    public int TotalInputTonnes { get; set; }
    public List<ReprocessingIORawMaterialorProductDto> RawMaterialorProduct { get; set; }
    public int TotalOutputTonnes
    {
        get
        {
            int total = SentToOtherSiteTonnes + ContaminantTonnes + ProcessLossTonnes;

            if (RawMaterialorProduct != null && RawMaterialorProduct.Any())
            {
                total += (int)RawMaterialorProduct.Sum(rm => rm.ReprocessedTonnes);
            }

            return total;
        }
    }
}

