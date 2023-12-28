namespace CsClient.Statistic
{
    /// <summary>
    /// Headers for the CSV sent to the cs-server by this (cs-client).
    /// </summary>
    public class CsClientCsvHeaders
    {
        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string AppId = "app-id";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string UserId = "user-id";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string TimeStamp = "timestamp";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string CPUEnergyConsumption = "cpu";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string SocEnergyConsumption = "soc";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string DisplayEnergyConsumption = "display";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string DiskEnergyConsumption = "disk";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string NetworkEnergyConsumption = "network";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string MBBEnergyConsumption = "mbb";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string OtherEnergyConsumption = "other";

        /// <summary>
        /// Header in the energy usage generated csv file.
        /// </summary>
        public static readonly string EmiEnergyConsumption = "emi";
    }
}
