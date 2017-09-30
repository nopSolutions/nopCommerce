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
            this._privateMessagesModelFactory = privateMessagesModelFactory;
        }

        public IViewComponentResult Invoke(int page, string tab)
        {
            var model = _privateMessagesModelFactory.PrepareSentModel(page, tab);
            return View(model);
        }
    }
}
