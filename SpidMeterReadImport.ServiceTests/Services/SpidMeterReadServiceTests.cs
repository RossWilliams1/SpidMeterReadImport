using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpidMeterReadImport.DAL.DataContext;
using SpidMeterReadImport.DAL.Models;
using SpidMeterReadImport.Domain.Csv;
using SpidMeterReadImport.Domain.DTO;
using SpidMeterReadImport.Service.Imports.Interfaces;
using SpidMeterReadImport.Service.Imports.Tests;
using System.Text;

namespace SpidMeterReadImport.Service.Services.Tests
{
    [TestClass()]
    public class SpidMeterReadServiceTests 
    {
        private SpidMeterReadService _spidMeterReadService;
        private Mock<ISpidMeterReadCsvImporter> _spidMeterReadCsvImporter;
        private DataContext _context;
        
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "SpidMeterReadImportTest")
                .Options;
            _context = new DataContext(options);
            _spidMeterReadCsvImporter = new Mock<ISpidMeterReadCsvImporter>();
            _spidMeterReadService = new SpidMeterReadService(_spidMeterReadCsvImporter.Object, _context);
        }

        [TestMethod()]
        public async Task ImportSpidMeterReadsTest_ShouldImportOneAndHaveOneDupeAsync()
        {
            SeedSpidInMemoryDb(new List<Spid>() { new Spid { SPId = "test" } });

            var readingDate = DateTime.Now.Date;

            var testSpidMeterResults = new List<SpidMeterReadCsvMapResult>()
                {
                    new SpidMeterReadCsvMapResult()
                    {
                        Record = new SpidMeterReadImportDto(){SpId = "test", Reading = 123, ReadingDate = readingDate},
                        RowNumber = 1
                    },
                    new SpidMeterReadCsvMapResult()
                    {
                        Record = new SpidMeterReadImportDto(){SpId = "test", Reading = 123, ReadingDate = readingDate},
                        RowNumber = 2
                    }
                };


            _spidMeterReadCsvImporter.Setup(x => x.ConvertCsvToObjects(It.IsAny<Stream>()))
                .Returns(testSpidMeterResults);

            var results = await _spidMeterReadService.ImportSpidMeterReads(It.IsAny<Stream>());
           
            results.Count.Should().Be(3);
            results.ElementAt(2).Should().Be($"Row: 2 - Already a Read with (Reading: 123 and Reading Date {readingDate:dd/MM/yyyy}) found for test");
            
            await _context.Database.EnsureDeletedAsync();
        }

        [TestMethod()]
        public async Task ImportSpidMeterReadsTest_ShouldHaveDupeAsync()
        {
            var readingDate = DateTime.Now.Date;

            SeedSpidInMemoryDb(new List<Spid>() { new Spid 
            { 
                SPId = "test", SpidMeterReads = new List<SpidMeterRead>() 
                    { new SpidMeterRead { Reading = 123, ReadingDate = readingDate } } } 
            });

            var testSpidMeterResults = new List<SpidMeterReadCsvMapResult>()
                {
                    new SpidMeterReadCsvMapResult()
                    {
                        Record = new SpidMeterReadImportDto(){SpId = "test", Reading = 123, ReadingDate = readingDate},
                        RowNumber = 1
                    }
                };

            _spidMeterReadCsvImporter.Setup(x => x.ConvertCsvToObjects(It.IsAny<Stream>()))
                .Returns(testSpidMeterResults);

            var results = await _spidMeterReadService.ImportSpidMeterReads(It.IsAny<Stream>());

            results.Count.Should().Be(3);
            results.ElementAt(2).Should().Be($"Row: 1 - Already a Read with (Reading: 123 and Reading Date {readingDate:dd/MM/yyyy}) found for test");

            var spid = await _context.Spid.Include(x => x.SpidMeterReads).FirstOrDefaultAsync();
            spid.SpidMeterReads.Count().Should().Be(1);

            await _context.Database.EnsureDeletedAsync();
        }

        [TestMethod()]
        public async Task ImportSpidMeterReadsTest_ShouldNotImportSpidDoesNotExist()
        {
            SeedSpidInMemoryDb(new List<Spid>() { new Spid { SPId = "test" } });

            var readingDate = DateTime.Now.Date;

            var testSpidMeterResults = new List<SpidMeterReadCsvMapResult>()
                {
                    new SpidMeterReadCsvMapResult()
                    {
                        Record = new SpidMeterReadImportDto(){SpId = "testMissingSpid", Reading = 123, ReadingDate = readingDate },
                        RowNumber = 1
                    }
                };

            _spidMeterReadCsvImporter.Setup(x => x.ConvertCsvToObjects(It.IsAny<Stream>()))
                .Returns(testSpidMeterResults);

            var results = await _spidMeterReadService.ImportSpidMeterReads(It.IsAny<Stream>());

            results.Count.Should().Be(3);
            results.ElementAt(2).Should().Be($"Row: 1 - No Spid found for testMissingSpid");

            await _context.Database.EnsureDeletedAsync();
        }

        [TestMethod()]
        public async Task ImportSpidMeterReadsTest_ShouldNotImportHeaderError()
        {
            SeedSpidInMemoryDb(new List<Spid>() { new Spid { SPId = "test" } });

            var readingDate = DateTime.Now.Date;

            var testSpidMeterResults = new List<SpidMeterReadCsvMapResult>()
                {
                    new SpidMeterReadCsvMapResult()
                    {
                        Record = new SpidMeterReadImportDto(){SpId = "test", Reading = 123, ReadingDate = readingDate },
                        RowNumber = 1
                    }
                };

            _spidMeterReadCsvImporter.Setup(x => x.ConvertCsvToObjects(It.IsAny<Stream>()))
                .Returns(testSpidMeterResults);

            var results = await _spidMeterReadService.ImportSpidMeterReads(It.IsAny<Stream>());
            results.Count.Should().Be(2);

            await _context.Database.EnsureDeletedAsync();
        }

        private void SeedSpidInMemoryDb(List<Spid> spids)
        {
            _context.Spid.AddRange(spids);
            _context.SaveChanges();
        }
    }
}