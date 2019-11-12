using Nop.Core;
using Nop.Data.Data;

namespace Nop.Data
{
    /// <summary>
    /// Represents the Entity Framework data provider manager
    /// </summary>
    public partial class DataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public IDataProvider DataProvider
        {
            get
            {
                var dataConnection = new NopDataConnection();

                switch (dataConnection.ProviderType)
                {
                    case DataProviderType.SqlServer:
                        return new MsSqlDataProvider(dataConnection);
                    default:
                        throw new NopException($"Not supported data provider name: '{dataConnection.DataProvider}'");
                }
            }
        }

        #endregion
    }
}
