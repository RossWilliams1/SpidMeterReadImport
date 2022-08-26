using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpidMeterReadImport.Service.Interfaces;
using System.Text;

namespace SpidMeterReadImport.Controllers.Tests
{
    [TestClass()]
    public class SpidMeterReadControllerTests
    {
        private Mock<ISpidMeterReadService> _spidMeterReadService;
        private SpidMeterReadController _spidMeterReadController;

        [TestInitialize]
        public void Setup()
        {
            _spidMeterReadService = new Mock<ISpidMeterReadService>();
            _spidMeterReadController = new SpidMeterReadController(_spidMeterReadService.Object);
        }

        [TestMethod()]
        public async Task MeterReadUploadTestAsync_OkWithResult()
        {
            _spidMeterReadController.ControllerContext.HttpContext = CreateContextWithFileRequest(Encoding.UTF8.GetBytes("Test File Data"));

            var importResult = new List<string>() { "TestResult" };
            _spidMeterReadService.Setup(x => x.ImportSpidMeterReads(It.IsAny<Stream>())).ReturnsAsync(importResult);

            var response = await _spidMeterReadController.MeterReadUpload();

            response.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)response;
            okResult.Value.Should().Be(importResult);
        }

        [TestMethod()]
        public async Task MeterReadUploadTestAsync_BadRequestEmptyFile()
        {
            _spidMeterReadController.ControllerContext.HttpContext = CreateContextWithFileRequest(Encoding.UTF8.GetBytes(""));

            var importResult = new List<string>() { "TestResult" };
            _spidMeterReadService.Setup(x => x.ImportSpidMeterReads(It.IsAny<Stream>())).ReturnsAsync(importResult);

            var response = await _spidMeterReadController.MeterReadUpload();

            response.Should().BeOfType<BadRequestObjectResult>();
            var badResult = (BadRequestObjectResult)response;
            badResult.Value.Should().Be("No file uploaded or it is empty.");
        }

        [TestMethod()]
        public async Task MeterReadUploadTestAsync_BadRequestNoFile()
        {
            var importResult = new List<string>() { "TestResult" };
            _spidMeterReadService.Setup(x => x.ImportSpidMeterReads(It.IsAny<Stream>())).ReturnsAsync(importResult);

            var response = await _spidMeterReadController.MeterReadUpload();

            response.Should().BeOfType<BadRequestObjectResult>();
            var badResult = (BadRequestObjectResult)response;
            badResult.Value.Should().Be("No file uploaded or it is empty.");
        }

        [TestMethod()]
        public async Task MeterReadUploadTestAsync_NullImportService500Error()
        {
            var spidMeterReadControllerNoImportService = new SpidMeterReadController(null);
            spidMeterReadControllerNoImportService.ControllerContext.HttpContext = CreateContextWithFileRequest(Encoding.UTF8.GetBytes("Test File Data"));

            var response = await spidMeterReadControllerNoImportService.MeterReadUpload();
            response.Should().BeOfType<StatusCodeResult>();
            var internalErrorResult = (StatusCodeResult)response;
            internalErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        private static DefaultHttpContext CreateContextWithFileRequest(byte[] fileDataBytes)
        {
            var httpContext = new DefaultHttpContext();
            var memoryStream = new MemoryStream(fileDataBytes);

            var formFileCollection = new FormFileCollection { new FormFile(memoryStream, 0, fileDataBytes.Length, "TestFile", "TestFileName") };
            httpContext.Request.Form = new FormCollection(null, formFileCollection);

            return httpContext;
        }
    }
}