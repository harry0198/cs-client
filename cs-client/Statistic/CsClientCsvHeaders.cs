namespace CsClient.Statistic
{
    /// <summary>
    /// Headers for the CSV sent to the cs-server by this (cs-client).
    /// </summary>
    public static class CsClientCsvHeaders
    {
        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string CPU = "cpu";
        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string Memory = "mem";
        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string Disk = "disk";
        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string Network = "nwk";
        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string AppId = "app-id";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string UserId = "user-id";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string TimeStamp = "timestamp";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string CPUEnergyConsumption = "cpu";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string SocEnergyConsumption = "soc";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string DisplayEnergyConsumption = "display";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string DiskEnergyConsumption = "disk";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string NetworkEnergyConsumption = "network";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string MBBEnergyConsumption = "mbb";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string OtherEnergyConsumption = "other";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string EmiEnergyConsumption = "emi";

        /// <summary>
        /// Header of the property that the server accepts.
        /// </summary>
        public const string AccountType = "acc-type";
    }
}
