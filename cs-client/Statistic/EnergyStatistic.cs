using CsClient.Data.DTO;
using CsClient.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CsClient.Statistic
{
    /// <summary>
    /// Energy statistic retrieval is very synchronous
    /// </summary>
    public class EnergyStatistic
    {
        /// <summary>
        /// Creates a new sample period for retrieving the energy consumption information.
        /// This clears the database and then starts a new session if possible.
        /// </summary>
        public string NewSample()
        {
            // Get the path of the system's temporary folder
            string tempPath = Path.GetTempPath();

            // Generate a random temporary filename
            string randomFileName = Path.GetRandomFileName();

            // Temp file to write to
            string tempFilePath = Path.Combine(tempPath, $"{randomFileName}.csv");
            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powercfg",
                        Arguments = $"/srumutil /output \"{tempFilePath}\"",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        Verb = "runas",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string err = process.StandardError.ReadToEnd();
            } catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Run in administrator mode.");
                // Not run in administrator mode.
            } catch (IOException ex)
            {
                Console.WriteLine("IO Excepton somehow");
            }

            return tempFilePath;
        }

        /// <summary>
        /// Parses the given file CSV and filters out entries that are over 1 minute old.
        /// </summary>
        /// <param name="resultPath">Path of file to parse.</param>
        /// <returns></returns>
        public string GetResults(string resultPath = "")
        {
            StringBuilder sb = new StringBuilder();

            // add headers.
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

            using (StreamReader reader = new StreamReader(resultPath))
            {
                string headerLine = reader.ReadLine();
                CsvParser csvParser = new CsvParser(headerLine);

                // Read the file line by line until the end of the file
                while (!reader.EndOfStream)
                {
                    // Read the next line
                    string line = reader.ReadLine();

                    string[] parts = csvParser.GetCsvParts(line);

                    // get timestamp
                    string timeStampOptional = csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.TimeStamp, parts);
                    if (timeStampOptional == null)
                    {
                        continue;
                    }
                   
                    
                    DateTime dateTime = DateTime.ParseExact(timeStampOptional, "yyyy-MM-dd:HH:mm:ss.ffff", null).ToUniversalTime();

                    // If less than 1 minute old, process.
                    if (DateTime.UtcNow.AddMinutes(-1) <= dateTime)
                    {
                        // Parse and append
                        StringBuilder csvLineBuilder = new StringBuilder();
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.AppId, parts) ?? "" + ","); // app
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.UserId, parts) ?? "" + ",");// user
                        csvLineBuilder.Append(new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() + ","); // timestamp
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.CPUEnergyConsumption, parts) ?? "" + ","); // cpu
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.SocEnergyConsumption, parts) ?? "" + ","); // soc
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.DisplayEnergyConsumption, parts) ?? "" + ","); // display
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.DiskEnergyConsumption, parts) ?? "" + ","); // disk
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.NetworkEnergyConsumption, parts) ?? "" + ","); // network
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.MBBEnergyConsumption, parts) ?? "" + ","); // mbb
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.OtherEnergyConsumption, parts) ?? "" + ","); // other
                        csvLineBuilder.Append(csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.EmiEnergyConsumption, parts) ?? ""); // emi

                        sb.AppendLine(csvLineBuilder.ToString());
                    }
                }
            }

            return sb.ToString();
        }
    }
}
