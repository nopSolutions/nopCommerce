using LinqToDB.Configuration;

namespace Nop.Core.Data
{
    /// <summary>
    /// Connection string configuration provider
    /// </summary>
    public partial class ConnectionStringConfigurationProvider : IConnectionStringSettings
    {
        /// <summary>
        /// Gets or sets connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets connection configuration name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets data provider configuration name
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Is this connection configuration defined on global level (machine.config) or on application level.
        /// </summary>
        public bool IsGlobal => false;
    }
}