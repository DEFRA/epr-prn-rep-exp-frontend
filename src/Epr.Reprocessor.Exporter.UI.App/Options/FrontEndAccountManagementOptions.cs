using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epr.Reprocessor.Exporter.UI.App.Options
{
    [ExcludeFromCodeCoverage]
    public class FrontEndAccountManagementOptions
    {
        public const string ConfigSection = "FrontEndAccountManagement";

        public string BaseUrl { get; set; }
    }
}
