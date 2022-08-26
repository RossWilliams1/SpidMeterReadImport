using SpidMeterReadImport.Domain.DTO;
using SpidMeterReadImport.Domain.Csv;
using SpidMeterReadImport.Service.Helper;
using SpidMeterReadImport.Service.Imports.Interfaces;

namespace SpidMeterReadImport.Service.Imports
{
    public class SpidMeterReadCsvImporter : ISpidMeterReadCsvImporter
    {
        public List<SpidMeterReadCsvMapResult> ConvertCsvToObjects(Stream stream)
        {
            List<SpidMeterReadCsvMapResult> mapResults = new();

            using (var reader = new StreamReader(stream))
            {
                int rowNumber = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (rowNumber == 0)
                    {
                        var headerResult = ValidateHeader(line);

                        mapResults.Add(headerResult);

                        if (headerResult.HasErrors)
                            break;

                        rowNumber++;
                        continue;
                    }

                    var rowResult = MapToObject(line, rowNumber);

                    mapResults.Add(rowResult);
                    rowNumber++;
                }
            }

            return mapResults;
        }

        public SpidMeterReadCsvMapResult ValidateHeader(string header)
        {
            SpidMeterReadCsvMapResult csvMapResult = new()
            {
                IsHeader = true,
                RowNumber = 0,
                ColumnErrors = new List<string>()
            };

            var importColumns = ImportHelper.GetImportColumnsForObject(new SpidMeterReadImportDto());
            var csvColumns = header.Split(',');

            foreach (var importHeader in importColumns)
            {
                if (importHeader.Value < 0 || importHeader.Value > csvColumns.Length
                    || csvColumns[importHeader.Value].Trim().ToLower() != importHeader.Key.Trim().ToLower())
                {
                    csvMapResult.ColumnErrors.Add($"Column {importHeader.Value + 1} expects header: {importHeader.Key}");
                }
            }

            return csvMapResult;
        }

        public SpidMeterReadCsvMapResult MapToObject(string row, int rowNumber)
        {
            var columns = row.Split(',');

            SpidMeterReadImportDto read = new();

            SpidMeterReadCsvMapResult csvMapResult = new()
            {
                IsHeader = false,
                RowNumber = rowNumber,
                ColumnErrors = new List<string>()
            };

            #region MapColumnsToRead
            Tuple<string, DateTime?> dateResultTuple;
            Tuple<string, string?> stringResultTuple;
            Tuple<string, bool?> boolResultTuple;
            Tuple<string, int?> intResultTuple;

            stringResultTuple = ImportHelper.MapString(columns[0], "SpId");
            if (!string.IsNullOrWhiteSpace(stringResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(stringResultTuple.Item1);
            else
                read.SpId = stringResultTuple.Item2;

            dateResultTuple = ImportHelper.MapDate(columns[2], "Reading Date");
            if (!string.IsNullOrWhiteSpace(dateResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(dateResultTuple.Item1);
            else if (dateResultTuple.Item2.HasValue)
                read.ReadingDate = dateResultTuple.Item2.Value;

            intResultTuple = ImportHelper.MapInt(columns[3], "Reading");
            if (!string.IsNullOrWhiteSpace(intResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(intResultTuple.Item1);
            else if (intResultTuple.Item2.HasValue)
                read.Reading = intResultTuple.Item2.Value;

            boolResultTuple = ImportHelper.MapBool(columns[4], "Used For Estimate");
            if (!string.IsNullOrWhiteSpace(boolResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(boolResultTuple.Item1);
            else if (boolResultTuple.Item2.HasValue)
                read.UsedForEstimate = boolResultTuple.Item2.Value;

            boolResultTuple = ImportHelper.MapBool(columns[5], "Manual Reading");
            if (!string.IsNullOrWhiteSpace(boolResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(boolResultTuple.Item1);
            else if (boolResultTuple.Item2.HasValue)
                read.ManualReading = boolResultTuple.Item2.Value;

            boolResultTuple = ImportHelper.MapBool(columns[6], "Rollover");
            if (!string.IsNullOrWhiteSpace(boolResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(boolResultTuple.Item1);
            else if (boolResultTuple.Item2.HasValue)
                read.Rollover = boolResultTuple.Item2.Value;

            stringResultTuple = ImportHelper.MapString(columns[7], "ReadType", 5);
            if (!string.IsNullOrWhiteSpace(stringResultTuple.Item1))
                csvMapResult.ColumnErrors.Add(stringResultTuple.Item1);
            else
                read.ReadType = stringResultTuple.Item2;

            csvMapResult.Record = read;
            #endregion

            return csvMapResult;
        }
    }
}
