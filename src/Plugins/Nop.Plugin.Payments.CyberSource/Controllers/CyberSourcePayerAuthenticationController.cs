using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.CyberSource.Services;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Controllers;

namespace Nop.Plugin.Payments.CyberSource.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CyberSourcePayerAuthenticationController : BasePublicController
    {
        #region Fields

        private readonly CustomerTokenService _customerTokenService;
        private readonly CyberSourceService _cyberSourceService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CyberSourcePayerAuthenticationController(CustomerTokenService customerTokenService,
            CyberSourceService cyberSourceService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderTotalCalculationService orderTotalCalculationService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _customerTokenService = customerTokenService;
            _cyberSourceService = cyberSourceService;
            _localizationService = localizationService;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Setup(int customerTokenId, string transientToken)
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PayerAuthentication.Fail");
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerToken = await _customerTokenService.GetByIdAsync(customerTokenId);
            if (string.IsNullOrEmpty(transientToken) && (customerToken is null || customerToken.CustomerId != customer.Id))
                return Json(new { success = false, message = error });

            var (result, _) = await _cyberSourceService.PayerAuthenticationSetupAsync(customerToken, transientToken);
            if (result?.ConsumerAuthenticationInformation is null || result.Status == CyberSourceDefaults.PayerAuthenticationSetupStatus.Failed)
            {
                var message = $"Payer authentication setup failed. {result?.ErrorInformation?.Message}";
                await _logger.ErrorAsync($"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}", customer: customer);

                return Json(new { success = false, message = error });
            }

            return Json(new
            {
                success = true,
                accessToken = result.ConsumerAuthenticationInformation.AccessToken,
                deviceDataCollectionUrl = result.ConsumerAuthenticationInformation.DeviceDataCollectionUrl,
                referenceId = result.ConsumerAuthenticationInformation.ReferenceId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Enrollment(string referenceId, int customerTokenId, string transientToken)
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PayerAuthentication.Fail");
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerToken = await _customerTokenService.GetByIdAsync(customerTokenId);
            if (string.IsNullOrEmpty(transientToken) && (customerToken is null || customerToken.CustomerId != customer.Id))
                return Json(new { success = false, message = error });

            var store = await _storeContext.GetCurrentStoreAsync();
            var returnUrl = $"{store.Url.TrimEnd('/')}{Url.RouteUrl(CyberSourceDefaults.PayerRedirectRouteName)}".ToLowerInvariant();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var (shoppingCartTotal, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
            var request = new ProcessPaymentRequest
            {
                OrderGuid = Guid.NewGuid(),
                CustomerId = customer.Id,
                OrderTotal = shoppingCartTotal ?? decimal.Zero
            };
            var (result, _) = await _cyberSourceService.PayerAuthenticationEnrollmentAsync(referenceId, customerToken, transientToken, request, returnUrl);
            if (result?.ConsumerAuthenticationInformation is null || result.Status == CyberSourceDefaults.PayerAuthenticationStatus.Failed)
            {
                var message = $"Payer authentication enrollment failed. {result?.ErrorInformation?.Message}";
                await _logger.ErrorAsync($"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}", customer: customer);

                if (result?.ConsumerAuthenticationInformation is not null && 
                    result.ErrorInformation?.Reason == CyberSourceDefaults.PayerAuthenticationErrorReason.ConsumerAuthenticationRequired)
                {
                    return Json(new
                    {
                        success = true,
                        complete = false,
                        authenticationTransactionId = result.ConsumerAuthenticationInformation.AuthenticationTransactionId,
                        accessToken = result.ConsumerAuthenticationInformation.AccessToken,
                        stepUpUrl = result.ConsumerAuthenticationInformation.StepUpUrl
                    });
                }

                return Json(new { success = false, message = result?.ConsumerAuthenticationInformation?.CardholderMessage ?? error });
            }

            if (result.Status == CyberSourceDefaults.PayerAuthenticationStatus.Pending ||
                result.ConsumerAuthenticationInformation.ChallengeRequired == "Y")
            {
                return Json(new
                {
                    success = true,
                    complete = false,
                    authenticationTransactionId = result.ConsumerAuthenticationInformation.AuthenticationTransactionId,
                    accessToken = result.ConsumerAuthenticationInformation.AccessToken,
                    stepUpUrl = result.ConsumerAuthenticationInformation.StepUpUrl
                });
            }

            if (result.Status == CyberSourceDefaults.PayerAuthenticationStatus.Success)
            {
                return Json(new
                {
                    success = true,
                    complete = true,
                    authenticationTransactionId = result.ConsumerAuthenticationInformation.AuthenticationTransactionId
                });
            }

            return Json(new { success = false, message = error });
        }

        [HttpPost]
        public async Task<IActionResult> Validate(string authenticationTransactionId)
        {
            var error = await _localizationService.GetResourceAsync("Plugins.Payments.CyberSource.PayerAuthentication.Fail");
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (string.IsNullOrEmpty(authenticationTransactionId))
                return Json(new { success = false, message = error });

            var (result, _) = await _cyberSourceService.PayerAuthenticationValidateAsync(authenticationTransactionId);
            if (result?.Status != CyberSourceDefaults.PayerAuthenticationStatus.Success)
            {
                var message = $"Payer authentication validation failed. {result?.ErrorInformation?.Message}";
                await _logger.ErrorAsync($"{CyberSourceDefaults.SystemName} error: {Environment.NewLine}{message}", customer: customer);

                return Json(new { success = false, message = error });
            }

            return Json(new { success = true });
        }

        #endregion
    }
}