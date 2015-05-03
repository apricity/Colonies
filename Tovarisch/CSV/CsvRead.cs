namespace Wacton.Tovarisch.CSV
{
    using System.Collections.Generic;
    using System.IO;

    using CsvHelper;
    using CsvHelper.Configuration;

    public class CsvRead
    {
        public static IEnumerable<string[]> ReadRows(string filepath)
        {
            List<string[]> records;
            using (var reader = File.OpenText(filepath))
            {
                records = ReadRows(reader);
            }

            return records;
        }

        private static List<string[]> ReadRows(TextReader reader)
        {
            List<string[]> records;
            using (var csvReader = new CsvReader(reader, new CsvConfiguration { HasHeaderRecord = false }))
            {
                records = ReadRows(csvReader);
            }

            return records;
        }

        private static List<string[]> ReadRows(ICsvReader csvReader)
        {
            var records = new List<string[]>();
            while (csvReader.Read())
            {
                records.Add(csvReader.CurrentRecord);
            }

            return records;
        }
    }
}
