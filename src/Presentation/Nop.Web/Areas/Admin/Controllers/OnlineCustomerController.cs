using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class OnlineCustomerController : BaseAdminController
    {
        #region Fields

        protected ICustomerModelFactory CustomerModelFactory { get; }
        protected IPermissionService PermissionService { get; }

        #endregion

        #region Ctor

        public OnlineCustomerController(ICustomerModelFactory customerModelFactory,
            IPermissionService permissionService)
        {
            CustomerModelFactory = customerModelFactory;
            PermissionService = permissionService;
        }

        #endregion
        
        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerModelFactory.PrepareOnlineCustomerSearchModelAsync(new OnlineCustomerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(OnlineCustomerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CustomerModelFactory.PrepareOnlineCustomerListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
    }
}