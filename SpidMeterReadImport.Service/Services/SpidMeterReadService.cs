using Microsoft.EntityFrameworkCore;
using SpidMeterReadImport.DAL.DataContext;
using SpidMeterReadImport.DAL.Models;
using SpidMeterReadImport.Domain.Csv;
using SpidMeterReadImport.Service.Interfaces;
using SpidMeterReadImport.Service.Helper;
using SpidMeterReadImport.Domain.Extensions;
using SpidMeterReadImport.Service.Imports.Interfaces;

namespace SpidMeterReadImport.Service.Services
{
    public class SpidMeterReadService : ISpidMeterReadService
    {
        private readonly ISpidMeterReadCsvImporter _spidMeterReadCsvImporter;
        private readonly DataContext _context;
        public SpidMeterReadService(ISpidMeterReadCsvImporter spidMeterReadCsvImporter, DataContext context)
        {
            _spidMeterReadCsvImporter = spidMeterReadCsvImporter;
            _context = context;
        }
        public async Task<List<string>> ImportSpidMeterReads(Stream stream)
        {
            List<SpidMeterReadCsvMapResult> mapResults = _spidMeterReadCsvImporter.ConvertCsvToObjects(stream);

            string currentSpidRef = string.Empty;
            Spid? currentSpid = null;

            foreach (var readMapResult in mapResults.Where(x => x.Record != null && !x.HasErrors && !x.IsHeader).OrderBy(x => x.Record.SpId))
            {
                if (currentSpidRef != readMapResult.Record.SpId)
                {
                    currentSpid = await _context.Spid.Include(x => x.SpidMeterReads)
                        .AsNoTracking().FirstOrDefaultAsync(x => x.SPId == readMapResult.Record.SpId);
                    currentSpidRef = readMapResult.Record.SpId;
                }

                readMapResult.ValidateNewSpidRead(currentSpid);

                if (readMapResult.HasErrors)
                    continue;

                var read = new SpidMeterRead()
                {
                    SpidId = currentSpid.Id,
                    ManualReading = readMapResult.Record.ManualReading,
                    Reading = readMapResult.Record.Reading,
                    ReadingDate = readMapResult.Record.ReadingDate,
                    Rollover = readMapResult.Record.Rollover,
                    ReadType = readMapResult.Record.ReadType,
                    UsedForEstimate = readMapResult.Record.UsedForEstimate
                };

                //not tracked by context, added for the duplicatiosn check
                currentSpid.SpidMeterReads.Add(read);

                _context.SpidMeterRead.Add(read);
            }

            await _context.SaveChangesAsync();

            var defaultCsvResults = mapResults.Select(x =>
                new DefaultCsvMapResult { ColumnErrors = x.ColumnErrors, IsHeader = x.IsHeader, RowNumber = x.RowNumber }).ToList();

            return ImportHelper.CreateImportSummary(defaultCsvResults);
        }
    }
}
