using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
    public partial class EfDataProviderManager : BaseDataProviderManager
    {
        public EfDataProviderManager(DataSettings settings):base(settings)
        {
        }

        public override IDataProvider LoadDataProvider()
        {

            var providerName = Settings.DataProvider;
            if (string.IsNullOrWhiteSpace(providerName))
                throw new NopException("Data Settings doesn't contain a providerName");

            switch (providerName.ToLowerInvariant())
            {
                case "sqlserver":
                    return new SqlServerDataProvider(Settings.DataConnectionString);
                case "sqlite":
                    return new SqliteDataProvider(Settings.DataConnectionString);
                case "mysql":
                    return new MySqlDataProvider(Settings.DataConnectionString);
                case "npgsql":
                    return new NpgSqlDataProvider(Settings.DataConnectionString);
                default:
                    throw new NopException($"Not supported dataprovider name: {providerName}");
            }
        }
    }
}
