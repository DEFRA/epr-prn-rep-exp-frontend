﻿

using Epr.Reprocessor.Exporter.UI.ViewModels.Shared.GovUk;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    public static class ModelStateDictionaryExtension
    {
        public static Dictionary<string, List<ErrorViewModel>> ToErrorDictionary(this ModelStateDictionary modelState)
        {
            var errors = new List<ErrorViewModel>();

            foreach (var item in modelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    errors.Add(new ErrorViewModel
                    {
                        Key = item.Key,
                        Message = error.ErrorMessage
                    });
                }
            }

            var errorsDictionary = new Dictionary<string, List<ErrorViewModel>>();

            var groupedErrors = errors.GroupBy(e => e.Key);

            foreach (var error in groupedErrors)
            {
                errorsDictionary.Add(error.Key, error.ToList());
            }

            return errorsDictionary;
        }
    }
}
