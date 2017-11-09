using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.Worldpay.Domain;
using Nop.Plugin.Payments.Worldpay.Domain.Enums;
using Nop.Plugin.Payments.Worldpay.Domain.Models;
using Nop.Plugin.Payments.Worldpay.Domain.Requests;
using Nop.Plugin.Payments.Worldpay.Models;
using Nop.Plugin.Payments.Worldpay.Models.Customer;
using Nop.Plugin.Payments.Worldpay.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Worldpay.Controllers
{
    public class PaymentWorldpayController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly WorldpayPaymentManager _worldpayPaymentManager;
        private readonly WorldpayPaymentSettings _worldpayPaymentSettings;

        #endregion

        #region Ctor

        public PaymentWorldpayController(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IWorkContext workContext,
            WorldpayPaymentManager worldpayPaymentManager,
            WorldpayPaymentSettings worldpayPaymentSettings)
        {
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._workContext = workContext;
            this._worldpayPaymentManager = worldpayPaymentManager;
            this._worldpayPaymentSettings = worldpayPaymentSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = new ConfigurationModel
            {
                SecureNetId = _worldpayPaymentSettings.SecureNetId,
                SecureKey = _worldpayPaymentSettings.SecureKey,
                PublicKey = _worldpayPaymentSettings.PublicKey,
                TransactionModeId = (int)_worldpayPaymentSettings.TransactionMode,
                TransactionModes = _worldpayPaymentSettings.TransactionMode.ToSelectList(),
                UseSandbox = _worldpayPaymentSettings.UseSandbox,
                ValidateAddress = _worldpayPaymentSettings.ValidateAddress,
                AdditionalFee = _worldpayPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = _worldpayPaymentSettings.AdditionalFeePercentage
            };

            return View("~/Plugins/Payments.Worldpay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _worldpayPaymentSettings.SecureNetId = model.SecureNetId;
            _worldpayPaymentSettings.SecureKey = model.SecureKey;
            _worldpayPaymentSettings.PublicKey = model.PublicKey;
            _worldpayPaymentSettings.TransactionMode = (TransactionMode)model.TransactionModeId;
            _worldpayPaymentSettings.UseSandbox = model.UseSandbox;
            _worldpayPaymentSettings.ValidateAddress = model.ValidateAddress;
            _worldpayPaymentSettings.AdditionalFee = model.AdditionalFee;
            _worldpayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            _settingService.SaveSetting(_worldpayPaymentSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult CreateUpdateCustomer(int customerId, string worldpayCustomerId)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //check whether customer exists
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", nameof(customerId));

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            //ensure that customer identifier not exceed 25 characters
            if ((worldpayCustomerId?.Length ?? 0) > 25)
                throw new NopException("Worldpay Vault error: Customer ID must not exceed 25 characters");

            //create request parameters to store a customer in Vault
            var createCustomerRequest = new CreateCustomerRequest
            {
                CustomerId = worldpayCustomerId,
                CustomerDuplicateCheckType = CustomerDuplicateCheckType.Error,
                EmailReceiptEnabled = !string.IsNullOrEmpty(customer.Email),
                Email = customer.Email,
                FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                BillingAddress = new Address
                {
                    Line1 = customer.BillingAddress?.Address1,
                    City = customer.BillingAddress?.City,
                    State = customer.BillingAddress?.StateProvince?.Abbreviation,
                    Country = customer.BillingAddress?.Country?.TwoLetterIsoCode,
                    Zip = customer.BillingAddress?.ZipPostalCode,
                    Company = customer.BillingAddress?.Company,
                    Phone = customer.BillingAddress?.PhoneNumber
                }
            };

            //check whether customer is already stored in the Vault and try to store, if it is not so
            var vaultCustomer = _worldpayPaymentManager.GetCustomer(customer.GetAttribute<string>(WorldpayPaymentDefaults.CustomerIdAttribute))
                ?? _worldpayPaymentManager.CreateCustomer(createCustomerRequest);

            if (vaultCustomer == null)
                throw new NopException("Worldpay Vault error: Failed to create customer. Error details in the log");

            //save Vault customer identifier as a generic attribute
            _genericAttributeService.SaveAttribute(customer, WorldpayPaymentDefaults.CustomerIdAttribute, vaultCustomer.CustomerId);
            
            //selected tab
            SaveSelectedTabName();

            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult CardList(int customerId)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            //check whether customer exists
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", nameof(customerId));
            
            //try to get stored credit cards of the Vault customer
            var storedCards = _worldpayPaymentManager.GetCustomer(customer.GetAttribute<string>(WorldpayPaymentDefaults.CustomerIdAttribute))?
                .PaymentMethods?.Where(method => method?.Card != null).ToList() ?? new List<PaymentMethod>();

            //prepare grid model
            var gridModel = new DataSourceResult
            {
                Data = storedCards.Select(card => new WorldpayCardModel
                {
                    Id = card.PaymentId,
                    CardId = card.PaymentId,
                    CardType = card.Card.CreditCardType.HasValue ? card.Card.CreditCardType.Value.GetLocalizedEnum(_localizationService, _workContext) : null,
                    ExpirationDate = card.Card.ExpirationDate,
                    MaskedNumber = card.Card.MaskedNumber
                }),
                Total = storedCards.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult CardDelete(string id, int customerId)
        {
            //whether user has the authority
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //check whether customer exists
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", nameof(customerId));

            //try to delere selected card from the Vault
            var deleted = _worldpayPaymentManager.Deletecard(new DeleteCardRequest
            {
                CustomerId = customer.GetAttribute<string>(WorldpayPaymentDefaults.CustomerIdAttribute),
                PaymentId = id
            });
            if (!deleted)
                throw new NopException("Worldpay Vault error: Failed to delete card. Error details in the log");

            return new NullJsonResult();
        }

        #endregion
    }
}