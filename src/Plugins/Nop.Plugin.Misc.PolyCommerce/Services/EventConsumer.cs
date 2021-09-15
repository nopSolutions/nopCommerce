using System.Net.Http;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.PolyCommerce.Services
{
    public class EventConsumer : IConsumer<EntityDeletedEvent<Product>>
    {
        private readonly IStoreContext _storeContext;

        public EventConsumer(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {

            if (eventMessage.Entity == null)
            {
                return;
            }

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var storeId = currentStore.Id;

            var token = PolyCommerceHelper.GetTokenByStoreId(storeId);

            using (var client = new HttpClient())
            {
                // get short lived token for user..
                var content = new StringContent(string.Empty);

                client.DefaultRequestHeaders.Add("Token", token);

                await client.PostAsync($"{PolyCommerceHelper.GetBaseUrl()}/api/hooks/nopcommerce/delete_product/{eventMessage.Entity.Id}", content);
            }
        }
    }
}
