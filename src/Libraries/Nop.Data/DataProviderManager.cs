using Nop.Core;
using Nop.Core.Infrastructure;

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
                var dataProviderType = Singleton<DataSettings>.Instance.DataProvider;

                switch (dataProviderType)
                {
                    case DataProviderType.SqlServer:
                        return new MsSqlDataProvider();
                    default:
                        throw new NopException($"Not supported data provider name: '{dataProviderType}'");
                }
            }
        }

        #endregion
    }
}
