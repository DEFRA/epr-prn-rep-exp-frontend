

namespace Epr.Reprocessor.Exporter.UI.App.Constants
{
    public class ValidationRegExConstants
    {
        public const string GridReferenceNoSpecialCharacters = @"^(?=.*\d)([a-zA-Z\d]*[\W_]?)*(?<![\W_])$";
        public const string GridReference10DigitMax = @"^\d{0,10}$";
        public const string GridReferenceLettersAndNumbers = @"[a-zA-Z]+\d";
        public const string GridReferenceNumbers = @"\d";
        public const string GreaterThen0 = @"^[1-9][0-9]*$";
    }
}
