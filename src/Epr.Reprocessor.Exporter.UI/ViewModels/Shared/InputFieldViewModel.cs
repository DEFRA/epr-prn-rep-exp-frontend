using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared
{
    [ExcludeFromCodeCoverage]
    public class InputFieldViewModel
    {
        public string FieldName { get; set; }
        public decimal Value { get; set; }
        public string LabelText { get; set; }
        public string LabelDesc { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
