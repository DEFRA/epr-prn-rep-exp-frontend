using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Epr.Reprocessor.Exporter.UI.Helpers;

public static class FileHelpers
{
    private const int OneMB = 1048576;
    private const int OneKB = 1024;
    private const string UploadFieldName = "file";
    private static readonly string[] AllowedExtensions = [".pfd", ".doc", ".docx", ".xls", ".xlsx", ".csv", ".png", ".tif", ".tiff", ".jpg", ".jpeg", ".msg"];
    private static readonly FormOptions FormOptions = new();    

    public static async Task<byte[]?> ValidateUploadFileAndGetBytes(
        IFormFile file,
        ModelStateDictionary modelState,
        int fileUploadSizeinBytes)
    {
        if (file == null || file.Length == 0)
        {
            modelState.AddModelError(UploadFieldName, "The selected file is empty");
            return null;
        }

        if (file.Length > fileUploadSizeinBytes)
        {
            var sizeLimit = fileUploadSizeinBytes / OneMB;
            modelState.AddModelError(UploadFieldName, string.Format("The selected file must be smaller than {0}MB", sizeLimit));
            return null;
        }

        if (string.IsNullOrEmpty(file.FileName))
        {
            modelState.AddModelError(UploadFieldName, "The selected file name cannot be empty");
            return null;
        }

        if (!IsAllowedExtension(file.FileName))
        {
            modelState.AddModelError(UploadFieldName, "The selected file must be PDF, DOC, DOCX, XLS, XLSX, CSV, PNG, TIF, JPG or MSG");
            return null;
        }       

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public static async Task<(MultipartSection? Section, ContentDispositionHeaderValue? ContentDisposition)> ValidateUploadAsync(
        string? contentType,
        Stream fileStream,
        ModelStateDictionary modelState)
    {
        if (!MultipartRequestHelpers.IsMultipartContentType(contentType))
        {
            modelState.AddModelError(UploadFieldName, "Select a valid file");
            return (null, null);
        }

        var boundary = MultipartRequestHelpers.GetBoundary(contentType, FormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, fileStream);

        ContentDispositionHeaderValue? contentDisposition = null;
        MultipartSection section;

        while ((section = await reader.ReadNextSectionAsync()) != null)
        {
            var hasContentDispositionHeader =
                ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);
            if (!hasContentDispositionHeader)
            {
                modelState.AddModelError(UploadFieldName, "File upload is invalid - try again");
                return (null, null);
            }

            if (!MultipartRequestHelpers.HasFileContentDisposition(contentDisposition))
            {
                modelState.AddModelError(UploadFieldName, "Select a valid file");
                return (null, null);
            }

            var isFileSectionFound = !string.IsNullOrWhiteSpace(contentDisposition.FileName.Value);

            if (isFileSectionFound)
                break;
        }
        return (section, contentDisposition);
    }

    public static async Task<byte[]> ProcessFileAsync(
        MultipartSection section,
        string fileName,
        ModelStateDictionary modelState,
        string uploadFieldName,
        int fileUploadSizeinBytes)
    {
        using var memoryStream = new MemoryStream();
        await section.Body.CopyToAsync(memoryStream);

        if (string.IsNullOrEmpty(fileName))
        {
            modelState.AddModelError(uploadFieldName, "Select a valid file");
            return [];
        }

        if (!IsAllowedExtension(fileName))
        {
            modelState.AddModelError(uploadFieldName, "File type must be one of these: PDF, DOC, DOCX, XLS, CSV, PNG, TIF, JPG or MSG");
            return [];
        }

        // Reset the memory stream position to the beginning, just in case it's not at the start
        memoryStream.Position = 0;

        using (var reader = new StreamReader(memoryStream, leaveOpen: true)) // Ensure memoryStream is not disposed
        {
            // Read the first line to check if there is any content
            var firstLine = await reader.ReadLineAsync();

            if (memoryStream.Length == 0 || string.IsNullOrWhiteSpace(firstLine))
            {
                modelState.AddModelError(uploadFieldName, "The selected file is empty");
                return [];
            }
        }

        if (memoryStream.Length >= fileUploadSizeinBytes)
        {
            var fileUploadLimit = fileUploadSizeinBytes;

            int sizeLimit;
            if (fileUploadLimit >= OneMB)
                sizeLimit = fileUploadLimit / OneMB;
            else if (fileUploadLimit >= OneKB)
                sizeLimit = fileUploadLimit / OneKB;
            else
                sizeLimit = fileUploadLimit;

            modelState.AddModelError(uploadFieldName, string.Format("Each file must be no more than {0}MB", sizeLimit));

            return [];
        }

        return memoryStream.ToArray();
    }

    public static string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(fileName, out string contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }

    private static bool IsAllowedExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLower();
        return !string.IsNullOrWhiteSpace(extension) && AllowedExtensions.Contains(extension);
    }
}
