using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class PrivateMessagesInboxViewComponent : ViewComponent
    {
        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;

        public PrivateMessagesInboxViewComponent(IPrivateMessagesModelFactory privateMessagesModelFactory)
        {
            this._privateMessagesModelFactory = privateMessagesModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int page, string tab)
        {
            var model = _privateMessagesModelFactory.PrepareInboxModel(page, tab);
            return View(model);
        }
    }
}
