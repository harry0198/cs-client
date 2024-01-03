using Extend;
using System;

namespace CsClient.Statistic
{
    /// <summary>
    /// Processes usage statistics to a CSV.
    /// </summary>
    public class UsageStatisticsCsvProcessor
    {
        /// <summary>
        /// Generates a CSV from the usage statistics.
        /// </summary>
        /// <param name="cpu">CPU usage.</param>
        /// <param name="memory">Memory usage.</param>
        /// <param name="disk">Disk usage.</param>
        /// <param name="network">Network usage.</param>
        /// <param name="timestamp">Timestamp of measure.</param>
        /// <returns></returns>
        public static string GenerateCsv(float cpu, float memory, float disk, float network, DateTime timestamp)
        {
            long timestampMs = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();

            string dataLine = $"{cpu},{memory},{disk},{network},{timestampMs}";

            return GetHeader() + System.Environment.NewLine + dataLine;
        }

        private static string GetHeader()
        {
            return
                $"{CsClientCsvHeaders.CPU}," +
                $"{CsClientCsvHeaders.Memory}," +
                $"{CsClientCsvHeaders.Disk}," +
                $"{CsClientCsvHeaders.Network}," +
                $"{CsClientCsvHeaders.TimeStamp}";
        }
    }
}
