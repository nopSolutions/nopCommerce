using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Customers;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Nop.Services.Security;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class CustomerRoleController : BaseNopController
	{
		#region Fields

		private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

		#endregion

		#region Constructors

        public CustomerRoleController(ICustomerService customerService,
            ILocalizationService localizationService, ICustomerActivityService customerActivityService,
            IPermissionService permissionService)
		{
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
		}

		#endregion 

		#region Customer roles

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
			var customerRoles = _customerService.GetAllCustomerRoles(true);
			var gridModel = new GridModel<CustomerRoleModel>
			{
                Data = customerRoles.Select(x => x.ToModel()),
                Total = customerRoles.Count()
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            var gridModel = new GridModel<CustomerRoleModel>
			{
                Data = customerRoles.Select(x => x.ToModel()),
                Total = customerRoles.Count()
			};
			return new JsonResult
			{
				Data = gridModel
			};
		}

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
            var model = new CustomerRoleModel();
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity();
                _customerService.InsertCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerRole", _localizationService.GetResource("ActivityLog.AddNewCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null) throw new ArgumentException("No customer role found with the specified id", "id");
            return View(customerRole.ToModel());
		}

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(model.Id);
            if (customerRole == null) 
                throw new ArgumentException("No customer role found with the specified id");

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));


                    customerRole = model.ToEntity(customerRole);
                    _customerService.UpdateCustomerRole(customerRole);

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomerRole", _localizationService.GetResource("ActivityLog.EditCustomerRole"), customerRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", customerRole.Id) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomerRoles))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                throw new ArgumentException("No customer role found with the specified id");

            try
            {
                _customerService.DeleteCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }

		}

		#endregion
    }
}
