using SpidMeterReadImport.Domain.Csv;

namespace SpidMeterReadImport.Service.Imports.Interfaces
{
    public interface ISpidMeterReadCsvImporter
    {
        public List<SpidMeterReadCsvMapResult> ConvertCsvToObjects(Stream stream);
        public SpidMeterReadCsvMapResult ValidateHeader(string header);
        public SpidMeterReadCsvMapResult MapToObject(string row, int rowNumber);
    }
}
