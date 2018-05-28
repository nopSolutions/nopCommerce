using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// Represents the Entity Framework data provider manager
    /// </summary>
    public partial class EfDataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public IDataProvider DataProvider
        {
            get
            {
                var providerName = DataSettingsManager.LoadSettings()?.DataProvider;
                switch (providerName)
                {
                    case DataProviderType.SqlServer:
                        return new SqlServerDataProvider();

                    //starting version 4.10 we support MS SQL Server only. SQL Server Compact is not supported anymore
                    //but we leave this code because we plan to support other databases soon (e.g. MySQL)

                    //case "sqlce":
                    //    return new SqlCeDataProvider();

                    default:
                        throw new NopException($"Not supported data provider name: '{providerName}'");
                }
            }
        }

        #endregion
    }
}