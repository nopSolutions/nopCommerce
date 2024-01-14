using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingEventConsumer : IConsumer<EntityUpdatedEvent<Core.Domain.Catalog.Product>>, IConsumer<EntityDeletedEvent<Core.Domain.Catalog.Product>>
    {
        private readonly ILogger _logger;
        private readonly POSKachingSettings _kachingSettings;
        private readonly IPOSKachingService _poskachingService;
        private bool _hasBeenSentToKaching = false;

        public POSKachingEventConsumer(ILogger logger, IPOSKachingService poskachingService, POSKachingSettings kachingSettings)
        {
            _logger = logger;
            _kachingSettings = kachingSettings;
            _poskachingService = poskachingService;
            _hasBeenSentToKaching = false;
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Core.Domain.Catalog.Product> eventMessage)
        {
            if (_hasBeenSentToKaching)
            {
                return;
            }

            if (_kachingSettings.POSKaChingActive == false)
            {
                return;
            }

            string json = "";
            var debugInfo = new StringBuilder("Kaching Plugin information:");
            try
            {
                debugInfo.AppendLine($"Fetching product in EntityUpdatedEvent (Kaching)");
                var product = eventMessage.Entity;
                if (product == null)
                {
                    await _logger.ErrorAsync($"HandleEvent (Product Updated Event), no product found in EventMessage");
                    return;
                }

                debugInfo.AppendLine($"Product '{product.Name}' is published: '{product.Published}' in EntityUpdatedEvent (Kaching)");
                if (product.Published)
                {
                    json = await _poskachingService.BuildJSONStringAsync(product);
                    if (string.IsNullOrEmpty(json))
                    {
                        await _logger.ErrorAsync($"HandleEvent (Product Updated Event), Json is null or empty on product: '{product.Name}' ({product.Id})");
                        return;
                    }

                    await _poskachingService.SaveProductAsync(json);
                }
                else
                {
                    await DeleteProductFromKaching(product);
                    debugInfo.AppendLine($"Product: '{product.Id}' is deleted from Kaching in EntityUpdatedEvent (Kaching)");
                }

                _hasBeenSentToKaching = true;
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;

                await _logger.ErrorAsync($"HandleEvent (Product Updated Event) POS Kaching: {inner.Message}{Environment.NewLine}Json sent to kaching on this error:{Environment.NewLine}'{json}'", ex);

                debugInfo.AppendLine($"Host: '{_kachingSettings.POSKaChingHost}'");
                debugInfo.AppendLine($"Queue: '{_kachingSettings.POSKaChingImportQueueName}'");
                debugInfo.AppendLine($"Kaching Id: '{_kachingSettings.POSKaChingId}'");
                debugInfo.AppendLine($"Token: '{_kachingSettings.POSKaChingAccountToken}'");
                debugInfo.AppendLine($"API Token: '{_kachingSettings.POSKaChingAPIToken}'");
                await _logger.InformationAsync(debugInfo.ToString());
            }
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Core.Domain.Catalog.Product> eventMessage)
        {
            if (_hasBeenSentToKaching)
            {
                return;
            }

            if (_kachingSettings.POSKaChingActive == false)
            {
                return;
            }

            try
            {
                var product = eventMessage.Entity;
                await DeleteProductFromKaching(product);
                _hasBeenSentToKaching = true;
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                await _logger.ErrorAsync($"HandleEvent (Product Deleted Event) POS Kaching: {inner.Message}", ex);
            }
        }

        private async Task DeleteProductFromKaching(Core.Domain.Catalog.Product product)
        {
            string[] ids = new string[1];
            ids[0] = product.Id.ToString();
            await _poskachingService.DeleteProductAsync(ids);
        }
    }
}