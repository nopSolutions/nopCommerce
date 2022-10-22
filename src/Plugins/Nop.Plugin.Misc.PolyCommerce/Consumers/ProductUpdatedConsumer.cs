using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.PolyCommerce.Consumers
{
    public class ProductUpdatedConsumer : IConsumer<EntityUpdatedEvent<Product>>
    {
        private readonly ILogger _logger;
        public ProductUpdatedConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            try
            {
                // Custom handling for UpdatedOnoUtc date. NopCommerce Core does not update UpdatedOnUtc date before/after/within AdjustInventory() method.
                // When orders are inserted into system and inventory is decreased, PolyCommerce requires the UpdatedOnUtc to be set to detect the product. 
                var dataSettings = DataSettingsManager.LoadSettings();

                // directly update entity with ado.net instead of expensive calls using repository to fetch & save entire product
                using (var conn = new SqlConnection(dataSettings.ConnectionString))
                {
                    using (var command = new SqlCommand())
                    {
                        command.CommandText = @"update [dbo].[Product]
                                                set UpdatedOnUtc = @UpdatedOnUtc
                                                where Id = @ProductId";

                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@UpdatedOnUtc", DateTime.UtcNow));
                        command.Parameters.Add(new SqlParameter("@ProductId", eventMessage.Entity.Id));

                        command.Connection = conn;

                        conn.Open();

                        var affectedRecords = command.ExecuteNonQuery();

                        if (affectedRecords != 1)
                        {
                            await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Error, "ProductUpdatedConsumer error", $"Handle product update event failed. Expected 1 affected record from transaction but instead saw {affectedRecords} affected records.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("Handle product update event failed", ex);
            }
        }
    }
}
