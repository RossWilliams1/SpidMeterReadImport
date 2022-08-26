using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SpidMeterReadImport.Domain.Csv;

namespace SpidMeterReadImport.Service.Helper
{
    public static class ImportHelper
    {
        private static string ConstructFailedToConvertError(string column, string value, bool required, string dataType) => $"{column} - failed to convert ({value}) to a {(required ? "required" : "")} {dataType}";

        public static Dictionary<string, int> GetImportColumnsForObject(object objectTogetName)
        {
            Type TheType = objectTogetName.GetType();
            PropertyInfo[] Properties = TheType.GetProperties();
            Dictionary<string, int> columns = new();

            foreach (PropertyInfo Prop in Properties)
            {
                string columnHeader = Prop.Name;
                var displayAttr = Prop.GetCustomAttribute<DisplayAttribute>();

                if (!string.IsNullOrWhiteSpace(displayAttr?.Name))
                    columnHeader = displayAttr.Name;

                var order = displayAttr?.Order ?? -1;

                columns.Add(columnHeader, order);
            }

            return columns;
        }

        public static Tuple<string, int?> MapInt(string value, string column, bool required = true)
        {
            if (!required && string.IsNullOrWhiteSpace(value))
                return new Tuple<string, int?>("", null);
            else if (int.TryParse(value, out int parsedReading))
                return new Tuple<string, int?>("", parsedReading);
            else
                return new Tuple<string, int?>(ConstructFailedToConvertError(column, value, required, "int"), null);
        }

        public static Tuple<string, string?> MapString(string value, string column, int? maxLength = null, bool required = true)
        {
            if (maxLength.HasValue && maxLength < value.Length)
                return new Tuple<string, string?>($"{column} - value ({value}) is too large: maximum of {maxLength}", null);
            else if (required && string.IsNullOrWhiteSpace(value))
                return new Tuple<string, string?>($"{column} - is required", null);
            else
                return new Tuple<string, string?>("", value);
        }

        public static Tuple<string, bool?> MapBool(string value, string column, bool required = true)
        {
            var formattedValue = value.Trim().ToLower();
            List<string> validTrue = new() { "1", "true", "yes" };
            List<string> validFalse = new() { "0", "false", "no" };
            validTrue.Concat(validFalse);

            if (!required && string.IsNullOrWhiteSpace(formattedValue))
                return new Tuple<string, bool?>("", null);
            else if (validTrue.Concat(validFalse).Any(x => x == formattedValue))
                return new Tuple<string, bool?>("", validTrue.Any(x => x == formattedValue));
            else
                return new Tuple<string, bool?>(ConstructFailedToConvertError(column, value, required, "bool"), null);
        }

        public static Tuple<string, DateTime?> MapDate(string value, string column, bool required = true)
        {
            if (!required && string.IsNullOrWhiteSpace(value))
                return new Tuple<string, DateTime?>("", null);
            else if (DateTime.TryParse(value, out DateTime parsedDate))
                return new Tuple<string, DateTime?>("", parsedDate);
            else
                return new Tuple<string, DateTime?>(ConstructFailedToConvertError(column, value, required, "date"), null);
        }

        public static List<string> CreateImportSummary(List<DefaultCsvMapResult> results)
        {
            List<string> Summary = new();
            int successfulRowCount = results.Count(x => !x.HasErrors && !x.IsHeader);
            int unsuccessfulRowCount = results.Where(x => x.HasErrors && !x.IsHeader).Select(x => x.RowNumber).Distinct().Count();

            Summary.Add($"{successfulRowCount} Rows successfully imported");
            Summary.Add($"{unsuccessfulRowCount} Rows failed to import");

            foreach (var i in results.Where(x => x.HasErrors))
                Summary.AddRange(i.ColumnErrors.Select(x => $"Row: {i.RowNumber} - {x}").ToList());

            return Summary;
        }
    }
}
