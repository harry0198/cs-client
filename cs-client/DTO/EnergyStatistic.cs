using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_client.DTO
{
    public class EnergyStatistic
    {
        [JsonProperty(PropertyName = "appId")]
        public string AppId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "cpuEnergyConsumption")]
        public long CPUEnergyConsumption { get; set; }

        [JsonProperty(PropertyName = "socEnergyConsumption")]
        public long SOCEnergyConsumption { get; set; }

        [JsonProperty(PropertyName = "displayEnergyConsumption")]
        public long DisplayEnergyConsumption { get; set; }

        [JsonProperty(PropertyName = "diskEnergyConsumption")]
        public long DiskEnergyConsumpution { get; set; }

        [JsonProperty(PropertyName = "networkEnergyConsumption")]
        public long NetworkEnergyConsumption { get; set; }

        [JsonProperty(PropertyName = "mbbEnergyConsumption")]
        public long MBBEnergyConsumption { get; set; }

        [JsonProperty(PropertyName = "otherEnergyConsumption")]
        public long OtherEnergyConsumption { get; set; }

        [JsonProperty(PropertyName = "emiEnergyConsumption")]
        public long EMIEnergyConsumption { get; set; }
    }
}
