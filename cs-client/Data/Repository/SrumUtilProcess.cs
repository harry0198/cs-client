using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClient.Data.Repository
{
    internal class SrumUtilProcess
    {
        static void Example(string[] args)
        {
            var info = new ProcessStartInfo()
            {
                FileName = "powercfg.exe",
                Arguments = "/srumutil /output C:\\EnergyConsumption\\srumutil.csv /csv",
                Verb = "runas",
                UseShellExecute = false
            };

            var proc = Process.Start(info);
            proc.WaitForExit();
            Console.ReadLine();
        }
    }
}
