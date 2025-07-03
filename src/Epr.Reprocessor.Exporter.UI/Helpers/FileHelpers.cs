using Epr.Reprocessor.Exporter.UI.Resources.Views.FileUpload;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;

namespace Epr.Reprocessor.Exporter.UI.Helpers;

public static class FileHelpers
{
    private const int OneMB = 1048576;
    private const string UploadFieldName = "File";
    private static readonly string[] AllowedExtensions = [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv", ".png", ".tif", ".tiff", ".jpg", ".jpeg", ".msg"];
    private static readonly FormOptions FormOptions = new();    

    public static async Task<byte[]?> ValidateUploadFileAndGetBytes(
        IFormFile file,
        ModelStateDictionary modelState,
        int fileUploadSizeinBytes)
    {
        if (file == null || file.Length == 0)
        {
            modelState.AddModelError(UploadFieldName, FileUpload.selected_file_is_empty);
            return null;
        }

        if (file.Length > fileUploadSizeinBytes)
        {
            var sizeLimit = fileUploadSizeinBytes / OneMB;
            modelState.AddModelError(UploadFieldName, string.Format(FileUpload.selected_file_must_be_smaller_than, sizeLimit));
            return null;
        }

        if (string.IsNullOrEmpty(file.FileName))
        {
            modelState.AddModelError(UploadFieldName, FileUpload.selected_filename_cannot_be_empty);
            return null;
        }

        if (!IsAllowedExtension(file.FileName))
        {
            modelState.AddModelError(UploadFieldName, FileUpload.selected_file_must_be_valid_file_type);
            return null;
        }       

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
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
        string? extension = GetFileExtension(fileName);
        return !string.IsNullOrWhiteSpace(extension) && AllowedExtensions.Contains(extension);
    }

    private static string? GetFileExtension(string fileName) => Path.GetExtension(fileName)?.ToLower();
}
