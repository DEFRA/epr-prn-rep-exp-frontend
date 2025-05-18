using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

public class BaseController : Controller
{
    protected T GetStubDataFromTempData<T>(string key)
    {
        TempData.TryGetValue(key, out var tempData);
        if (tempData is not null)
        {
            TempData.Clear();
            return JsonConvert.DeserializeObject<T>(tempData.ToString());
        }

        return default(T);
    }
}
