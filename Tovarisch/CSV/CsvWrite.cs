namespace Wacton.Tovarisch.CSV
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CsvHelper;

    public class CsvWrite
    {
        // TODO: check for file lock ("TryWrite...")?
        public static void WriteRows<T>(IEnumerable<IEnumerable<T>> rows, string filepath)
        {
            using (var writer = File.CreateText(filepath))
            {
                WriteRows(rows, writer);
                writer.Flush();
            }
        }

        public static void AppendRows<T>(IEnumerable<IEnumerable<T>> rows, string filepath)
        {
            using (var appender = File.AppendText(filepath))
            {
                WriteRows(rows, appender);
                appender.Flush();
            }
        }

        public static void WriteColumns<T>(IEnumerable<IEnumerable<T>> columns, string filepath)
        {
            using (var fileWriter = File.CreateText(filepath))
            {
                WriteColumns(columns, fileWriter);
                fileWriter.Flush();
            }
        }

        private static void WriteRows<T>(IEnumerable<IEnumerable<T>> columns, TextWriter writer)
        {
            using (var csvWriter = new CsvWriter(writer))
            {
                WriteRows(columns, csvWriter);
                writer.Flush();
            }
        }

        private static void WriteColumns<T>(IEnumerable<IEnumerable<T>> columns, TextWriter writer)
        {
            using (var csvWriter = new CsvWriter(writer))
            {
                WriteColumns(columns, csvWriter);
                writer.Flush();
            }
        }

        private static void WriteRows<T>(IEnumerable<IEnumerable<T>> rows, CsvWriter csvWriter)
        {
            foreach (var row in rows)
            {
                foreach (var field in row)
                {
                    csvWriter.WriteField(field);
                }

                csvWriter.NextRecord();
            }
        }

        private static void WriteColumns<T>(IEnumerable<IEnumerable<T>> columns, CsvWriter csvWriter)
        {
            var columnList = columns.Select(column => column.ToList()).ToList();
            if (columnList.Count == 0)
            {
                return;
            }

            var dataCount = columnList.Max(column => column.Count);
            for (var i = 0; i < dataCount; i++)
            {
                foreach (var dataColumn in columnList)
                {
                    if (i >= dataColumn.Count)
                    {
                        csvWriter.WriteField(null);
                    }
                    else
                    {
                        csvWriter.WriteField(dataColumn[i]);
                    }
                }

                csvWriter.NextRecord();
            }
        }
    }
}
