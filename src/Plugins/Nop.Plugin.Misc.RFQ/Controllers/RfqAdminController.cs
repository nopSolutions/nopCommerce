using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Factories;
using Nop.Plugin.Misc.RFQ.Models.Admin;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.RFQ.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class RfqAdminController : BasePluginController
{
    #region Fields

    private readonly AdminModelFactory _modelFactory;
    private readonly ICustomerModelFactory _customerModelFactory;
    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly ISettingService _settingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly RfqMessageService _rfqMessageService;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public RfqAdminController(AdminModelFactory modelFactory,
        ICustomerModelFactory customerModelFactory,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        RfqMessageService rfqMessageService,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
        _modelFactory = modelFactory;
        _customerModelFactory = customerModelFactory;
        _customerService = customerService;
        _localizationService = localizationService;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _notificationService = notificationService;
        _settingService = settingService;
        _shoppingCartService = shoppingCartService;
        _rfqMessageService = rfqMessageService;
        _rfqService = rfqService;
        _rfqSettings = rfqSettings;
    }

    #endregion

    #region Methods

    #region Configure

    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel
        {
            Enabled = _rfqSettings.Enabled
        };

        return View("~/Plugins/Misc.RFQ/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();
        
        _rfqSettings.Enabled = model.Enabled;

        await _settingService.SaveSettingAsync(_rfqSettings);
        
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion

    #region Add product to quote

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddNewProduct(ProductSearchModel searchModel)
    {
        //prepare model
        var model = await _modelFactory.PrepareProductListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddProductDetails(int productId, int quoteId)
    {
        var quote = await _rfqService.GetQuoteByIdAsync(quoteId)
            ?? throw new ArgumentException("No quote found with the specified id");

        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //prepare model
        var model = await _modelFactory.PrepareAddProductModelAsync(new AddProductModel(), quote, product);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AddProductDetails.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddProductDetails(int quoteId, int productId, IFormCollection form)
    {
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //basic properties
        _ = decimal.TryParse(form["UnitPriceInclTax"], out var unitPriceInclTax);
        _ = int.TryParse(form["Quantity"], out var quantity);

        //warnings
        var warnings = new List<string>();

        //attributes
        var attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, warnings);

        //rental product
        _productAttributeParser.ParseRentalDates(product, form, out _, out _);

        var quote = await _rfqService.GetQuoteByIdAsync(quoteId)
                ?? throw new ArgumentException("No quote found with the specified id");

        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(quote.CustomerId)
            ?? throw new ArgumentException("No customer found with the specified id");

        //warnings
        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));

        if (!warnings.Any())
        {
            var quoteItem = new QuoteItem
            {
                AttributesXml = attributesXml,
                ProductId = product.Id,
                OfferedQty = quantity,
                QuoteId = quoteId,
                OfferedUnitPrice = unitPriceInclTax
            };

            await _rfqService.InsertQuoteItemAsync(quoteItem);

            return RedirectToAction("AdminQuote", new { id = quoteId });
        }

        //prepare model
        var model = await _modelFactory.PrepareAddProductModelAsync(new AddProductModel(), quote, product);
        model.Warnings.AddRange(warnings);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AddProductDetails.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> ProductDetailsAttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return new NullJsonResult();

        var errors = new List<string>();
        var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);

        //conditional attributes
        var enabledAttributeMappingIds = new List<int>();
        var disabledAttributeMappingIds = new List<int>();

        if (validateAttributeConditions)
        {
            var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);

            foreach (var attribute in attributes)
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);

                if (!conditionMet.HasValue)
                    continue;

                if (conditionMet.Value)
                    enabledAttributeMappingIds.Add(attribute.Id);
                else
                    disabledAttributeMappingIds.Add(attribute.Id);
            }
        }

        return Json(new
        {
            enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
            disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
            message = errors.Any() ? errors.ToArray() : null
        });
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public async Task<IActionResult> CustomerList(CustomerSearchModel searchModel)
    {
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public async Task<IActionResult> SelectCustomerPopup()
    {
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerSearchModelAsync(new CustomerSearchModel());

        return View("~/Plugins/Misc.RFQ/Views/Admin/SelectCustomerPopup.cshtml", model);
    }

    #endregion

    #region Request a quote

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminRequests()
    {
        //prepare model
        var model = await _modelFactory.PrepareRequestQuoteSearchModelAsync();

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminRequests.cshtml", model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminRequest(int id)
    {
        if (id <= 0)
            return RedirectToAction("AdminRequests");

        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(id);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        var model = await _modelFactory.PreparedRequestQuoteModelAsync(requestQuote);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminRequest.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminRequest(RequestQuoteModel model, bool continueEditing)
    {
        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(model.Id);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        if (ModelState.IsValid)
        {
            if (requestQuote.AdminNotes != model.AdminNotes)
            {
                requestQuote.AdminNotes = model.AdminNotes;
                await _rfqService.UpdateRequestQuoteAsync(requestQuote);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Updated"));

            return !continueEditing ? RedirectToAction("AdminRequests") : RedirectToAction("AdminRequest", new { id = requestQuote.Id });
        }

        //if we got this far, something failed, redisplay form
        model = await _modelFactory.PreparedRequestQuoteModelAsync(requestQuote);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminRequest.cshtml", model);
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnSave")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> EditRequestItem(int id, IFormCollection form)
    {
        if (id <= 0)
            return RedirectToAction("AdminRequests");

        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(id);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        //get request a quote item identifier
        var requestQuoteItemId = 0;

        var saveButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnSave", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(saveButtonId))
            requestQuoteItemId = Convert.ToInt32(saveButtonId["btnSave".Length..]);

        if (requestQuoteItemId <= 0)
            return await AdminRequest(id);

        int.TryParse(form[$"quantity{requestQuoteItemId}"], out var requestedQty);
        decimal.TryParse(form[$"unitPrice{requestQuoteItemId}"], out var requestedUnitPrice);

        await _rfqService.UpdateRequestQuoteItemAsync(requestQuoteItemId, requestedQty, requestedUnitPrice);

        if (requestQuote.AdminNotes != form["AdminNotes"])
        {
            requestQuote.AdminNotes = form["AdminNotes"];
            await _rfqService.UpdateRequestQuoteAsync(requestQuote);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Updated"));

        return await AdminRequest(id);
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnDelete")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteRequestItem(int id, IFormCollection form)
    {
        if (id <= 0)
            return RedirectToAction("AdminRequests");

        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(id);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        //get request a quote item identifier
        var requestQuoteItemId = 0;

        var deleteButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnDelete", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(deleteButtonId))
            requestQuoteItemId = Convert.ToInt32(deleteButtonId["btnDelete".Length..]);

        if (requestQuoteItemId <= 0)
            return await AdminRequest(id);

        await _rfqService.DeleteRequestQuoteItemAsync(requestQuoteItemId);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Updated"));

        return await AdminRequest(id);
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired("deleteRequest")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteRequest(RequestQuoteModel model)
    {
        await _rfqService.DeleteRequestQuoteAsync(model.Id);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Deleted"));

        return RedirectToAction("AdminRequests");
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired("cancelRequest")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> CancelRequest(RequestQuoteModel model)
    {
        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(model.Id);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        await _rfqService.ChangeAndLogRequestQuoteStatusAsync(requestQuote, RequestQuoteStatus.Canceled);
        await _rfqService.UpdateRequestQuoteAsync(requestQuote);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Canceled"));

        return RedirectToAction("AdminRequest", new { id = requestQuote.Id });
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteSelectedRequests(ICollection<int> selectedIds)
    {
        await _rfqService.DeleteRequestsQuoteByIdsAsync(selectedIds);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Selected.Deleted"));

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> RequestList(RequestQuoteSearchModel searchModel)
    {
        //prepare model
        var model = await _modelFactory.PrepareRequestQuoteListModelAsync(searchModel);

        return Json(model);
    }

    #endregion

    #region Quote

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> CreateQuote(RequestQuoteModel model)
    {
        var quoteId = await _rfqService.CreateQuoteByRequestAsync(model.Id);

        return RedirectToAction("AdminQuote", new { id = quoteId });
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuote(int id)
    {
        if (id <= 0)
            return RedirectToAction("AdminQuotes");

        var quote = await _rfqService.GetQuoteByIdAsync(id);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        var model = await _modelFactory.PreparedQuoteModelAsync(quote);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminQuote.cshtml", model);
    }

    [HttpPost]
    [FormValueRequired("btnCreate")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuotes(QuoteSearchModel model)
    {
        var requestQuote = await _rfqService.CreateQuoteAsync(model.CustomerId);

        return RedirectToAction("AdminQuote", new { id = requestQuote.Id });
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuotes()
    {
        //prepare model
        var model = await _modelFactory.PrepareQuoteSearchModelAsync();

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminQuotes.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteSelectedQuotes(ICollection<int> selectedIds)
    {
        await _rfqService.DeleteQuotesByIdsAsync(selectedIds);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Selected.Deleted"));

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> QuoteList(QuoteSearchModel searchModel)
    {
        //prepare model
        var model = await _modelFactory.PrepareQuoteListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddProductToQuote(int quoteId)
    {
        var quote = await _rfqService.GetQuoteByIdAsync(quoteId);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        //prepare model
        var model = await _modelFactory.PrepareAddProductSearchModelAsync(new ProductSearchModel(), quote.Id);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AddProductToQuote.cshtml", model);
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired("sendQuote")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> SendQuote(QuoteModel model)
    {
        var quote = await _rfqService.GetQuoteByIdAsync(model.Id);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        await _rfqService.ChangeAndLogQuoteStatusAsync(quote, QuoteStatus.Submitted);
        await _rfqService.UpdateQuoteAsync(quote);

        await _rfqMessageService.AdminSentNewQuoteAsync(quote);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.QuoteSubmitted"));

        return RedirectToAction("AdminQuote", new { id = quote.Id });
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired("deleteQuote")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteQuote(QuoteModel model)
    {
        await _rfqService.DeleteQuoteAsync(model.Id);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Deleted"));

        return RedirectToAction("AdminQuotes");
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuote(QuoteModel model, bool continueEditing)
    {
        var quote = await _rfqService.GetQuoteByIdAsync(model.Id);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        if (ModelState.IsValid)
        {
            var needUpdate = false;

            if (quote.AdminNotes != model.AdminNotes)
            {
                needUpdate = true;
                quote.AdminNotes = model.AdminNotes;
            }

            if (quote.ExpirationDateUtc != model.ExpirationDateUtc)
            {
                needUpdate = true;
                quote.ExpirationDateUtc = model.ExpirationDateUtc;
            }

            if (needUpdate)
                await _rfqService.UpdateQuoteAsync(quote);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Updated"));

            return !continueEditing ? RedirectToAction("AdminQuotes") : RedirectToAction("AdminQuote", new { id = quote.Id });
        }

        //if we got this far, something failed, redisplay form
        model = await _modelFactory.PreparedQuoteModelAsync(quote);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminQuote.cshtml", model);
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnSave")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> EditQuoteItem(int id, IFormCollection form)
    {
        if (id <= 0)
            return RedirectToAction("AdminQuotes");

        var quote = await _rfqService.GetQuoteByIdAsync(id);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        //get the quote item identifier
        var quoteItemId = 0;

        var saveButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnSave", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(saveButtonId))
            quoteItemId = Convert.ToInt32(saveButtonId["btnSave".Length..]);

        if (quoteItemId <= 0)
            return await AdminQuote(id);

        int.TryParse(form[$"quantity{quoteItemId}"], out var offeredQty);
        decimal.TryParse(form[$"unitPrice{quoteItemId}"], out var offeredUnitPrice);

        await _rfqService.UpdateQuoteItemAsync(quoteItemId, offeredQty, offeredUnitPrice);

        if (quote.AdminNotes != form["AdminNotes"])
        {
            quote.AdminNotes = form["AdminNotes"];
            await _rfqService.UpdateQuoteAsync(quote);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Updated"));

        return await AdminQuote(id);
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnDelete")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteQuoteItem(int id, IFormCollection form)
    {
        if (id <= 0)
            return RedirectToAction("AdminQuotes");

        var quote = await _rfqService.GetQuoteByIdAsync(id);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        //get the quote item identifier
        var quoteItemId = 0;

        var deleteButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnDelete", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(deleteButtonId))
            quoteItemId = Convert.ToInt32(deleteButtonId["btnDelete".Length..]);

        if (quoteItemId <= 0)
            return await AdminQuote(id);

        await _rfqService.DeleteQuoteItemAsync(quoteItemId);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Updated"));

        return await AdminQuote(id);
    }

    #endregion

    #endregion
}