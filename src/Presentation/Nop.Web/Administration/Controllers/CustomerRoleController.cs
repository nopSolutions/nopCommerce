using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Localization;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Admin.Models;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using Nop.Web.Framework;
using Telerik.Web.Mvc.Extensions;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class CustomerRoleController : BaseNopController
	{
		#region Fields

		private readonly ICustomerService _customerService;

		#endregion Fields 

		#region Constructors

        public CustomerRoleController(ICustomerService customerService)
		{
            this._customerService = customerService;
		}

		#endregion Constructors 

		#region Customer roles

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
		{
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
            return View(new CustomerRoleModel());
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CustomerRoleModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity();
                _customerService.InsertCustomerRole(customerRole);
                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		public ActionResult Edit(int id)
		{
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null) throw new ArgumentException("No customer role found with the specified id", "id");
            return View(customerRole.ToModel());
		}

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CustomerRoleModel model, bool continueEditing)
        {
            var customerRole = _customerService.GetCustomerRoleById(model.Id);
            if (ModelState.IsValid)
            {
                customerRole = model.ToEntity(customerRole);
                _customerService.UpdateCustomerRole(customerRole);
                return continueEditing ? RedirectToAction("Edit", customerRole.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
		{
            //TODO display warning when customer role could not be deleted (system role)
            var customerRole = _customerService.GetCustomerRoleById(id);
            _customerService.DeleteCustomerRole(customerRole);
			return RedirectToAction("List");
		}

		#endregion
    }
}
