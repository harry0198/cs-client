using System;
using System.Collections.Generic;

namespace CsClient.Utils
{
    /// <summary>
    /// Parses a CSV String.
    /// Contains functions for extracting information by the given headers.
    /// </summary>
    public class CsvParser
    {
        private readonly Dictionary<String, int> headerToColumn = new Dictionary<string, int>();

        /// <summary>
        /// Initializes the parser and assigns headers to columns.
        /// </summary>
        /// <param name="header">Header portion of csv.</param>
        public CsvParser(string header)
        {
            AssignHeadersToColumns(header);
        }

        /// <summary>
        /// Gets a string at the index defined by the header or null.
        /// </summary>
        /// <param name="header">Header to fetch value for.</param>
        /// <param name="csvLines">Parts of csv to get string for.</param>
        /// <returns>String in part from the given header or null if not found.</returns>
        public string GetStringValueWithHeader(string header, string[] csvLines)
        {
            bool valueExists = headerToColumn.TryGetValue(header, out int column);
            if (!valueExists)
            {
                return null;
            } else
            {
                string value = csvLines[column].Trim();
                return value;
            }
        }

        /// <summary>
        /// Splits a csv into its respective parts.
        /// </summary>
        /// <param name="csvLine">Csv to split.</param>
        /// <returns>The split csv text.</returns>
        public string[] GetCsvParts(string csvLine)
        {
            return csvLine.Split(',');
        }

        private void AssignHeadersToColumns(string headerLine)
        {
            string[] headers = headerLine.Split(',');

            for (int i = 0; i < headers.Length; i++)
            {
                string header = headers[i].Trim();
                headerToColumn[header] = i;
            }
        }
    }
}
