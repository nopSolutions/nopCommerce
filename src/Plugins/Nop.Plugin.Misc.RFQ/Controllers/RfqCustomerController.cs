using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Factories;
using Nop.Plugin.Misc.RFQ.Models.Customer;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.RFQ.Controllers;

[AutoValidateAntiforgeryToken]
public class RfqCustomerController : BasePublicController
{
    #region Fields

    private readonly CaptchaSettings _captchaSettings;
    private readonly CustomerModelFactory _modelFactory;
    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly PdfSettings _pdfSettings;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public RfqCustomerController(CaptchaSettings captchaSettings,
        CustomerModelFactory modelFactory,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        PdfSettings pdfSettings,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
        _captchaSettings = captchaSettings;
        _modelFactory = modelFactory;
        _customerService = customerService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
        _pdfSettings = pdfSettings;
        _rfqService = rfqService;
        _rfqSettings = rfqSettings;
    }

    #endregion

    #region Utilities

    private async Task<IActionResult> CheckCustomerPermissionAsync(Customer customer)
    {
        if (!_rfqSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        if (!await _permissionService.AuthorizeAsync(RfqPermissionConfigManager.ACCESS_RFQ, customer))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        return null;
    }

    private async Task<IActionResult> CheckCustomerPermissionAsync(RequestQuote request)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        if (request == null)
            return null;

        if (request.CustomerId != customer.Id)
            return RedirectToRoute(RfqDefaults.CustomerRequestsRouteName);

        return null;
    }

    private async Task<IList<string>> ValidateFormAsync(RequestQuote request, List<RequestQuoteItem> items)
    {
        var errors = new List<string>();

        if (request == null)
            return errors;

        if (!Request.IsPostRequest() || !Request.HasFormContentType) 
            return errors;

        var form = await Request.ReadFormAsync();

        foreach (var requestQuoteItem in items)
        {
            await validateUnitPrice(requestQuoteItem);
            await validateQuantity(requestQuoteItem);
        }

        return errors;

        async Task validateUnitPrice(RequestQuoteItem requestQuoteItem)
        {
            var key = $"{RfqDefaults.UNIT_PRICE_FORM_KEY}{requestQuoteItem.Id}";

            if (!form.ContainsKey(key)) 
                return;

            var formValue = form[key];

            if (!decimal.TryParse(formValue, out var unitPrice))
                return;

            requestQuoteItem.RequestedUnitPrice = unitPrice;

            if (unitPrice >= 0)
                return;

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var model = await _modelFactory.PrepareRequestQuoteItemModelAsync(new RequestQuote(),
                requestQuoteItem, currentCurrency);

            errors.Add(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.CustomerRequest.RequestedUnitPrice.MustBeEqualOrGreaterThanZero"), model.ProductName));
        }

        async Task validateQuantity(RequestQuoteItem requestQuoteItem)
        {
            var key = $"{RfqDefaults.QUANTITY_FORM_KEY}{requestQuoteItem.Id}";

            if (!form.ContainsKey(key))
                return;

            var formValue = form[key];

            if (!int.TryParse(formValue, out var quantity))
                return;

            requestQuoteItem.RequestedQty = quantity;

            if (quantity > 0)
                return;

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var model = await _modelFactory.PrepareRequestQuoteItemModelAsync(new RequestQuote(),
                requestQuoteItem, currentCurrency);

            errors.Add(string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.CustomerRequest.RequestedQty.MustGreaterThanZero"), model.ProductName));
        }
    }

    #endregion

    #region Methods

    #region Requests

    public async Task<IActionResult> CustomerRequest(int? requestId = null)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        RequestQuoteModel model;

        if (requestId != null)
        {
            try
            {
                model = await _modelFactory.PrepareRequestQuoteModelAsync(requestId.Value);
            }
            catch (ArgumentNullException)
            {
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
            }

            if (model.CustomerId != customer.Id)
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
        }
        else
        {
            var (request, items) = await _rfqService.CreateRequestQuoteByShoppingCartAsync();

            if (request == null)
                return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

            model = await _modelFactory.PrepareRequestQuoteModelAsync(request, items);
        }

        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("send")]
    [ValidateCaptcha]
    public async Task<IActionResult> SendRequest(RequestQuoteModel model, bool captchaValid)
    {
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

        if (result != null)
            return result;

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _rfqSettings.ShowCaptchaOnRequestPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        var (request, items) = await _rfqService.CreateRequestQuoteByShoppingCartAsync();

        if (request == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var validationErrors = await ValidateFormAsync(request, items);

        if (validationErrors != null && validationErrors.Any())
            foreach (var validationError in validationErrors) 
                ModelState.AddModelError(string.Empty, validationError);

        if (ModelState.IsValid)
        {
            var checkResult = await CheckCustomerPermissionAsync(await _rfqService.GetRequestQuoteByIdAsync(model.Id));

            if (checkResult != null)
                return checkResult;

            model.Id = await _rfqService.SendNewRequestAsync(model.CustomerNotes);

            return RedirectToRoute(RfqDefaults.CreateCustomerRequestRouteName, new { requestId = model.Id });
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(request, items, model);

        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("back-to-cart")]
    public IActionResult BackToCart(RequestQuoteModel model)
    {
        return RedirectToRoute(NopRouteNames.General.CART);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("cancel")]
    public async Task<IActionResult> CancelRequest(RequestQuoteModel model)
    {
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            var request = await _rfqService.GetRequestQuoteByIdAsync(model.Id);

            if (request == null)
                return RedirectToRoute(RfqDefaults.CustomerRequestsRouteName);

            var checkResult = await CheckCustomerPermissionAsync(request);

            if (checkResult != null)
                return checkResult;

            await _rfqService.ChangeAndLogRequestQuoteStatusAsync(request, RequestQuoteStatus.Canceled);
            await _rfqService.UpdateRequestQuoteAsync(request);

            return await CustomerRequest(model.Id);
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);

        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("delete")]
    public async Task<IActionResult> DeleteRequest(RequestQuoteModel model)
    {
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            var request = await _rfqService.GetRequestQuoteByIdAsync(model.Id);

            var checkResult = await CheckCustomerPermissionAsync(request);

            if (checkResult != null)
                return checkResult;

            await _rfqService.DeleteRequestQuoteAsync(request);

            return RedirectToRoute(RfqDefaults.CustomerRequestsRouteName);
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);

        return View("~/Plugins/Misc.RFQ/Views/CustomerRequest.cshtml", model);
    }

    public async Task<IActionResult> CustomerRequests()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        var items = await _rfqService.GetCustomerRequestsAsync(customer.Id);
        var model = await items.SelectAwait(async item =>
            await _modelFactory.PrepareRequestQuoteModelAsync(item, new List<RequestQuoteItem>())).ToListAsync();

        return View("~/Plugins/Misc.RFQ/Views/CustomerRequests.cshtml", model);
    }

    #endregion

    #region Quotes

    public async Task<IActionResult> CustomerQuotes()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        var items = await _rfqService.GetCustomerQuotesAsync(customer.Id);
        var model = await items
            .SelectAwait(async item => await _modelFactory.PrepareQuoteModelAsync(item, new List<QuoteItem>()))
            .ToListAsync();

        return View("~/Plugins/Misc.RFQ/Views/CustomerQuotes.cshtml", model);
    }

    public async Task<IActionResult> CustomerQuote(int quoteId)
    {
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

        if (result != null)
            return result;

        QuoteModel model;

        try
        {
            model = await _modelFactory.PrepareQuoteModelAsync(quoteId);
        }
        catch (ArgumentNullException)
        {
            return RedirectToRoute(RfqDefaults.CustomerQuotesRouteName);
        }

        if (model.CustomerId != (await _workContext.GetCurrentCustomerAsync()).Id)
            return RedirectToRoute(RfqDefaults.CustomerQuotesRouteName);

        var statuses = new[] { QuoteStatus.Submitted, QuoteStatus.OrderCreated };

        if (!statuses.Contains(model.StatusType))
            return RedirectToRoute(RfqDefaults.CustomerQuotesRouteName);

        return View("~/Plugins/Misc.RFQ/Views/CustomerQuote.cshtml", model);
    }

    [HttpPost]
    [FormValueRequired("createOrder")]
    public async Task<IActionResult> CustomerQuote(QuoteModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            if (model.CustomerId != customer.Id)
                return RedirectToRoute(RfqDefaults.CustomerQuotesRouteName);

            var quote = await _rfqService.GetQuoteByIdAsync(model.Id);
            if (quote.Status is QuoteStatus.Expired or QuoteStatus.OrderCreated)
                return RedirectToRoute(RfqDefaults.CustomerQuotesRouteName);

            await _rfqService.CreateShoppingCartAsync(model.Id);

            return RedirectToRoute(NopRouteNames.General.CART);
        }

        model = await _modelFactory.PrepareQuoteModelAsync(model.Id, model);

        return View("~/Plugins/Misc.RFQ/Views/CustomerQuote.cshtml", model);
    }

    public async Task<IActionResult> ExitQuoteMode()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        var store = await _storeContext.GetCurrentStoreAsync();

        await _shoppingCartService.ClearShoppingCartAsync(customer, store.Id);

        return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> PdfDocument(int quoteId)
    {
        if (!_rfqSettings.AllowCustomerGenerateQuotePdf)
            return RedirectToRoute(RfqDefaults.CustomerQuoteRouteName, new { quoteId });

        var customer = await _workContext.GetCurrentCustomerAsync();

        var result = await CheckCustomerPermissionAsync(customer);

        if (result != null)
            return result;

        var quote = await _rfqService.GetQuoteByIdAsync(quoteId);

        if (quote.CustomerId != customer.Id)
            return RedirectToRoute(RfqDefaults.CustomerQuotesRouteName);

        await using var stream = new MemoryStream();

        await _rfqService.PrintQuoteToPdfAsync(stream, quote, _pdfSettings);
        var bytes = stream.ToArray();

        return File(bytes, MimeTypes.ApplicationPdf, string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.PdfFileName"), quote.Id) + ".pdf");
    }

    #endregion

    #endregion
}
