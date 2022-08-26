using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpidMeterReadImport.Domain.Csv;
using System.ComponentModel.DataAnnotations;

namespace SpidMeterReadImport.Service.Helper.Tests
{
    [TestClass()]
    public class ImportHelperTests
    {
        #region MapDate
        [DataTestMethod()]
        [DataRow("20/1/2022")]
        [DataRow("20 Jan 2022")]
        public void MapDate_ShouldConvert(string date)
        {
            var resultDate = ImportHelper.MapDate(date, "test");

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().Be(new DateTime(2022, 1, 20));
        }

        [DataTestMethod()]
        [DataRow("test")]
        [DataRow("1/20/2022")]
        [DataRow("")]
        public void MapDate_ShouldNotCovert(string date)
        {
            var resultDate = ImportHelper.MapDate(date, "test");

            resultDate.Item1.Length.Should().BeGreaterThan(1);
            resultDate.Item2.Should().BeNull();
        }

        [TestMethod()]
        public void MapDate_ShouldNotConvertRequired()
        {
            var resultDate = ImportHelper.MapInt("", "test", false);

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().BeNull();
        }
        #endregion

        #region MapInt

        [DataTestMethod()]
        [DataRow("-1", -1)]
        [DataRow("12", 12)]
        [DataRow("109", 109)]
        public void MapInt_ShouldConvert(string stringInt, int expectedInt)
        {
            var resultDate = ImportHelper.MapInt(stringInt, "test");

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().Be(expectedInt);
        }

        [DataTestMethod()]
        [DataRow("2.8")]
        [DataRow("1.3")]
        [DataRow("-3.2")]
        [DataRow("")]
        public void MapInt_ShouldNotCovert(string testInt)
        {
            var resultDate = ImportHelper.MapInt(testInt, "test");

            resultDate.Item1.Length.Should().BeGreaterThan(1);
            resultDate.Item2.Should().BeNull();
        }

        [TestMethod()]
        public void MapInt_ShouldConvertNotRequired()
        {
            var resultDate = ImportHelper.MapDate("", "test", false);

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().BeNull();
        }

        #endregion

        #region MapString

        [DataTestMethod()]
        [DataRow("1234567", 5)]
        [DataRow("12", 1)]
        [DataRow("12345678910", 10)]
        [DataRow("", 10)]
        public void MapString_ShouldNotConvert(string stringInt, int maxLength)
        {
            var resultDate = ImportHelper.MapString(stringInt, "test", maxLength);

            resultDate.Item1.Length.Should().BeGreaterThan(1);
            resultDate.Item2.Should().BeNull();
        }

        [DataTestMethod()]
        [DataRow("2.8")]
        [DataRow("1.3")]
        [DataRow("-3.2")]
        [DataRow("2312non12o")]
        public void MapString_ShouldCovert(string testString)
        {
            var resultDate = ImportHelper.MapString(testString, "test");

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().Be(testString);
        }

        [TestMethod()]
        public void MapString_ShouldConvertNotRequired()
        {
            var resultDate = ImportHelper.MapDate("", "test", false);

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().BeNull();
        }
        #endregion

        #region MapBool

        [DataTestMethod()]
        [DataRow("1", true)]
        [DataRow("0", false)]
        [DataRow("true", true)]
        [DataRow("True", true)]
        [DataRow("false", false)]
        [DataRow("False", false)]
        [DataRow("yes", true)]
        [DataRow("no", false)]
        public void MapBool_ShouldConvert(string stringBool, bool expectedBool)
        {
            var resultDate = ImportHelper.MapBool(stringBool, "Test");

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().Be(expectedBool);
        }

        [DataTestMethod()]
        [DataRow("1.3")]
        [DataRow("lll")]
        [DataRow("0.2")]
        public void MapBool_ShouldNotCovert(string testInt)
        {
            var resultDate = ImportHelper.MapBool(testInt, "test");

            resultDate.Item1.Length.Should().BeGreaterThan(1);
            resultDate.Item2.Should().BeNull();
        }

        [TestMethod()]
        public void MapBool_ShouldConvertNotRequired()
        {
            var resultDate = ImportHelper.MapBool("", "test", false);

            resultDate.Item1.Should().Be(string.Empty);
            resultDate.Item2.Should().BeNull();
        }
        #endregion

        
        private class TestObjectNoAttr
        {
            public string test { get; set; }
        }

        private class TestObjectWithAttr
        {
            [Display(Name = "Test Field Name", Order = 29)]
            public string test { get; set; }
        }

        [TestMethod()]
        public void GetImportColumnsForObject_WithNoAttr()
        {
            var testObject = new TestObjectNoAttr { test = "testValue" };
            var result = ImportHelper.GetImportColumnsForObject(testObject);

            result.FirstOrDefault().Value.Should().Be(-1);
            result.FirstOrDefault().Key.Should().Be("test");
        }

        [TestMethod()]
        public void GetImportColumnsForObject_WithAttr()
        {
            var testObject = new TestObjectWithAttr { test = "testValue" };
            var result = ImportHelper.GetImportColumnsForObject(testObject);

            result.FirstOrDefault().Value.Should().Be(29);
            result.FirstOrDefault().Key.Should().Be("Test Field Name");
        }

        [TestMethod()]
        public void CreateImportSummaryTest()
        {
            var results = new List<DefaultCsvMapResult>()
            {
                new DefaultCsvMapResult()
                {
                    IsHeader = false,
                    RowNumber = 1,
                    ColumnErrors = new List<string>()
                    {
                        "error on row 1"
                    }
                },
                new DefaultCsvMapResult()
                {
                    IsHeader = false,
                    RowNumber = 2,
                    ColumnErrors = new List<string>()
                },
                new DefaultCsvMapResult()
                {
                    IsHeader = false,
                    RowNumber = 3,
                    ColumnErrors = new List<string>()
                    {
                        "error on row 3"
                    }
                }
            };

            var summary = ImportHelper.CreateImportSummary(results);

            summary.ElementAt(0).Should().Be("1 Rows successfully imported");
            summary.ElementAt(1).Should().Be("2 Rows failed to import");
            summary.ElementAt(2).Should().Be("Row: 1 - error on row 1");
            summary.ElementAt(3).Should().Be("Row: 3 - error on row 3");
        }
    }
}