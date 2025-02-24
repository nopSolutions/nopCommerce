using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Factories;
using Nop.Plugin.Misc.RFQ.Models.Customer;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.RFQ.Controllers;

[AutoValidateAntiforgeryToken]
public class RfqCustomerController : BaseController
{
    #region Fields

    private readonly CustomerModelFactory _modelFactory;
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;

    #endregion

    #region Ctor

    public RfqCustomerController(CustomerModelFactory modelFactory,
        ICustomerService customerService,
        IWorkContext workContext,
        RfqService rfqService)
    {
        _modelFactory = modelFactory;
        _customerService = customerService;
        _workContext = workContext;
        _rfqService = rfqService;
    }

    #endregion

    #region Utilities

    private async Task<IActionResult> CheckCustomerPermissionAsync(RequestQuoteModel model)
    {
        return await CheckCustomerPermissionAsync(await _rfqService.GetRequestQuoteByIdAsync(model.Id));
    }

    private async Task<IActionResult> CheckCustomerPermissionAsync(RequestQuote request)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        if (request == null)
            return null;

        if (request.CustomerId != customer.Id)
            return RedirectToAction("CustomerRequests");

        return null;
    }

    private async Task<IActionResult> CheckCustomerPermissionAsync(QuoteModel quote)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        if (quote == null)
            return null;

        if (quote.CustomerId != customer.Id)
            return RedirectToAction("CustomerQuotes");

        return null;
    }

    #endregion

    #region Methods

    #region Requests

    public async Task<IActionResult> CustomerRequest(int? requestId = null)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        RequestQuoteModel model;

        if (requestId != null)
        {
            try
            {
                model = await _modelFactory.PrepareRequestQuoteModelAsync(requestId.Value);
            }
            catch (ArgumentNullException)
            {
                return RedirectToRoute("Homepage");
            }

            if (model.CustomerId != customer.Id)
                return RedirectToRoute("Homepage");
        }
        else
        {
            var (request, items) = await _rfqService.CreateRequestQuoteByShoppingCartAsync();

            if (request == null)
                return RedirectToRoute("Homepage");

            model = await _modelFactory.PrepareRequestQuoteModelAsync(request, items);
        }

        return View(model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("send")]
    public async Task<IActionResult> SendRequest(RequestQuoteModel model)
    {
        if (ModelState.IsValid)
        {
            var checkResult = await CheckCustomerPermissionAsync(model);

            if (checkResult != null)
                return checkResult;

            await _rfqService.SendNewRequestAsync(model);

            return await CustomerRequest(model.Id);
        }

        model = await _modelFactory.PrepareRequestQuoteModelAsync(model);

        return View(model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("back-to-cart")]
    public IActionResult BackToCart(RequestQuoteModel model)
    {
        return RedirectToRoute("ShoppingCart");
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("cancel")]
    public async Task<IActionResult> CancelRequest(RequestQuoteModel model)
    {
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

        return View(model);
    }

    [HttpPost, ActionName("CustomerRequest")]
    [FormValueRequired("delete")]
    public async Task<IActionResult> DeleteRequest(RequestQuoteModel model)
    {
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

        return View(model);
    }

    public async Task<IActionResult> CustomerRequests()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var items = await _rfqService.GetCustomerRequestsAsync(customer.Id);

        return View(await items.SelectAwait(async item => await _modelFactory.PrepareRequestQuoteModelAsync(item, new List<RequestQuoteItem>())).ToListAsync());
    }

    #endregion

    #region Quotes

    public async Task<IActionResult> CustomerQuotes()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsGuestAsync(customer))
            return Challenge();

        var items = await _rfqService.GetCustomerQuotesAsync(customer.Id);

        return View(await items.SelectAwait(async item => await _modelFactory.PrepareQuoteModelAsync(item, new List<QuoteItem>())).ToListAsync());
    }

    public async Task<IActionResult> CustomerQuote(int quoteId)
    {
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

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("createOrder")]
    public async Task<IActionResult> CustomerQuote(QuoteModel model)
    {
        if (ModelState.IsValid)
        {
            var checkResult = await CheckCustomerPermissionAsync(model);

            if (checkResult != null)
                return checkResult;

            await _rfqService.CreateShoppingCartAsync(model.Id);

            return RedirectToRoute("ShoppingCart");
        }

        model = await _modelFactory.PrepareQuoteModelAsync(model.Id, model);

        return View(model);
    }

    #endregion

    #endregion
}
