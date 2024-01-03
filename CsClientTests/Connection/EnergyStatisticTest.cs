
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsClient.Statistic;
using CsClient.Credentials;
using System.IO;

namespace CsClientTests.Connection
{
    /// <summary>
    /// Tests for the <see cref="EnergyStatisticTask"/> class.
    /// </summary>
    [TestClass]
    public class EnergyStatisticTest : BaseTest
    {
        private string header = 
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

        /// <summary>
        /// Tests that the <see cref="ParseMicrosoftEnergyCsvFile"/> function works as expected with actual csv file data.
        /// </summary>
        [TestMethod]
        public void ParseMicrosoftEnergyCsvFile()
        {
            // Arrange
            string resultPath = Path.Combine(TestDir, "energydata.csv");
            EnergyStatisticsCsvProcessor energyStatistic = new EnergyStatisticsCsvProcessor(resultPath, new WindowsSIDAccountHelper());
            string firstLine = $"{header}\r\n";
            string entry1 = $"MicrosoftWindows.Client.CBS_1000.22677.1000.0_x64__cw5n1h2txyewy,harry,253400111147000,517,20,30,38,40,50,60,70,LOCAL";
            string entry2 = $"Microsoft.Windows.StartMenuExperienceHost_10.0.22621.2506_neutral_neutral_cw5n1h2txyewy,harry,253400111147000,1,90,20,30,40,50,60,70,LOCAL";

            // Act
            string results = energyStatistic.ProcessCsv(false);

            // Assert
            Assert.IsTrue(results.StartsWith(firstLine));
            Assert.IsTrue(results.Contains(entry1));
            Assert.IsTrue(results.Contains(entry2));
        }

        /// <summary>
        /// Tests that the file gets removed after processing.
        /// </summary>
        [TestMethod]
        public void ParseMicrosoftEnergyCsvFile_DoesDeleteAfter()
        {
            // Arrange
            string resultPath = Path.Combine(TestDir, "energydata.csv");
            string tempPath = Path.Combine(TempDir, "energydata.csv");
            File.Copy(resultPath, tempPath);
            EnergyStatisticsCsvProcessor energyStatistic = new EnergyStatisticsCsvProcessor(tempPath, new WindowsSIDAccountHelper());

            // Act
            string results = energyStatistic.ProcessCsv(true);

            // Assert
            Assert.IsFalse(File.Exists(tempPath));
        }
    }
}
