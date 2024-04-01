using CsClient.Credentials;
using CsClient.Utils;
using Extend;
using NLog;
using System;
using System.Collections.Concurrent;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string _resultPath;
        private readonly IAccountHelper _accountHelper;

        public EnergyStatisticsCsvProcessor(string resultPath, IAccountHelper accountHelper)
        {
            _resultPath = resultPath;
            _accountHelper = accountHelper;
        }

        /// <summary>
        /// Reads and processes the CSV file, filtering out entries over a specified age.
        /// </summary>
        /// <param name="removeFileAfterProcess">Should the File be removed after processing.</param>
        /// <returns>Processed CSV data as a string.</returns>
        /// <exception cref="FileNotFoundException">If the energy statistic CSV file cannot be found.</exception>
        public string ProcessCsv(bool removeFileAfterProcess = true)
        {
            if (!File.Exists(_resultPath))
            {
                logger.Error("Tried to process generated energy CSV but no file was found! Does this system support energy statistics data?");
                throw new FileNotFoundException("Could not find energy CSV file.");
            }
            string[] allLines = File.ReadAllLines(_resultPath);

            string header = allLines.FirstOrDefault();
            string[] dataLines = allLines.Skip(1).ToArray();
            CsvParser csvParser = new CsvParser(header);

            // Use thread safe collection. Order of the processed lines is irrelevant
            ConcurrentBag<string> processedLines = new ConcurrentBag<string>();

            // Concurrently process each line in the file.
            /*Parallel.ForEach(dataLines, (line) =>
            {
                string processedLine = ProcessLine(csvParser, line);

                // Add to concurrent bag if not null or is empty.
                if (!processedLine.IsNullOrEmpty())
                {
                    processedLines.Add(processedLine);
                }
            });*/
            for (int i = dataLines.Length -1; i >= 0; i--)
            {
                string line = dataLines[i];
                string processedLine = ProcessLine(csvParser, line);

                // Add to concurrent bag if not null or is empty.
                if (!processedLine.IsNullOrEmpty())
                {
                    processedLines.Add(processedLine);
                }
            }
               


            if (removeFileAfterProcess)
            {
                try
                {
                    File.Delete(_resultPath);
                } catch (Exception ex)
                {
                    logger.Warn("Unable to delete energy statistics CSV file. This may lead to excessive application storage consumption. See: " + _resultPath);
                    logger.Warn("Operation error msg: " + ex.Message);
                }
            }

            return GetHeader() + System.Environment.NewLine + string.Join(System.Environment.NewLine, processedLines);
        }

        private static string GetHeader()
        {
            return
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
                $"{CsClientCsvHeaders.EmiEnergyConsumption}," + 
                $"{CsClientCsvHeaders.AccountType}";
        }

        private string ProcessLine(CsvParser csvParser, string line)
        {
            string[] parts = csvParser.GetCsvParts(line);
            string timeStampOptional = csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.TimeStamp, parts);

            if (timeStampOptional == null) return null;
            DateTime dateTime;
            try
            {
                dateTime = DateTime.ParseExact(timeStampOptional, "yyyy-MM-dd:HH:mm:ss.ffff", null).ToUniversalTime().AddHours(1);
            } catch (FormatException)
            {
                logger.Warn("Failed to process line: " + line);
                return "";
            }
            if (DateTime.UtcNow.AddMinutes(-1) > dateTime) return null;

            return BuildCsvLine(csvParser, parts, dateTime);
        }

        private string BuildCsvLine(CsvParser csvParser, string[] parts, DateTime dateTime)
        {
            StringBuilder csvLineBuilder = new StringBuilder();
            string appId = csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.AppId, parts) ?? string.Empty;
            appId = TruncateStringToLastSlash(appId);

            string userSID = csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.UserId, parts) ?? string.Empty;
            AccountDetails? windowsAccountDetails = _accountHelper.GetUserAccount(userSID);

            // If it is not a user account / account does not exist, ignore.
            if (windowsAccountDetails == null || windowsAccountDetails.AccountType == AccountType.UNKNOWN) return null;


            csvLineBuilder.Append(appId + ","); // app
            csvLineBuilder.Append(windowsAccountDetails.AccountName + ",");// user
            csvLineBuilder.Append(new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() + ","); // timestamp
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.CPUEnergyConsumption, parts) ?? "") + ","); // cpu
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.SocEnergyConsumption, parts) ?? "") + ","); // soc
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.DisplayEnergyConsumption, parts) ?? "") + ","); // display
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.DiskEnergyConsumption, parts) ?? "") + ","); // disk
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.NetworkEnergyConsumption, parts) ?? "") + ","); // network
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.MBBEnergyConsumption, parts) ?? "") + ","); // mbb
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.OtherEnergyConsumption, parts) ?? "") + ","); // other
            csvLineBuilder.Append((csvParser.GetStringValueWithHeader(MicrosoftCsvHeaders.EmiEnergyConsumption, parts) ?? "") + ","); // emi
            csvLineBuilder.Append(Enum.GetName(typeof(AccountType), windowsAccountDetails.AccountType)); // Account Type

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
            int lastIndexForward = input.LastIndexOf('/');
            int lastIndexBack = input.LastIndexOf("\\");
            int lastIndex = Math.Max(lastIndexForward, lastIndexBack);

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
