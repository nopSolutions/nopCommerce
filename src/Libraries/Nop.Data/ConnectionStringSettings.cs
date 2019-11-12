using LinqToDB.Configuration;
using Nop.Data.Data;

namespace Nop.Data
{
    /// <summary>
    /// Connection string configuration provider
    /// </summary>
    public partial class ConnectionStringSettings : IConnectionStringSettings
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
        public string ProviderName => ProviderType.ToString();

        /// <summary>
        /// Gets or sets data provider configuration name
        /// </summary>
        public DataProviderType ProviderType { get; set; }

        /// <summary>
        /// Is this connection configuration defined on global level (machine.config) or on application level.
        /// </summary>
        public bool IsGlobal => false;
    }
}