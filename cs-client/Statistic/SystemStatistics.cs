using System;
using System.Management;
using System.Threading.Tasks;

namespace CsClient.Statistic
{

    /// <summary>
    /// Contains methods to retrieve usage from a machine.
    /// </summary>
    public class SystemStatistics: IDisposable
    {
        private readonly ManagementObjectSearcher _cpuSearcher;
        private readonly ManagementObjectSearcher _memorySearcher;
        private readonly ManagementObjectSearcher _diskSearcher;
        private readonly ManagementObjectSearcher _networkSearcher;

        /// <summary>
        /// Class constructor.
        /// Initializes the management objects to retrieve the system statistics.
        /// </summary>
        public SystemStatistics()
        {
            _cpuSearcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM CIM_Processor");
            _memorySearcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM CIM_OperatingSystem");
            _diskSearcher = new ManagementObjectSearcher("SELECT FreeSpace FROM Win32_LogicalDisk WHERE DeviceID='C:'");
            _networkSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_Tcpip_NetworkInterface");
        }

        /// <summary>
        /// Gets the cpu usage async.
        /// </summary>
        /// <returns>CPU usage task.</returns>
        public async Task<float> GetCpuUsageAsync()
        {
            return await GetUsageAsync(_cpuSearcher, "LoadPercentage", "CPU");
        }

        /// <summary>
        /// Gets the memory usage async.
        /// </summary>
        /// <returns>Memory usage task.</returns>
        public async Task<float> GetMemoryUsageAsync()
        {
            return await GetUsageAsync(_memorySearcher, "FreePhysicalMemory", "memory");
        }

        /// <summary>
        /// Gets the disk usage async.
        /// </summary>
        /// <returns>Disk usage task.</returns>
        public async Task<float> GetDiskUsageAsync()
        {
            return await GetUsageAsync(_diskSearcher, "FreeSpace", "disk");
        }

        /// <summary>
        /// Gets the network usage async.
        /// </summary>
        /// <returns>Network usage task.</returns>
        public async Task<float> GetNetworkUsageAsync()
        {
            float currentBytesPerSec = await GetUsageAsync(_networkSearcher, "CurrentBandwidth", "network-bandwidth");
            float bytesPerSec = await GetUsageAsync(_networkSearcher, "BytesTotalPersec", "network-bytes");

            return (bytesPerSec / currentBytesPerSec) * 100;
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

        /// <summary>
        /// Dispose the memory allocations.
        /// </summary>
        public void Dispose()
        {
            _cpuSearcher.Dispose();
            _memorySearcher.Dispose();
            _diskSearcher.Dispose();
            _networkSearcher.Dispose();
        }
    }
}
