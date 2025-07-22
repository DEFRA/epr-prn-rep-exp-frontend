namespace Epr.Reprocessor.Exporter.UI.App.Constants
{
    public static class EprPrnFacadePaths
    {
        public const string SaveAndContinue = "api/saveandcontinue"; 
        public const string Accreditation = "api/v1.0/Accreditation";
        public const string AccreditationPrnIssueAuth = "api/v1.0/AccreditationPrnIssueAuth";
        public const string OverseasAccreditationSite = "api/v1/OverseasAccreditationSite"; 
        public const string AccreditationFileUploadGet = "api/v{0}/accreditation/{1}/Files/{2}/{3}";
        public const string AccreditationFileUploadPost = "api/v{0}/accreditation/{1}/Files";
        public const string AccreditationFileUploadDelete = "api/v{0}/accreditation/{1}/Files/{2}";
    }
}
