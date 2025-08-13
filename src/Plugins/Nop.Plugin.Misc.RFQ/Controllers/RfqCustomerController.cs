using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Factories;
using Nop.Plugin.Misc.RFQ.Models.Customer;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.RFQ.Controllers;

[AutoValidateAntiforgeryToken]
public class RfqCustomerController : BasePublicController
{
    #region Fields

    private readonly CustomerModelFactory _modelFactory;
    private readonly ICustomerService _customerService;
    private readonly IPermissionService _permissionService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public RfqCustomerController(CustomerModelFactory modelFactory,
        ICustomerService customerService,
        IPermissionService permissionService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
        _modelFactory = modelFactory;
        _customerService = customerService;
        _permissionService = permissionService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
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
            return RedirectToAction("CustomerRequests");

        return null;
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
    public async Task<IActionResult> SendRequest(RequestQuoteModel model)
    {
        var result = await CheckCustomerPermissionAsync(await _workContext.GetCurrentCustomerAsync());

        if (result != null)
            return result;

        if (ModelState.IsValid)
        {
            var checkResult = await CheckCustomerPermissionAsync(await _rfqService.GetRequestQuoteByIdAsync(model.Id));

            if (checkResult != null)
                return checkResult;

            model.Id = await _rfqService.SendNewRequestAsync(model.CustomerNotes);

            return RedirectToAction("CustomerRequest", "RfqCustomer", new { requestId = model.Id });
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);

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
                return RedirectToAction("CustomerRequests");

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

            return RedirectToAction("CustomerRequests");
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
            return RedirectToAction("CustomerQuotes");
        }

        if (model.CustomerId != (await _workContext.GetCurrentCustomerAsync()).Id)
            return RedirectToAction("CustomerQuotes");

        var statuses = new[] { QuoteStatus.Submitted, QuoteStatus.OrderCreated };

        if (!statuses.Contains(model.StatusType))
            return RedirectToAction("CustomerQuotes");

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
                return RedirectToAction("CustomerQuotes");

            var quote = await _rfqService.GetQuoteByIdAsync(model.Id);
            if (quote.Status is QuoteStatus.Expired or QuoteStatus.OrderCreated)
                return RedirectToAction("CustomerQuotes");

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

    #endregion

    #endregion
}
