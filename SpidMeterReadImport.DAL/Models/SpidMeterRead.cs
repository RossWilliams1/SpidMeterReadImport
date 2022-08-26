namespace SpidMeterReadImport.DAL.Models
{
    public class SpidMeterRead
    {
        public int Id { get; set; }
        public int SpidId { get; set; }
        public DateTime ReadingDate { get; set; }
        public int Reading { get; set; }
        public bool UsedForEstimate { get; set; }
        public bool ManualReading { get; set; }
        public bool Rollover { get; set; }
        public string? ReadType { get; set; }
        public Spid Spid { get; set; }
    }
}