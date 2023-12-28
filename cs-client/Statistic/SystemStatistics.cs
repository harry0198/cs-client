using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CsClient.Statistic
{

    /// <summary>
    /// 
    /// </summary>
    public class SystemStatistics: IDisposable
    {
        private readonly ManagementObjectSearcher _cpuSearcher;
        private readonly ManagementObjectSearcher _memorySearcher;
        private readonly ManagementObjectSearcher _diskSearcher;

        public SystemStatistics()
        {
            _cpuSearcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM CIM_Processor");
            _memorySearcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM CIM_OperatingSystem");
            _diskSearcher = new ManagementObjectSearcher("SELECT FreeSpace FROM Win32_LogicalDisk WHERE DeviceID='C:'");
        }

        public async Task<float> GetCpuUsageAsync()
        {
            return await GetUsageAsync(_cpuSearcher, "LoadPercentage", "CPU");
        }

        public async Task<float> GetMemoryUsageAsync()
        {
            return await GetUsageAsync(_memorySearcher, "FreePhysicalMemory", "memory");
        }

        public async Task<float> GetDiskUsageAsync()
        {
            return await GetUsageAsync(_diskSearcher, "FreeSpace", "disk");
        }

        private async Task<float> GetUsageAsync(ManagementObjectSearcher searcher, string propertyName, string infoType)
        {
            try
            {
                return await Task.Run(() =>
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return Convert.ToSingle(obj[propertyName]);
                    }

                    throw new InvalidOperationException($"Unable to retrieve {infoType} information.");
                });
            }
            catch (Exception ex)
            {
                // Log or handle the exception more gracefully
                Console.WriteLine($"Error retrieving {infoType} information: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _cpuSearcher.Dispose();
            _memorySearcher.Dispose();
            _diskSearcher.Dispose();
        }
    }
}
