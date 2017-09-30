using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class StoreScopeConfigurationViewComponent : NopViewComponent
    {
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        public StoreScopeConfigurationViewComponent(IStoreService storeService, IWorkContext workContext)
        {
            this._storeService = storeService;
            this._workContext = workContext;
        }

        public IViewComponentResult Invoke()
        {
            var allStores = _storeService.GetAllStores();
            if (allStores.Count < 2)
                return Content("");

            var model = new StoreScopeConfigurationModel();
            foreach (var s in allStores)
            {
                model.Stores.Add(new StoreModel
                {
                    Id = s.Id,
                    Name = s.Name
                });
            }
            model.StoreId = GetActiveStoreScopeConfiguration(_storeService, _workContext);

            return View(model);
        }

        private int GetActiveStoreScopeConfiguration(IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return 0;

            var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);

            return store != null ? store.Id : 0;
        }
    }
}
