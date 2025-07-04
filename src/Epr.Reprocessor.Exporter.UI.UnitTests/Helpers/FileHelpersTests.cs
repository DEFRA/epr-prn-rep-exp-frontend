using Epr.Reprocessor.Exporter.UI.Helpers;
using System.Globalization;

namespace Epr.Reprocessor.Exporter.UI.UnitTests.Helpers;

[TestClass]
public class FileHelpersTests
{
    private const string UploadFileName = "File";
    private ModelStateDictionary _modelStateDictionary;

    [TestInitialize]
    public void Setup()
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
        _modelStateDictionary = new ModelStateDictionary();
    }

    [TestMethod]
    public async Task ValidateUploadFileAndGetBytes_Invalid_ModalState_When_The_File_IsNull()
    {
        // Arrange
        var fileUploadSizeInBytes = 100;
        IFormFile formFile = null;
        
        // Act
        var result = await FileHelpers.ValidateUploadFileAndGetBytes(formFile, _modelStateDictionary, fileUploadSizeInBytes);

        // Assert
        result.Should().BeNull();
        var modalErrors = GetModelStateErrors();
        modalErrors.Should().HaveCount(1).And.Contain("The selected file is empty");
    }

    [TestMethod]
    public async Task ValidateUploadFileAndGetBytes_Invalid_ModalState_When_The_File_IsEmpty()
    {
        var fileUploadSizeInBytes = 1024;
        var fileName = "file.csv";

        IFormFile formFile = CreateFormFile(fileName, 0);

        // Act
        var result = await FileHelpers.ValidateUploadFileAndGetBytes(formFile, _modelStateDictionary, fileUploadSizeInBytes);

        // Assert
        result.Should().BeNull();
        var modalErrors = GetModelStateErrors();
        modalErrors.Should().HaveCount(1).And.Contain("The selected file is empty");
    }

    [TestMethod]
    public async Task ValidateUploadFileAndGetBytes_Invalid_ModalState_When_The_File_IsTooLarge()
    {
        var fileUploadSizeInBytes = 10485760; // 10 MB
        var fileName = "file.csv";
        var maxMb = fileUploadSizeInBytes / 1048576;
        IFormFile formFile = CreateFormFile(fileName, 10485900);

        // Act
        var result = await FileHelpers.ValidateUploadFileAndGetBytes(formFile, _modelStateDictionary, fileUploadSizeInBytes);

        // Assert
        result.Should().BeNull();
        var modalErrors = GetModelStateErrors();
        modalErrors.Should().HaveCount(1).And.Contain($"The selected file must be smaller than {maxMb}MB");
    }

    [TestMethod]
    public async Task ValidateUploadFileAndGetBytes_Invalid_ModalState_When_The_FileName_IsEmpty()
    {
        var fileUploadSizeInBytes = 1024;
        var fileName = "";

        IFormFile formFile = CreateFormFile(fileName, 100);

        // Act
        var result = await FileHelpers.ValidateUploadFileAndGetBytes(formFile, _modelStateDictionary, fileUploadSizeInBytes);

        // Assert
        result.Should().BeNull();
        var modalErrors = GetModelStateErrors();
        modalErrors.Should().HaveCount(1).And.Contain("The selected file name cannot be empty");
    }

    [TestMethod]
    public async Task ValidateUploadFileAndGetBytes_Invalid_ModalState_When_The_FileExtension_IsNotValid()
    {
        var fileUploadSizeInBytes = 1024;
        var fileName = "file.xyz";

        IFormFile formFile = CreateFormFile(fileName, 100);

        // Act
        var result = await FileHelpers.ValidateUploadFileAndGetBytes(formFile, _modelStateDictionary, fileUploadSizeInBytes);

        // Assert
        result.Should().BeNull();
        var modalErrors = GetModelStateErrors();
        modalErrors.Should().HaveCount(1).And.Contain("The selected file must be PDF, DOC, DOCX, XLS, XLSX, CSV, PNG, TIF, JPG or MSG");
    }

    [TestMethod]
    [DataRow("test.pdf")]
    [DataRow("test.doc")]
    [DataRow("test.docx")]
    [DataRow("test.xls")]
    [DataRow("test.xlsx")]
    [DataRow("test.csv")]
    [DataRow("test.png")]
    [DataRow("test.jpg")]
    [DataRow("test.jpeg")]
    [DataRow("test.tif")]
    [DataRow("test.tiff")]
    [DataRow("test.msg")]
    public async Task ValidateUploadFileAndGetBytes_ValidFile_Returns_FileBytes(string fileName)
    {
        var fileUploadSizeInBytes = 1024;

        IFormFile formFile = CreateFormFile(fileName, 100);

        // Act
        var result = await FileHelpers.ValidateUploadFileAndGetBytes(formFile, _modelStateDictionary, fileUploadSizeInBytes);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<byte[]?>();
    }

    [TestMethod]
    [DataRow("test.pdf", "application/pdf")]
    [DataRow("test.doc", "application/msword")]
    [DataRow("test.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [DataRow("test.xls", "application/vnd.ms-excel")]
    [DataRow("test.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [DataRow("test.csv", "text/csv")]
    [DataRow("test.png", "image/png")]
    [DataRow("test.jpg", "image/jpeg")]
    [DataRow("test.jpeg", "image/jpeg")]
    [DataRow("test.tif", "image/tiff")]
    [DataRow("test.tiff", "image/tiff")]
    [DataRow("test.msg", "application/octet-stream")]
    public void GetContentType_Should_Return_ContentType(string fileName, string expectedContentType)
    {
        var result = FileHelpers.GetContentType(fileName);
        result.Should().Be(expectedContentType);
    }

    private static FormFile CreateFormFile(string fileName, long length)
    {
        var content = new byte[100];
        var stream = new MemoryStream(content);
        var formFile = new FormFile(stream, 0, length, "Test data", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octect-stream"
        };
        return formFile;
    }

    private IEnumerable<string> GetModelStateErrors()
    {
        return _modelStateDictionary.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage);
    }
}
