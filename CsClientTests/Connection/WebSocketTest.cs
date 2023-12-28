
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsClient.Statistic;
using System.Collections.Generic;
using CsClient.Data.DTO;
using System.IO;

namespace CsClientTests.Connection
{
    [TestClass]
    public class WebSocketTest : BaseTest
    {
        private EnergyStatistic energyStatistic;

        [TestInitialize]
        public void Setup()
        {
            this.energyStatistic = new EnergyStatistic();
        }


        [TestMethod]
        public async Task ParseMicrosoftEnergyXmlFile()
        {
            string resultPath = Path.Combine(TestDir, "srumutil.xml");
            List<EnergyStatisticDTO> results = await energyStatistic.ParseResults(resultPath);

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            
            EnergyStatisticDTO result1 = results[0];
            EnergyStatisticDTO result2 = results[1];

            Assert.AreEqual("app", result1.AppId);
            Assert.AreEqual("user", result1.UserId);
            Assert.AreEqual(5, result1.CPUEnergyConsumption);
            Assert.AreEqual(38, result1.DiskEnergyConsumpution);
            Assert.AreEqual(20, result1.DisplayEnergyConsumption);
            Assert.AreEqual(0, result1.EMIEnergyConsumption);
            Assert.AreEqual(40, result1.NetworkEnergyConsumption);
            Assert.AreEqual(50, result1.MBBEnergyConsumption);
            Assert.AreEqual(60, result1.OtherEnergyConsumption);
            Assert.AreEqual(10, result1.SOCEnergyConsumption);
            Assert.AreEqual(315535707947000, result1.Timestamp);

            Assert.AreEqual("app", result2.AppId);
            Assert.AreEqual("user", result2.UserId);
            Assert.AreEqual(1, result2.CPUEnergyConsumption);
            Assert.AreEqual(4, result2.DiskEnergyConsumpution);
            Assert.AreEqual(3, result2.DisplayEnergyConsumption);
            Assert.AreEqual(0, result2.EMIEnergyConsumption);
            Assert.AreEqual(5, result2.NetworkEnergyConsumption);
            Assert.AreEqual(6, result2.MBBEnergyConsumption);
            Assert.AreEqual(7, result2.OtherEnergyConsumption);
            Assert.AreEqual(2, result2.SOCEnergyConsumption);
            Assert.AreEqual(315535707947000, result2.Timestamp);
        }
    }
}
