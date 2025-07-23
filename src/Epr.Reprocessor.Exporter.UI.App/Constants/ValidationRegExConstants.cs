

namespace Epr.Reprocessor.Exporter.UI.App.Constants
{
    public class ValidationRegExConstants
    {
        public const string GridReferenceNoSpecialCharacters = @"^(?=.*\d)([a-zA-Z\d]*[\W_]?)*(?<![\W_])$";
        public const string GridReference10DigitMax = @"^\d{0,10}$";
        public const string GridReferenceLettersAndNumbers = @"[a-zA-Z]+\d";
        public const string GridReferenceNumbers = @"\d";
        public const string GreaterThen0 = @"^[1-9][0-9]*$";

        public const string Postcode = "^([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})$";
        public const string ReferenceNumber = @"^[\s\S]*";
    }
}
