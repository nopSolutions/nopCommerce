using System.Net.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.PolyCommerce.Services
{
    public class EventConsumer : IConsumer<EntityDeletedEvent<Product>>
    {
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;

        public EventConsumer(IStoreContext storeContext, ILogger logger)
        {
            _storeContext = storeContext;
            _logger = logger;

        }
        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {

            if (eventMessage.Entity == null)
            {
                return;
            }

            var storeId = _storeContext.CurrentStore.Id;

            var token = PolyCommerceHelper.GetTokenByStoreId(storeId);

            using (var client = new HttpClient())
            {
                // get short lived token for user..
                var content = new StringContent(string.Empty);

                client.DefaultRequestHeaders.Add("Token", token);

                client.PostAsync($"{PolyCommerceHelper.GetBaseUrl()}/api/hooks/nopcommerce/delete_product/{eventMessage.Entity.Id}", content).GetAwaiter().GetResult();
            }
        }
    }
}
