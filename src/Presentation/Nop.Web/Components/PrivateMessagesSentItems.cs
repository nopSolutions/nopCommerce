using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class PrivateMessagesSentItemsViewComponent : NopViewComponent
    {
        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

        public PrivateMessagesSentItemsViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
        {
            _privateMessagesModelFactory = privateMessagesModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int pageNumber, string tab)
        {
            var model = await _privateMessagesModelFactory.PrepareSentModelAsync(pageNumber, tab);
            return View(model);
        }
    }
}
