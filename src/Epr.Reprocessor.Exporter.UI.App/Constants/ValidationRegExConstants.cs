

namespace Epr.Reprocessor.Exporter.UI.App.Constants
{
    public class ValidationRegExConstants
    {
        public const string GridReferenceNoSpecialCharacters = "^[a-zA-Z]+\\d{0,}";
        public const string GridReference10DigitMax = "^\\d{0,10}$";
        public const string GridReferenceLettersAndNumbers = "[a-zA-Z]+\\d";
        public const string GridReferenceNumbers = "\\d";
    }
}
