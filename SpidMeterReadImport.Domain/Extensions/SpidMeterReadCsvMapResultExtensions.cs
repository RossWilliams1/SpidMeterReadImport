using SpidMeterReadImport.DAL.Models;
using SpidMeterReadImport.Domain.Csv;

namespace SpidMeterReadImport.Domain.Extensions
{
    public static class SpidMeterReadCsvMapResultExtensions
    {
        public static SpidMeterReadCsvMapResult ValidateNewSpidRead(this SpidMeterReadCsvMapResult result, Spid? spid)
        {
            if (spid == null)
            {
                result.ColumnErrors.Add($"No Spid found for {result.Record.SpId}");
            }
            else if (spid.SpidMeterReads.Any(x => x.ReadingDate == result.Record.ReadingDate && x.Reading == result.Record.Reading))
            {
                result.ColumnErrors.Add($"Already a Read with (Reading: {result.Record.Reading} and Reading Date {result.Record.ReadingDate:dd/MM/yyyy}) found for {result.Record.SpId}");
            }

            return result;
        }
    }
}