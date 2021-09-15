namespace Nop.Data
{
    /// <summary>
    /// Represents default values related to data settings
    /// </summary>
    public static partial class NopDataSettingsDefaults
    {
        /// <summary>
        /// Gets a path to the file that was used in old nopCommerce versions to contain data settings
        /// </summary>
        public static string ObsoleteFilePath => "~/App_Data/Settings.txt";

        /// <summary>
        /// Gets a path to the file that contains data settings
        /// </summary>
        public static string FilePath => "~/App_Data/dataSettings.json";

        /// <summary>
        /// Gets the name of an environment variable with connection string settings
        /// </summary>
        public static string EnvironmentVariableDataConnectionString => "dataSettings__DataConnectionString";

        /// <summary>
        /// Gets the name of an environment variable with data provider settings
        /// </summary>
        public static string EnvironmentVariableDataProvider => "dataSettings__DataProvider";

        /// <summary>
        /// Gets the name of an environment variable with SQL command timеimeout settings
        /// </summary>
        public static string EnvironmentVariableSQLCommandTimeout => "dataSettings__SQLCommandTimeout";
    }
}