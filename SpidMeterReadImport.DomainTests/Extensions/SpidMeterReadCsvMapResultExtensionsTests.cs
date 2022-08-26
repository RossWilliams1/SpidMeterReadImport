using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpidMeterReadImport.DAL.Models;
using SpidMeterReadImport.Domain.Csv;

namespace SpidMeterReadImport.Domain.Extensions.Tests
{
    [TestClass()]
    public class SpidMeterReadCsvMapResultExtensionsTests
    {
        [TestMethod()]
        public void ValidateNewSpidRead_NoSpidTest()
        {
            var result = new SpidMeterReadCsvMapResult()
            {
                Record = new DTO.SpidMeterReadImportDto()
                {
                    SpId = "TestSpid"
                }
            };

            result.ValidateNewSpidRead(null);

            result.ColumnErrors.Count().Should().Be(1);
            result.ColumnErrors.First().Should().Be($"No Spid found for {result.Record.SpId}");
        }

        [TestMethod()]
        public void ValidateNewSpidRead_ShouldBeValid()
        {
            var result = new SpidMeterReadCsvMapResult()
            {
                Record = new DTO.SpidMeterReadImportDto()
                {
                    SpId = "TestSpid",
                    Reading = 1,
                    ReadingDate = DateTime.Now.Date
                }
            };

            var spid = new Spid() { SPId = "TestSpid", SpidMeterReads = new List<SpidMeterRead>() };

            result.ValidateNewSpidRead(spid);

            result.ColumnErrors.Count().Should().Be(0);
        }

        [TestMethod()]
        public void ValidateNewSpidRead_ShouldFlagDuplicate()
        {
            var result = new SpidMeterReadCsvMapResult()
            {
                Record = new DTO.SpidMeterReadImportDto()
                {
                    SpId = "TestSpid",
                    Reading = 1,
                    ReadingDate = DateTime.Now.Date
                }
            };

            var spid = new Spid()
            {
                SPId = "TestSpid",
                SpidMeterReads = new List<SpidMeterRead>()
                {
                    new SpidMeterRead()
                    {
                        ReadingDate = DateTime.Now.Date,
                        Reading = 1
                    }
                }
            };

            result.ValidateNewSpidRead(spid);

            result.ColumnErrors.Count().Should().Be(1);
            result.ColumnErrors.First().Should().Be($"Already a Read with (Reading: {result.Record.Reading} and Reading Date {result.Record.ReadingDate:dd/MM/yyyy}) found for {result.Record.SpId}");
        }
    }
}