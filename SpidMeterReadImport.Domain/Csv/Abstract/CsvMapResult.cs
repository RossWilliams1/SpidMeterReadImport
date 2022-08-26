namespace SpidMeterReadImport.Domain.Csv.Abstract
{
    public abstract class CsvMapResult
    {
        public int RowNumber { get; set; }
        public List<string> ColumnErrors { get; set; } = new List<string>();
        public bool IsHeader { get; set; }
        public bool HasErrors => ColumnErrors.Any();
    }
}
