
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Common;
using Nop.Core;
using Nop.Services.Localization;

namespace Nop.Admin.Components
{
    public class AdminLanguageSelectorViewComponent : ViewComponent
    {
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public AdminLanguageSelectorViewComponent(ILanguageService languageService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            this._languageService = languageService;
            this._storeContext = storeContext;
            this._workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService
                .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                .Select(x => x.ToModel())
                .ToList();

            return View(model);
        }
    }
}
