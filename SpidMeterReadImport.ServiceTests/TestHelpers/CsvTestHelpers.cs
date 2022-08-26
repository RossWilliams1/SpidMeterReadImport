using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpidMeterReadImport.Domain.DTO;
using SpidMeterReadImport.Service.Imports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpidMeterReadImport.Service.Imports.Tests
{
    public static class CsvTestHelpers
    {
        private const string _testCsvPath = @"C:\temp\SpidMeterReadCsvTest.csv";
        public static Stream SetupCsvStream(StringBuilder csv)
        {
            Stream truncateStream;

            if (File.Exists(_testCsvPath))
                truncateStream = File.Open(_testCsvPath, FileMode.Truncate);
            else
                truncateStream = File.Open(_testCsvPath, FileMode.OpenOrCreate);

            using (StreamWriter writer = new StreamWriter(truncateStream))
            {
                writer.Write(csv);
            }

            return File.Open(_testCsvPath, FileMode.Open);
        }
    }
}