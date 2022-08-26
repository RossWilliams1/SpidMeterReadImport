namespace SpidMeterReadImport.Service.Interfaces
{
    public interface ISpidMeterReadService
    {
        public Task<List<string>> ImportSpidMeterReads(Stream stream);
    }
}
