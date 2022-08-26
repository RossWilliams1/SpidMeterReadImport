using System.ComponentModel.DataAnnotations;

namespace SpidMeterReadImport.Domain.DTO
{
    public class SpidMeterReadImportDto
    {
        [Display(Order = 0)]
        public string SpId { get; set; }
        [Display(Order = 2)]
        public DateTime ReadingDate { get; set; }
        [Display(Order = 3)]
        public int Reading { get; set; }
        [Display(Order = 4)]
        public bool UsedForEstimate { get; set; }
        [Display(Order = 5)]
        public bool ManualReading { get; set; }
        [Display(Order = 6)]
        public bool Rollover { get; set; }
        [Display(Order = 7)]
        public string ReadType { get; set; }
    }
}
