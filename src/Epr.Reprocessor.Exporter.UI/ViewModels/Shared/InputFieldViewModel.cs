using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared
{
    [ExcludeFromCodeCoverage]
    public class InputFieldViewModel
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
        public string LabelText { get; set; }
        public string LabelDesc { get; set; }
        public bool HasError { get; set; }
        public bool IsDisabled { get; set; }
        public string ErrorMessage { get; set; }
    }
}
