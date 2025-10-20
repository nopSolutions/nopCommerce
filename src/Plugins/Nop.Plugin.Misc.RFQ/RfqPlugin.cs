using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.RFQ.Components;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.RFQ;

/// <summary>
/// Represents the "Request a quote" and "Quotes" plugin
/// </summary>
public class RfqPlugin : BasePlugin, IWidgetPlugin, IMiscPlugin
{
    #region Fields

    private readonly EmailAccountSettings _emailAccountSettings;
    private readonly IEmailAccountService _emailAccountService;
    private readonly ILocalizationService _localizationService;
    private readonly IMessageTemplateService _messageTemplateService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public RfqPlugin(EmailAccountSettings emailAccountSettings,
        IEmailAccountService emailAccountService,
        ILocalizationService localizationService,
        IMessageTemplateService messageTemplateService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _emailAccountSettings = emailAccountSettings;
        _emailAccountService = emailAccountService;
        _localizationService = localizationService;
        _messageTemplateService = messageTemplateService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(RfqDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget zones
    /// </returns>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            PublicWidgetZones.OrderSummaryContentAfter,
            PublicWidgetZones.AccountNavigationAfter
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    /// <param name="widgetZone">Name of the widget zone</param>
    /// <returns>View component type</returns>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == PublicWidgetZones.OrderSummaryContentAfter)
            return typeof(AddRfqComponent);

        if (widgetZone == PublicWidgetZones.AccountNavigationAfter)
            return typeof(CustomerRfqMenuComponent);

        return null;
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new RfqSettings
        {
            Enabled = true
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(RfqDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(RfqDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        var eaGeneral = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault()
            ?? throw new Exception("Default email account cannot be loaded");

        var messageTemplates = new List<MessageTemplate>();
        var allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(0);
        var existsMessageTemplateNames = allMessageTemplates.Select(t => t.Name).ToList();

        if (!existsMessageTemplateNames.Contains(RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE))
        {
            messageTemplates.Add(new MessageTemplate
            {
                Name = RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE,
                Subject = "%Store.Name%. New request a quote #%RequestQuote.Id% sent",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Customer.FullName% (%Customer.Email%) has just sent a new request a quote.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Request a quote number: %RequestQuote.Id%{Environment.NewLine}<br />{Environment.NewLine}Date of request a quote: %RequestQuote.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}See details on the <a href=\"%RequestQuote.URL%\">request a quote page</a>{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        if (!existsMessageTemplateNames.Contains(RfqDefaults.ADMIN_SENT_NEW_QUOTE))
        {
            messageTemplates.Add(new MessageTemplate
            {
                Name = RfqDefaults.ADMIN_SENT_NEW_QUOTE,
                Subject = "%Store.Name%. New quote #%Quote.Id% received",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Store owner has just sent a new quote.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quote number: %Quote.Id%{Environment.NewLine}<br />{Environment.NewLine}Date of the quote: %Quote.CreatedOn%{Environment.NewLine}<br />%if(%Quote.ExpirationOnIsSet%)Expiration date of the quote: %Quote.ExpirationOn%{Environment.NewLine}<br />endif%{Environment.NewLine}<br />{Environment.NewLine}See details on the <a href=\"%Quote.URL%\">The quote page</a>{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        if (messageTemplates.Any())
            await Task.WhenAll(messageTemplates.Select(_messageTemplateService.InsertMessageTemplateAsync));

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.RFQ.Enabled"] = "Enabled",
            ["Plugins.Misc.RFQ.Enabled.Hint"] = "Check to enable RFQ functionality.",
            ["Plugins.Misc.RFQ.CreateRequest"] = "Request a quote",
            ["Plugins.Misc.RFQ.ClearRequest"] = "Exit quote mode",
            ["Plugins.Misc.RFQ.CreateNew"] = "Add new",
            ["Plugins.Misc.RFQ.SendRequest"] = "Send request",
            ["Plugins.Misc.RFQ.RequestsQuote"] = "Requests for quote",
            ["Plugins.Misc.RFQ.RequestQuoteNumber"] = "Request a quote #{0}",
            ["Plugins.Misc.RFQ.NewRequestQuote"] = "Create a new request a quote",
            ["Plugins.Misc.RFQ.QuoteNumber"] = "Quote #{0}",
            ["Plugins.Misc.RFQ.Quotes"] = "Quotes",
            ["Plugins.Misc.RFQ.CustomerRequest.Info"] = "Request info",
            ["Plugins.Misc.RFQ.CustomerQuote.Info"] = "Quote info",
            ["Plugins.Misc.RFQ.Fields.Quote.CreatedOn"] = "Created on",
            ["Plugins.Misc.RFQ.Fields.Quote.CreatedOn.Hint"] = "The date/time that the quote was created.",
            ["Plugins.Misc.RFQ.Fields.RequestQuote.CreatedOn"] = "Created on",
            ["Plugins.Misc.RFQ.Fields.RequestQuote.CreatedOn.Hint"] = "The date/time that the request a quote was created.",
            ["Plugins.Misc.RFQ.Fields.Quote.Status"] = "Status",
            ["Plugins.Misc.RFQ.Fields.Quote.Status.Hint"] = "The status of the quote",
            ["Plugins.Misc.RFQ.Fields.RequestQuote.Status"] = "Status",
            ["Plugins.Misc.RFQ.Fields.RequestQuote.Status.Hint"] = "The status of the request a quote",
            ["Plugins.Misc.RFQ.Fields.Order"] = "Order",
            ["Plugins.Misc.RFQ.Fields.Order.Hint"] = "Created order",
            ["Plugins.Misc.RFQ.Fields.CustomerNotes"] = "Customer notes",
            ["Plugins.Misc.RFQ.Fields.CustomerNotes.Hint"] = "The customer notes and additional information",
            ["Plugins.Misc.RFQ.Products"] = "Product(s)",
            ["Plugins.Misc.RFQ.Product"] = "Product",
            ["Plugins.Misc.RFQ.ProductDeleted"] = "The product has been deleted",
            ["Plugins.Misc.RFQ.OriginalProductPrice"] = "Original unit price",
            ["Plugins.Misc.RFQ.CustomerRequest.RequestedQty"] = "Requested qty",
            ["Plugins.Misc.RFQ.CustomerRequest.RequestedUnitPrice"] = "Requested unit price",
            ["Plugins.Misc.RFQ.RequestQuoteItem.Fields.CustomerNotes"] = "Customer notes",
            ["Plugins.Misc.RFQ.Fields.AdminNotes"] = "Admin notes",
            ["Plugins.Misc.RFQ.Fields.AdminNotes.Hint"] = "The admin notes and changelog",
            ["Plugins.Misc.RFQ.CustomerRequests.NoRequests"] = "You don't have any requests for quote.",
            ["Plugins.Misc.RFQ.CustomerRequests.NoQuotes"] = "You don't have any quotes yet.",
            ["Plugins.Misc.RFQ.CreatedOnFrom"] = "Created from",
            ["Plugins.Misc.RFQ.CreatedOnFrom.Hint"] = "The creation from date for the search.",
            ["Plugins.Misc.RFQ.CreatedOnTo"] = "Created to",
            ["Plugins.Misc.RFQ.CreatedOnTo.Hint"] = "The creation to date for the search.",
            ["Plugins.Misc.RFQ.AdminRequests.RequestQuoteStatus"] = "Request status",
            ["Plugins.Misc.RFQ.AdminRequests.RequestQuoteStatus.Hint"] = "Select the request status",
            ["Plugins.Misc.RFQ.Fields.CustomerEmail"] = "Customer email",
            ["Plugins.Misc.RFQ.Fields.CustomerEmail.Hint"] = "The email address of customer",
            ["Plugins.Misc.RFQ.DeleteSelected"] = "Delete selected",
            ["Plugins.Misc.RFQ.AdminRequest.BackToList"] = "back to requests for quote list",
            ["Plugins.Misc.RFQ.AdminQuote.BackToList"] = "back to quote list",
            ["Plugins.Misc.RFQ.AddNewProduct"] = "Add new product",
            ["Plugins.Misc.RFQ.AdminQuote.Products.AddNew.Title1"] = "Add a new product to quote #{0}",
            ["Plugins.Misc.RFQ.AdminRequest.Customer.Select.Title"] = "Select a customer to create the quote",
            ["Plugins.Misc.RFQ.AdminRequest.Products.AddNew.Title2"] = "Add product '{0}' to quote #{1}",
            ["Plugins.Misc.RFQ.AdminRequest.Products.AddNew.BackToRequest"] = "back to requests for quote",
            ["Plugins.Misc.RFQ.AdminQuote.Products.AddNew.BackToQuote"] = "back to quote",
            ["Plugins.Misc.RFQ.Products.AddNew.Name"] = "Product name",
            ["Plugins.Misc.RFQ.Products.AddNew.SKU"] = "SKU",
            ["Plugins.Misc.RFQ.RequestCreated"] = "Request created by {0}",
            ["Plugins.Misc.RFQ.RequestItemCreated"] = "Request item created by {0}",
            ["Plugins.Misc.RFQ.QuantityChanged"] = "Quantity changed from {0} to {1} by {2}",
            ["Plugins.Misc.RFQ.UnitPriceChanged"] = "Unit price changed from {0} to {1} by {2}",
            ["Plugins.Misc.RFQ.CustomerNoteChanged"] = "Customer changed the notes",
            ["Plugins.Misc.RFQ.CreateQuote"] = "Create the quote",
            ["Plugins.Misc.RFQ.AdminQuote.QuoteCreatedByRequest"] = "Quote created from the request by {0}",
            ["Plugins.Misc.RFQ.AdminQuote.QuoteCreatedManuallyByStoreOwner"] = "Quote created manually by {0}",
            ["Plugins.Misc.RFQ.Fields.Quote.ExpirationDate"] = "Expiration date",
            ["Plugins.Misc.RFQ.Fields.Quote.ExpirationDate.Hint"] = "The date/time that the quote will expire in Coordinated Universal Time (UTC).",
            ["Plugins.Misc.RFQ.AdminQuote.QuoteStatus"] = "Quote status",
            ["Plugins.Misc.RFQ.AdminQuote.QuoteStatus.Hint"] = "Select the quote status",
            ["Plugins.Misc.RFQ.AdminRequest.Updated"] = "The request a quote has been updated successfully.",
            ["Plugins.Misc.RFQ.AdminRequest.Deleted"] = "The request a quote has been deleted successfully.",
            ["Plugins.Misc.RFQ.AdminRequest.Canceled"] = "The request a quote has been canceled successfully.",
            ["Plugins.Misc.RFQ.AdminRequest.Selected.Deleted"] = "The selected requests a quote has been deleted successfully.",
            ["Plugins.Misc.RFQ.AdminQuote.Updated"] = "The quote has been updated successfully.",
            ["Plugins.Misc.RFQ.AdminQuote.Deleted"] = "The quote has been deleted successfully.",
            ["Plugins.Misc.RFQ.AdminQuote.QuoteSubmitted"] = "The quote has been submitted successfully",
            ["Plugins.Misc.RFQ.AdminQuote.RequestedUnitPrice"] = "Requested unit price",
            ["Plugins.Misc.RFQ.AdminQuote.RequestedQty"] = "Requested qty",
            ["Plugins.Misc.RFQ.AdminQuote.Selected.Deleted"] = "The selected quotes has been deleted successfully.",
            ["Plugins.Misc.RFQ.OfferedQty"] = "Offered qty",
            ["Plugins.Misc.RFQ.OfferedUnitPrice"] = "Offered unit price",
            ["Plugins.Misc.RFQ.SendQuote"] = "Send quote",
            ["Plugins.Misc.RFQ.CreateOrder"] = "Create the order",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.RequestQuoteStatus.Submitted"] = "Submitted to store owner",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.RequestQuoteStatus.Canceled"] = "Canceled",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.RequestQuoteStatus.QuoteIsCreated"] = "Quote is created",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.QuoteStatus.CreatedFromRequestQuote"] = "Created from customer request",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.QuoteStatus.CreatedManuallyByStoreOwner"] = "Created manually by store owner",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.QuoteStatus.Submitted"] = "Submitted to customer",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.QuoteStatus.OrderCreated"] = "Order is created",
            ["Enums.Nop.Plugin.Misc.RFQ.Domains.QuoteStatus.Expired"] = "Quote is expired",
            ["Plugins.Misc.RFQ.RequestQuoteStatusChanged"] = "Status changed from {0} to {1} by {2}",
            ["Plugins.Misc.RFQ.QuoteStatusChanged"] = "Status changed from {0} to {1} by {2}",
            ["Plugins.Misc.RFQ.RequestQuoteItemDeleted"] = "Item id #{0} is deleted by {1}",
            ["Plugins.Misc.RFQ.QuoteItemAdded"] = "Item id #{0} is added by {1}",
            ["Plugins.Misc.RFQ.QuoteItemDeleted"] = "Item id #{0} is deleted by {1}",
            ["Plugins.Misc.RFQ.Image"] = "Image",
            ["Plugins.Misc.RFQ.Product(s)"] = "Product(s)",
            ["Plugins.Misc.RFQ.Fields.RequestQuoteId"] = "Related request a quote",
            ["Plugins.Misc.RFQ.Fields.RequestQuoteId.Hint"] = "The link to the related request a quote",
            ["Plugins.Misc.RFQ.Fields.QuoteId"] = "Related quote",
            ["Plugins.Misc.RFQ.Fields.QuoteId.Hint"] = "The link to the related quote",
            ["Plugins.Misc.RFQ.BackToShoppingCart"] = "Back to shopping cart",
            ["Plugins.Misc.RFQ.QuoteExpired"] = "Status changed to expired",
            [$"Admin.ContentManagement.MessageTemplates.Description.{RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE}"] = "This message template is used to notify a store owner that the new request a quote sent",
            [$"Admin.ContentManagement.MessageTemplates.Description.{RfqDefaults.ADMIN_SENT_NEW_QUOTE}"] = "This message template is used to notify a customer that the new quote sent",
            ["Security.Permission.Misc.RFQ.AccessRFQ.Admin.AccessRFQ"] = "Admin area. Access to the customer’s Request and Price Offer functionality",
            ["Security.Permission.Misc.RFQ.AccessRFQ.PublicStore.AccessRFQ"] = "Public store. Access to the customer’s Request and Price Offer functionality"
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<RfqSettings>();

        //delete permission
        var permissionRecord = (await _permissionService.GetAllPermissionRecordsAsync())
            .FirstOrDefault(x => x.SystemName == RfqPermissionConfigManager.ADMIN_ACCESS_RFQ);

        if (permissionRecord != null)
            await _permissionService.DeletePermissionRecordAsync(permissionRecord);

        permissionRecord = (await _permissionService.GetAllPermissionRecordsAsync())
            .FirstOrDefault(x => x.SystemName == RfqPermissionConfigManager.ACCESS_RFQ);

        if (permissionRecord != null)
            await _permissionService.DeletePermissionRecordAsync(permissionRecord);

        if (_widgetSettings.ActiveWidgetSystemNames.Contains(RfqDefaults.SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(RfqDefaults.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        var deletedMessageTemplates = new List<MessageTemplate>();
        deletedMessageTemplates.AddRange((await _messageTemplateService.GetMessageTemplatesByNameAsync(RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE)).ToList());
        deletedMessageTemplates.AddRange((await _messageTemplateService.GetMessageTemplatesByNameAsync(RfqDefaults.ADMIN_SENT_NEW_QUOTE)).ToList());

        deletedMessageTemplates = deletedMessageTemplates.Where(mt => mt != null).ToList();

        if (deletedMessageTemplates.Any())
            await Task.WhenAll(deletedMessageTemplates.Select(_messageTemplateService.DeleteMessageTemplateAsync));

        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.RFQ");
        await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Misc.RFQ.Domains.RequestQuoteStatus");
        await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Misc.RFQ.Domains.QuoteStatus");
        await _localizationService.DeleteLocaleResourceAsync($"Admin.ContentManagement.MessageTemplates.Description.{RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE}");
        await _localizationService.DeleteLocaleResourceAsync($"Admin.ContentManagement.MessageTemplates.Description.{RfqDefaults.ADMIN_SENT_NEW_QUOTE}");
        await _localizationService.DeleteLocaleResourcesAsync("Security.Permission.Misc.RFQ.AccessRFQ.Admin.AccessRFQ");
        await _localizationService.DeleteLocaleResourcesAsync("Security.Permission.Misc.RFQ.AccessRFQ.PublicStore.AccessRFQ");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => true;

    #endregion
}