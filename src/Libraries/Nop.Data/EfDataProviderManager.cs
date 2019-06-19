using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    /// <summary>
    /// Represents the Entity Framework data provider manager
    /// </summary>
    public partial class EfDataProviderManager : IDataProviderManager
    {
        private readonly DataSettings _dataSettings;

        public EfDataProviderManager(IOptions<DataSettings> dataSettings)
        {
            _dataSettings = dataSettings.Value;
        }

        /// <summary>
        /// Gets data provider
        /// </summary>
        public IDataProvider DataProvider
        {
            get
            {
                var providerName = _dataSettings.DataProvider;
                switch (providerName)
                {
                    case DataProviderType.SqlServer:
                        return new SqlServerDataProvider();

                    default:
                        throw new NopException($"Not supported data provider name: '{providerName}'");
                }
            }
        }
    }
}