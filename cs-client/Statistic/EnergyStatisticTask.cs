using CsClient.Data.DTO;
using CsClient.Utils;
using NLog;
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
    public class EnergyStatisticTask
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a new sample period for retrieving the energy consumption information.
        /// This clears the database and then starts a new session if possible.
        /// </summary>
        /// <exception cref="NotSupportedException">If OS does not support this tool / exit code is non-success.</exception>
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
                        Verb = "runas", // run as admin.
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0 )
                {
                    logger.Error("Process exit with non-success exit code. Does this OS support this utility? Process Exit Code: " + process.ExitCode);
                    throw new NotSupportedException("Energy Estimation Engine exited with fatal error.");
                }
            } catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            } catch (IOException ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }

            return tempFilePath;
        }
    }
}
