using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpidMeterReadImport.Domain.DTO;
using System.Text;

namespace SpidMeterReadImport.Service.Imports.Tests
{
    [TestClass()]
    public class SpidMeterReadCsvImporterTests
    {
        private SpidMeterReadCsvImporter _spidMeterReadCsvImporter;
        private const string _validHeaders = "SPId,MeterSerial,ReadingDate,Reading,UsedForEstimate,ManualReading,Rollover,ReadType,GeneralSPId";

        [TestInitialize]
        public void Setup()
        {
            _spidMeterReadCsvImporter = new SpidMeterReadCsvImporter();
        }

        [TestMethod()]
        public void ConvertCsvToObjectTest_ShouldCovertRecord()
        {
            var csv = new StringBuilder();
            var newLine = _validHeaders;
            csv.AppendLine(newLine);
            newLine = "3024422755W10,08M161403,2017-12-17 00:00:00.000,3532,0,0,0,C,3024422757";
            csv.AppendLine(newLine);

            var stream = CsvTestHelpers.SetupCsvStream(csv);

            var meterReadImport = new SpidMeterReadImportDto()
            {
                SpId = "3024422755W10",
                ReadingDate = new DateTime(2017, 12, 17),
                Reading = 3532,
                ManualReading = false,
                ReadType = "C",
                Rollover = false,
                UsedForEstimate = false
            };

            var result = _spidMeterReadCsvImporter.ConvertCsvToObjects(stream);

            result.Count().Should().Be(2);
            result.SelectMany(x => x.ColumnErrors).Count().Should().Be(0);
            result.ElementAt(1).Record.Should().BeEquivalentTo(meterReadImport);
        }

        [TestMethod()]
        public void ConvertCsvToObjectTest_ShouldNotCovertRecord()
        {
            var csv = new StringBuilder();
            var newLine = _validHeaders;
            csv.AppendLine(newLine);
            newLine = "3024422755W10,08M161403,WrongDate,WrongInt,WrongBool1,WrongBool2,WrongBool3,ToooBiiig,3024422757";
            csv.AppendLine(newLine);

            var stream = CsvTestHelpers.SetupCsvStream(csv);

            var meterReadImport = new SpidMeterReadImportDto()
            {
                SpId = "3024422755W10",
                ReadingDate = new DateTime(2017, 12, 17),
                Reading = 3532,
                ManualReading = false,
                ReadType = "C",
                Rollover = false,
                UsedForEstimate = false
            };

            var result = _spidMeterReadCsvImporter.ConvertCsvToObjects(stream);

            result.Count().Should().Be(2);
            result.SelectMany(x => x.ColumnErrors).Count().Should().Be(6);
        }

        [TestMethod()]
        public void ConvertCsvToObjectTest_ShouldNotCovertRecordHeaderError()
        {
            var csv = new StringBuilder();
            var newLine = "SPId, MeterSerial, WrongDate, Reading, UsedForEstimate, ManualReading, Rollover, ReadType, GeneralSPId";
            csv.AppendLine(newLine);
            newLine = "3024422755W10,08M161403,WrongDate,WrongInt,WrongBool1,WrongBool2,WrongBool3,ToooBiiig,3024422757";
            csv.AppendLine(newLine);

            var stream = CsvTestHelpers.SetupCsvStream(csv);
            var result = _spidMeterReadCsvImporter.ConvertCsvToObjects(stream);

            result.Count().Should().Be(1);
            result.First().ColumnErrors.Count().Should().Be(1);
            result.First().RowNumber.Should().Be(0);
        }

        [TestMethod()]
        public void ValidateHeaderTest_ShouldNotHaveErrors()
        {
            var result = _spidMeterReadCsvImporter.ValidateHeader(_validHeaders);

            result.RowNumber.Should().Be(0);
            result.IsHeader.Should().Be(true);
            result.HasErrors.Should().Be(false);
            result.ColumnErrors.Count().Should().Be(0);
        }

        [DataTestMethod()]
        [DataRow("SPId, MeterSerial, WrongDate, Reading, UsedForEstimate, ManualReading, Rollover, ReadType, GeneralSPId", 1)]
        [DataRow("", 7)]
        [DataRow("SPId, MeterSerial, Reading, ReadingDate, UsedForEstimate, ManualReading, Rollover, ReadType, GeneralSPId", 2)]

        public void ValidateHeaderTest_ShouldHaveErrors(string header, int errorCount)
        {
            var result = _spidMeterReadCsvImporter.ValidateHeader(header);

            result.RowNumber.Should().Be(0);
            result.IsHeader.Should().Be(true);
            result.HasErrors.Should().Be(true);
            result.ColumnErrors.Count().Should().Be(errorCount);
        }


        [TestMethod()]
        public void ValidateHeaderTest_ShouldHaveMissingColumnInError()
        {
            string header = "SPId, MeterSerial, WrongDate, Reading, UsedForEstimate, ManualReading, Rollover, ReadType, GeneralSPId";
            var result = _spidMeterReadCsvImporter.ValidateHeader(header);

            result.RowNumber.Should().Be(0);
            result.IsHeader.Should().Be(true);
            result.HasErrors.Should().Be(true);
            result.ColumnErrors.Count().Should().Be(1);
            result.ColumnErrors.First().Should().Contain("ReadingDate");
        }


        [TestMethod()]
        public void MapToObjectTest_ShouldMapWithNoErrors()
        {
            var rowData = "3024422755W10,08M161403,2017-12-17 00:00:00.000,3532,0,0,0,C,3024422757";
            var meterReadImport = new SpidMeterReadImportDto()
            {
                SpId = "3024422755W10",
                ReadingDate = new DateTime(2017, 12, 17),
                Reading = 3532,
                ManualReading = false,
                ReadType = "C",
                Rollover = false,
                UsedForEstimate = false
            };

            var result = _spidMeterReadCsvImporter.MapToObject(rowData, 1);

            result.ColumnErrors.Count().Should().Be(0);
            result.RowNumber.Should().Be(1);
            result.Record.Should().BeEquivalentTo(meterReadImport);
        }

        [TestMethod()]
        public void MapToObjectTest_ShouldNotMap()
        {
            var rowData = ",08M161403,WrongDate,WrongInt,WrongBool,0,0,C,3024422757";

            var result = _spidMeterReadCsvImporter.MapToObject(rowData, 1);

            result.ColumnErrors.Count().Should().Be(4);
            result.ColumnErrors.Any(x => x.Contains("SpId - is required")).Should().Be(true);
            result.ColumnErrors.Any(x => x.Contains("WrongDate")).Should().Be(true);
            result.ColumnErrors.Any(x => x.Contains("WrongInt")).Should().Be(true);
            result.ColumnErrors.Any(x => x.Contains("WrongBool")).Should().Be(true);
            result.RowNumber.Should().Be(1);
        }
    }
}