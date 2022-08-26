using SpidMeterReadImport.Domain.Csv.Abstract;
using SpidMeterReadImport.Domain.DTO;

namespace SpidMeterReadImport.Domain.Csv
{
    public class SpidMeterReadCsvMapResult : CsvMapResult
    {
        public SpidMeterReadImportDto? Record { get; set; }

    }
}
