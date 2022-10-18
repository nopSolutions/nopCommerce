using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;

namespace Nop.Plugin.Misc.PolyCommerce
{
    public static class PolyCommerceHelper
    {
        //public static string GetBaseUrl() => "https://localhost:44367";
        public static string GetBaseUrl() => "https://portal.polycommerce.com";

        public static async Task<PolyCommerceStore> GetPolyCommerceStoreByToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var dataSettings = DataSettingsManager.LoadSettings();

            using (var conn = new SqlConnection(dataSettings.ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = @"select Id, 
                                            Token,
                                            StoreId,
                                            IsActive,
                                            CreatedOnUtc
                                            from [dbo].[PolyCommerceStore]
                                            where Token = @Token";

                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Token", token));

                    command.Connection = conn;

                    await conn.OpenAsync();

                    var reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    while(await reader.ReadAsync())
                    {
                        var polyCommerceStore = new PolyCommerceStore
                        {
                            Id = reader.GetInt32(0),
                            Token = reader.GetString(1),
                            StoreId = reader.GetInt32(2),
                            IsActive = reader.GetBoolean(3),
                            CreatedOnUtc = reader.GetDateTime(4)
                        };

                        return polyCommerceStore;
                    }

                    return null;
                }
            }
        }

        public static string GetTokenByStoreId(int storeId)
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            using (var conn = new SqlConnection(dataSettings.ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = @"select 
                                            Token
                                            from[dbo].[PolyCommerceStore]
                                            where StoreId = @StoreId";

                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@StoreId", storeId));

                    command.Connection = conn;

                    conn.Open();

                    var result = command.ExecuteScalar()?.ToString();

                    return result;
                }
            }
        }
    }
}
