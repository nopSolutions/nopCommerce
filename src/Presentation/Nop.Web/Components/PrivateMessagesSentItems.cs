using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class PrivateMessagesSentItemsViewComponent : ViewComponent
    {
        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

        public PrivateMessagesSentItemsViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
        {
            this._privateMessagesModelFactory = privateMessagesModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int page, string tab)
        {
            var model = _privateMessagesModelFactory.PrepareSentModel(page, tab);
            return View(model);
        }
    }
}
