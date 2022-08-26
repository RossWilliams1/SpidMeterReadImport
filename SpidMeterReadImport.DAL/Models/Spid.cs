namespace SpidMeterReadImport.DAL.Models
{
    public class Spid
    {
        public int Id { get; set; }
        public string SPId { get; set; } = string.Empty;
        public string MeterSerial { get; set; } = string.Empty;
        public string MeterManufacturer { get; set; } = string.Empty;
        public decimal YearlyVolumeEstimate { get; set; }
        public byte MeterType { get; set; }
        public int? ReturnToSewer { get; set; }
        public string GeneralSPId { get; set; } = string.Empty;
        public int NumberOfReadDigits { get; set; }
        public string MeterLocationDescription { get; set; } = string.Empty;
        public int MeterReadFrequency { get; set; }
        public ICollection<SpidMeterRead> SpidMeterReads { get; set; }
    }
}