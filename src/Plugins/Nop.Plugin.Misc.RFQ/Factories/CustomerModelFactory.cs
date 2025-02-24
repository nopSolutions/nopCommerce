using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Models.Customer;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Misc.RFQ.Factories;

public class CustomerModelFactory : CommonModelFactory
{
    #region Fields

    private readonly ICurrencyService _currencyService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly ILocalizationService _localizationService;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IProductAttributeFormatter _productAttributeFormatter;
    private readonly IProductService _productService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWorkContext _workContext;
    private readonly MediaSettings _mediaSettings;
    private readonly RfqService _rfqService;

    #endregion

    #region Ctor

    public CustomerModelFactory(ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        RfqService rfqService) : base(pictureService)
    {
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _priceFormatter = priceFormatter;
        _productAttributeFormatter = productAttributeFormatter;
        _productService = productService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _rfqService = rfqService;
    }

    #endregion

    #region Utilities

    private async Task<QuoteItemModel> PrepareQuoteItemModelAsync(QuoteItem item, Currency currentCurrency)
    {
        var product = await _productService.GetProductByIdAsync(item.ProductId);
        var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);

        return new QuoteItemModel
        {
            Id = item.Id,
            Quantity = item.OfferedQty,
            ProductName = productName,
            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
            UnitPrice = await _priceFormatter.FormatPriceAsync(await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(item.OfferedUnitPrice, currentCurrency), true, currentCurrency),
            AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml),
            PictureUrl = await GetPictureUrlAsync(product, item.AttributesXml, productName, _mediaSettings.CartThumbPictureSize),
        };
    }

    #endregion

    #region Methods

    public async Task<RequestQuoteModel> PrepareRequestQuoteModelAsync(RequestQuote requestQuote, IList<RequestQuoteItem> requestQuoteItems = null, RequestQuoteModel model = null)
    {
        ArgumentNullException.ThrowIfNull(requestQuote);

        var items = requestQuoteItems ?? await _rfqService.GetRequestQuoteItemsAsync(requestQuote.Id);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

        var modelItems = await items.SelectAwait(async item => await PrepareRequestQuoteItemModelAsync(requestQuote, item, currentCurrency)).ToListAsync();

        model ??= new RequestQuoteModel();

        if (requestQuote.Status != 0)
            model.Status = await _localizationService.GetLocalizedEnumAsync(requestQuote.Status);
        
        model.StatusType = requestQuote.Status;
        model.CreatedOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(requestQuote.CreatedOnUtc, DateTimeKind.Utc);
        model.CustomerNotes = requestQuote.CustomerNotes;
        model.Id = requestQuote.Id;
        model.CustomerId = requestQuote.CustomerId;
        model.CustomerItems = modelItems;

        return model;
    }

    public async Task<QuoteModel> PrepareQuoteModelAsync(Quote quote, IList<QuoteItem> quoteItems = null, QuoteModel model = null)
    {
        ArgumentNullException.ThrowIfNull(quote);

        var items = quoteItems ?? await _rfqService.GetQuoteItemsAsync(quote.Id);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

        var modelItems = await items.SelectAwait(async item => await PrepareQuoteItemModelAsync(item, currentCurrency)).ToListAsync();

        model ??= new QuoteModel();

        model.Status = await _localizationService.GetLocalizedEnumAsync(quote.Status);
        model.Order = quote.OrderId;
        model.StatusType = quote.Status;
        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quote.CreatedOnUtc, DateTimeKind.Utc);
        model.Id = quote.Id;
        model.CustomerId = quote.CustomerId;
        model.CustomerItems = modelItems;

        if (quote.ExpirationDateUtc.HasValue)
            model.ExpirationDate = await _dateTimeHelper.ConvertToUserTimeAsync(quote.ExpirationDateUtc.Value, DateTimeKind.Utc);

        return model;
    }

    public async Task<RequestQuoteItemModel> PrepareRequestQuoteItemModelAsync(RequestQuote requestQuote, RequestQuoteItem item, Currency currentCurrency)
    {
        var product = await _productService.GetProductByIdAsync(item.ProductId);
        var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);

        var unitPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(item.RequestedUnitPrice, currentCurrency);

        return new RequestQuoteItemModel
        {
            Id = item.Id,
            Quantity = item.RequestedQty,
            OriginalProductCost = await _priceFormatter.FormatPriceAsync(item.OriginalProductPrice, true, currentCurrency),
            ProductName = productName,
            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
            UnitPrice = unitPrice,
            UnitPriceText = await _priceFormatter.FormatPriceAsync(unitPrice, true, currentCurrency),
            AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, item.ProductAttributesXml),
            PictureUrl = await GetPictureUrlAsync(product, item.ProductAttributesXml, productName, _mediaSettings.CartThumbPictureSize),
            Editable = requestQuote.Status == 0
        };
    }

    public async Task<RequestQuoteModel> PrepareRequestQuoteModelAsync(int requestId, RequestQuoteModel model = null)
    {
        return await PrepareRequestQuoteModelAsync(await _rfqService.GetRequestQuoteByIdAsync(requestId), model: model);
    }

    public async Task<RequestQuoteModel> PrepareRequestQuoteModelAsync(RequestQuoteModel model)
    {
        return await PrepareRequestQuoteModelAsync(await _rfqService.GetRequestQuoteByIdAsync(model.Id), model: model);
    }

    public async Task<QuoteModel> PrepareQuoteModelAsync(int quoteId, QuoteModel model = null)
    {
        return await PrepareQuoteModelAsync(await _rfqService.GetQuoteByIdAsync(quoteId), model: model);
    }

    #endregion
}