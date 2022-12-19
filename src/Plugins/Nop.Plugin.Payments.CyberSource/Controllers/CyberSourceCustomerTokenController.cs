using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.CyberSource.Domain;
using Nop.Plugin.Payments.CyberSource.Models;
using Nop.Plugin.Payments.CyberSource.Services;
using Nop.Plugin.Payments.CyberSource.Services.Helpers;
using Nop.Services.Customers;
using Nop.Web.Controllers;

namespace Nop.Plugin.Payments.CyberSource.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CyberSourceCustomerTokenController : BasePublicController
    {
        #region Fields

        private readonly CustomerTokenService _customerTokenService;
        private readonly CyberSourceService _cyberSourceService;
        private readonly CyberSourceSettings _cyberSourceSettings;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CyberSourceCustomerTokenController(CustomerTokenService customerTokenService,
            CyberSourceService cyberSourceService,
            CyberSourceSettings cyberSourceSettings,
            ICustomerService customerService,
            IWorkContext workContext)
        {
            _customerTokenService = customerTokenService;
            _cyberSourceService = cyberSourceService;
            _cyberSourceSettings = cyberSourceSettings;
            _customerService = customerService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> CustomerTokens()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return RedirectToRoute("CustomerInfo");

            if (!_cyberSourceSettings.TokenizationEnabled)
                return RedirectToRoute("CustomerInfo");

            var model = new CustomerTokenListModel();
            var tokens = await _customerTokenService.GetAllTokensAsync(customerId: customer.Id);
            foreach (var token in tokens)
            {
                var tokenModel = new CustomerTokenListModel.CustomerTokenDetailsModel
                {
                    ThreeDigitCardType = token.ThreeDigitCardType,
                    CardExpirationMonth = token.CardExpirationMonth,
                    CardExpirationYear = token.CardExpirationYear,
                    CardNumber = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX"),
                    Id = token.Id
                };

                model.Tokens.Add(tokenModel);
            }

            return View("~/Plugins/Payments.CyberSource/Views/CustomerToken/CustomerTokens.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> TokenDelete(int tokenId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            //find token (ensure that it belongs to the current customer)
            var token = await _customerTokenService.GetByIdAsync(tokenId);
            if (token != null && token.CustomerId == customer.Id)
            {
                var (_, error) = await _cyberSourceService.DeletePaymentInstrumentAsync(token.SubscriptionId);
                if (!string.IsNullOrEmpty(error))
                    throw new NopException(error);

                await _customerTokenService.DeleteAsync(token);
            }

            //redirect to the token list page
            return Json(new
            {
                redirect = Url.RouteUrl(CyberSourceDefaults.CustomerTokensRouteName)
            });
        }

        public async Task<IActionResult> TokenAdd()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return RedirectToRoute("CustomerInfo");

            if (!_cyberSourceSettings.TokenizationEnabled)
                return RedirectToRoute("CustomerInfo");

            var model = new CustomerTokenEditModel();

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.CustomerToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.CustomerToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            return View("~/Plugins/Payments.CyberSource/Views/CustomerToken/TokenAdd.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> TokenAdd(CustomerTokenEditModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (ModelState.IsValid)
            {
                var cardNumber = CreditCardHelper.RemoveSpecialCharacters(model.CustomerToken.CardNumber);
                var firstSixDigitsOfCard = CreditCardHelper.GetFirstSixDigitsOfCard(cardNumber);
                var lastFourDigitsOfCard = CreditCardHelper.GetLastFourDigitsOfCard(cardNumber);
                var existingTokens = await _customerTokenService.GetAllTokensAsync(customer.Id);
                if (existingTokens.Any(token => token.FirstSixDigitOfCard == firstSixDigitsOfCard && token.LastFourDigitOfCard == lastFourDigitsOfCard))
                    throw new NopException("Token already exists!");

                var (instrumentResult, instrumentError) = await _cyberSourceService.CreateInstrumentIdAsync(cardNumber);
                if (!string.IsNullOrEmpty(instrumentError))
                    throw new NopException(instrumentError);

                var instrumentToken = instrumentResult?.Id;
                if (!string.IsNullOrEmpty(instrumentToken))
                {
                    var (paymentInstrumentResult, paymentInstrumentError) = await _cyberSourceService.CreatePaymentInstrumentAsync(
                        instrumentIdentifier: instrumentToken,
                        cardExpirationMonth: model.CustomerToken.ExpireMonth.PadLeft(2, '0'),
                        cardExpirationYear: model.CustomerToken.ExpireYear,
                        cardType: CreditCardHelper.GetCardTypeByNumber(cardNumber));

                    if (!string.IsNullOrEmpty(paymentInstrumentError))
                        throw new NopException(paymentInstrumentError);

                    if (paymentInstrumentResult != null)
                    {
                        await _customerTokenService.InsertAsync(new CyberSourceCustomerToken
                        {
                            CustomerId = customer.Id,
                            ThreeDigitCardType = CreditCardHelper.GetThreeDigitCardTypeByNumber(model.CustomerToken.CardNumber),
                            CardExpirationMonth = paymentInstrumentResult.Card?.ExpirationMonth,
                            CardExpirationYear = paymentInstrumentResult.Card?.ExpirationYear,
                            FirstSixDigitOfCard = firstSixDigitsOfCard,
                            LastFourDigitOfCard = lastFourDigitsOfCard,
                            InstrumentIdentifier = instrumentToken,
                            InstrumentIdentifierStatus = instrumentResult.State,
                            SubscriptionId = paymentInstrumentResult.Id
                        });
                    }
                }

                return RedirectToRoute(CyberSourceDefaults.CustomerTokensRouteName);
            }

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.CustomerToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.CustomerToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            return View("~/Plugins/Payments.CyberSource/Views/CustomerToken/TokenAdd.cshtml", model);
        }

        public async Task<IActionResult> TokenEdit(int tokenId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!CyberSourceService.IsConfigured(_cyberSourceSettings))
                return RedirectToRoute("CustomerInfo");

            if (!_cyberSourceSettings.TokenizationEnabled)
                return RedirectToRoute("CustomerInfo");

            //find token (ensure that it belongs to the current customer)
            var token = await _customerTokenService.GetByIdAsync(tokenId);
            if (token == null || token.CustomerId != customer.Id)
                return RedirectToRoute(CyberSourceDefaults.CustomerTokensRouteName);

            var model = new CustomerTokenEditModel();

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.CustomerToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.CustomerToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            model.CustomerToken.Id = token.Id;
            model.CustomerToken.CardNumber = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX");
            model.CustomerToken.ExpireMonth = token.CardExpirationMonth;
            var selectedMonth = model.CustomerToken.ExpireMonths.FirstOrDefault(x => x.Value.Equals(model.CustomerToken.ExpireMonth, StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            model.CustomerToken.ExpireYear = token.CardExpirationYear;
            var selectedYear = model.CustomerToken.ExpireYears.FirstOrDefault(x => x.Value.Equals(model.CustomerToken.ExpireYear, StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;

            return View("~/Plugins/Payments.CyberSource/Views/CustomerToken/TokenEdit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> TokenEdit(CustomerTokenEditModel model, int tokenId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            //find token (ensure that it belongs to the current customer)
            var token = await _customerTokenService.GetByIdAsync(tokenId);
            if (token == null || token.CustomerId != customer.Id)
                return RedirectToRoute(CyberSourceDefaults.CustomerTokensRouteName);

            if (ModelState.IsValid)
            {
                var (result, error) = await _cyberSourceService.UpdatePaymentInstrumentAsync(paymentInstrumentTokenId: token.SubscriptionId,
                    instrumentIdentifier: token.InstrumentIdentifier,
                    cardExpirationMonth: model.CustomerToken.ExpireMonth.PadLeft(2, '0'),
                    cardExpirationYear: model.CustomerToken.ExpireYear);
                if (!string.IsNullOrEmpty(error))
                    throw new NopException(error);

                if (result != null)
                {
                    token.CardExpirationMonth = result.Card.ExpirationMonth;
                    token.CardExpirationYear = model.CustomerToken.ExpireYear;
                    await _customerTokenService.UpdateAsync(token);

                    return RedirectToRoute(CyberSourceDefaults.CustomerTokensRouteName);
                }
            }

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                model.CustomerToken.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                model.CustomerToken.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            model.CustomerToken.CardNumber = (token.FirstSixDigitOfCard ?? "XXXXXX") + "XXXXXX" + (token.LastFourDigitOfCard ?? "XXXX");

            return View("~/Plugins/Payments.CyberSource/Views/CustomerToken/TokenEdit.cshtml", model);
        }

        #endregion
    }
}