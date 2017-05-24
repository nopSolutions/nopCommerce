using Microsoft.AspNetCore.Mvc;
using Nop.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;

namespace Nop.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public HomeController(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings, 
            ISettingService settingService,
            IWorkContext workContext)
        {
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._workContext = workContext;
        }
        
        #endregion
        
        #region Methods

        public virtual IActionResult Index()
        {
            var model = new DashboardModel();
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            return View(model);
        }
#if NET451
        [HttpPost]
        public virtual IActionResult NopCommerceNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_adminAreaSettings);
            return Content("Setting changed");
        }
#endif

        #endregion
    }
}