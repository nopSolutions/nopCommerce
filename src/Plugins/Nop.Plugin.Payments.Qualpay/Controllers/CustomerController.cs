using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Qualpay.Factories;
using Nop.Plugin.Payments.Qualpay.Models.Customer;
using Nop.Plugin.Payments.Qualpay.Services;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Qualpay.Controllers
{
    public class CustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly QualpayCustomerModelFactory _qualpayCustomerModelFactory;
        private readonly QualpayManager _qualpayManager;

        #endregion

        #region Ctor

        public CustomerController(ICustomerService customerService,
            IPermissionService permissionService,
            QualpayCustomerModelFactory qualpayCustomerModelFactory,
            QualpayManager qualpayManager)
        {
            _customerService = customerService;
            _permissionService = permissionService;
            _qualpayCustomerModelFactory = qualpayCustomerModelFactory;
            _qualpayManager = qualpayManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult CreateQualpayCustomer(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(customerId)
                ?? throw new ArgumentException("No customer found with the specified id", nameof(customerId));

            //check whether customer is already exists in the Vault and try to create new one if does not exist
            var vaultCustomer = _qualpayManager.GetCustomer(customer.Id) ?? _qualpayManager.CreateCustomer(customer)
                ?? throw new NopException("Qualpay Customer Vault error: Failed to create customer. Error details in the log");

            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult QualpayCustomerCardList(QualpayCustomerCardSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _qualpayCustomerModelFactory.PrepareQualpayCustomerCardListModel(searchModel, customer);

            return Json(model);
        }

        [HttpPost]
        public IActionResult QualpayCustomerCardDelete(string id, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //check whether customer exists
            var customer = _customerService.GetCustomerById(customerId)
                ?? throw new ArgumentException("No customer found with the specified id", nameof(customerId));

            //try to delete selected card
            if (!_qualpayManager.DeleteCustomerCard(customer.Id, id))
                throw new NopException("Qualpay Customer Vault error: Failed to delete card. Error details in the log");

            return new NullJsonResult();
        }

        #endregion
    }
}