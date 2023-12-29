using CsClient.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClient.Statistic
{
    /// <summary>
    /// Processes and filters energy statistics data from a CSV file.
    /// </summary>
    public class EnergyStatisticsCsvProcessor
    {
        private readonly string _resultPath;

        public EnergyStatisticsCsvProcessor(string resultPath)
        {
            _resultPath = resultPath;
        }

        /// <summary>
        /// Reads and processes the CSV file, filtering out entries over a specified age.
        /// </summary>
        /// <returns>Processed CSV data as a string.</returns>
        public string ProcessCsv()
        {
            StringBuilder sb = new StringBuilder();
            AddHeaders(sb);

            using (StreamReader reader = new StreamReader(_resultPath))
            {
                string headerLine = reader.ReadLine();
                CsvParser csvParser = new CsvParser(headerLine);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string processedLine = ProcessLine(csvParser, line);
                    if (!string.IsNullOrEmpty(processedLine))
                    {
                        sb.AppendLine(processedLine);
                    }
                }
            }

            return sb.ToString();
        }

        private void AddHeaders(StringBuilder sb)
        {
            sb.AppendLine(
                $"{CsClientCsvHeaders.AppId}," +
                $"{CsClientCsvHeaders.UserId}," +
                $"{CsClientCsvHeaders.TimeStamp}," +
                $"{CsClientCsvHeaders.CPUEnergyConsumption}," +
                $"{CsClientCsvHeaders.SocEnergyConsumption}," +
                $"{CsClientCsvHeaders.DisplayEnergyConsumption}," +
                $"{CsClientCsvHeaders.DiskEnergyConsumption}," +
                $"{CsClientCsvHeaders.NetworkEnergyConsumption}," +
                $"{CsClientCsvHeaders.MBBEnergyConsumption}," +
                $"{CsClientCsvHeaders.OtherEnergyConsumption}," +
                $"{CsClientCsvHeaders.EmiEnergyConsumption}");
        }

        private string ProcessLine(CsvParser csvParser, string line)
        {
            string[] parts = csvParser.GetCsvParts(line);
            string timeStampOptional = csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.TimeStamp, parts);

            if (timeStampOptional == null) return null;

            DateTime dateTime = DateTime.ParseExact(timeStampOptional, "yyyy-MM-dd:HH:mm:ss.ffff", null).ToUniversalTime();

            if (DateTime.UtcNow.AddMinutes(-1) > dateTime) return null;

            return BuildCsvLine(csvParser, parts, dateTime);
        }

        private string BuildCsvLine(CsvParser csvParser, string[] parts, DateTime dateTime)
        {
            StringBuilder csvLineBuilder = new StringBuilder();
            string appId = csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.AppId, parts) ?? "";
            appId = TruncateStringToLastSlash(appId);

            csvLineBuilder.Append(appId + ","); // app
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.UserId, parts) ?? "") + ",");// user
            csvLineBuilder.Append(new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() + ","); // timestamp
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.CPUEnergyConsumption, parts) ?? "") + ","); // cpu
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.SocEnergyConsumption, parts) ?? "") + ","); // soc
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.DisplayEnergyConsumption, parts) ?? "") + ","); // display
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.DiskEnergyConsumption, parts) ?? "") + ","); // disk
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.NetworkEnergyConsumption, parts) ?? "") + ","); // network
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.MBBEnergyConsumption, parts) ?? "") + ","); // mbb
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.OtherEnergyConsumption, parts) ?? "") + ","); // other
            csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.EmiEnergyConsumption, parts) ?? ""); // emi

            return csvLineBuilder.ToString();
        }

        /// <summary>
        /// Truncates a string up to the last occurrence of '/'.
        /// </summary>
        /// <param name="input">The string to be truncated.</param>
        /// <returns>
        /// The truncated string up to the last occurrence of '/'.
        /// If '/' is not found, returns the original string.
        /// </returns>
        private string TruncateStringToLastSlash(string input)
        {
            // Find the last index of '/'
            int lastIndex = input.LastIndexOf('/');

            // If '/' is not found, return the original string
            if (lastIndex == -1)
            {
                return input;
            }

            // Truncate the string up to the last '/'
            return input.Substring(lastIndex + 1);
        }
    }
}
