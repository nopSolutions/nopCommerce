using Nop.Core;
using Nop.Core.Domain.Catalog;
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

public class CustomerModelFactory
{
    #region Fields

    private readonly ICurrencyService _currencyService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly ILocalizationService _localizationService;
    private readonly IPictureService _pictureService;
    private readonly IPriceCalculationService _priceCalculationService;
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
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductService productService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        RfqService rfqService)
    {
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
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

    /// <summary>
    /// Prepare the picture URL
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Product attributes xml</param>
    /// <param name="imageSize">Image size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture URL
    /// </returns>
    private async Task<string> GetPictureUrlAsync(Product product, string attributesXml, int imageSize = 200)
    {
        var sciPicture = await _pictureService.GetProductPictureAsync(product, attributesXml);

        return (await _pictureService.GetPictureUrlAsync(sciPicture, imageSize)).Url;
    }

    private async Task<QuoteItemModel> PrepareQuoteItemModelAsync(QuoteItem item, Currency currentCurrency)
    {
        var product = await _productService.GetProductByIdAsync(item.ProductId);

        return new QuoteItemModel
        {
            Id = item.Id,
            Quantity = item.OfferedQty,
            ProductName = product != null ? await _localizationService.GetLocalizedAsync(product, x => x.Name) : await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.ProductDeleted"),
            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
            UnitPrice = await _priceFormatter.FormatPriceAsync(await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(item.OfferedUnitPrice, currentCurrency), true, currentCurrency),
            AttributeInfo = product != null ? await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml) : string.Empty,
            PictureUrl = product != null ? await GetPictureUrlAsync(product, item.AttributesXml, _mediaSettings.CartThumbPictureSize) : await _pictureService.GetDefaultPictureUrlAsync(_mediaSettings.CartThumbPictureSize),
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare a request a quote model
    /// </summary>
    /// <param name="requestQuote">The request a quote</param>
    /// <param name="requestQuoteItems">The request a quote items</param>
    /// <param name="model">Request a quote model to extend</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote model
    /// </returns>
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
        model.QuoteId = requestQuote.QuoteId;

        if (!requestQuote.QuoteId.HasValue)
            return model;

        var quote = await _rfqService.GetQuoteByIdAsync(requestQuote.QuoteId.Value);
        model.QuoteStatus = quote.Status;

        return model;
    }

    /// <summary>
    /// Prepare a quote model
    /// </summary>
    /// <param name="quote">The quote</param>
    /// <param name="quoteItems">The quote items</param>
    /// <param name="model">Quote model to extend</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote model
    /// </returns>
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

    /// <summary>
    /// Prepare a request a quote item model
    /// </summary>
    /// <param name="requestQuote">The request a quote</param>
    /// <param name="item">The request a quote item</param>
    /// <param name="currentCurrency">The current currency</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item model
    /// </returns>
    public async Task<RequestQuoteItemModel> PrepareRequestQuoteItemModelAsync(RequestQuote requestQuote, RequestQuoteItem item, Currency currentCurrency)
    {
        var product = await _productService.GetProductByIdAsync(item.ProductId);

        var unitPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(item.RequestedUnitPrice, currentCurrency);
        unitPrice = _priceCalculationService.Round(unitPrice, currentCurrency.RoundingType);

        return new RequestQuoteItemModel
        {
            Id = item.Id,
            Quantity = item.RequestedQty,
            OriginalProductCost = await _priceFormatter.FormatPriceAsync(item.OriginalProductPrice, true, currentCurrency),
            ProductName = product != null ? await _localizationService.GetLocalizedAsync(product, x => x.Name) : await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.ProductDeleted"),
            ProductSeName = product != null ? await _urlRecordService.GetSeNameAsync(product) : string.Empty,
            UnitPrice = unitPrice,
            UnitPriceText = await _priceFormatter.FormatPriceAsync(unitPrice, true, currentCurrency),
            AttributeInfo = product != null ? await _productAttributeFormatter.FormatAttributesAsync(product, item.ProductAttributesXml) : string.Empty,
            PictureUrl = product != null ? await GetPictureUrlAsync(product, item.ProductAttributesXml, _mediaSettings.CartThumbPictureSize) : await _pictureService.GetDefaultPictureUrlAsync(_mediaSettings.CartThumbPictureSize),
            Editable = requestQuote.Status == 0
        };
    }

    /// <summary>
    /// Prepare a request a quote model
    /// </summary>
    /// <param name="requestId">The request a quote identifier</param>
    /// <param name="model">Request a quote model to extend</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote model
    /// </returns>
    public async Task<RequestQuoteModel> PrepareRequestQuoteModelAsync(int requestId, RequestQuoteModel model = null)
    {
        return await PrepareRequestQuoteModelAsync(await _rfqService.GetRequestQuoteByIdAsync(requestId), model: model);
    }

    /// <summary>
    /// Prepare a request a quote model
    /// </summary>
    /// <param name="model">Request a quote model to extend</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote model
    /// </returns>
    public async Task<RequestQuoteModel> PrepareRequestQuoteModelAsync(RequestQuoteModel model)
    {
        return await PrepareRequestQuoteModelAsync(await _rfqService.GetRequestQuoteByIdAsync(model.Id), model: model);
    }

    /// <summary>
    /// Prepare a quote model
    /// </summary>
    /// <param name="quoteId">The quote identifier</param>
    /// <param name="model">Quote model to extend</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote model
    /// </returns>
    public async Task<QuoteModel> PrepareQuoteModelAsync(int quoteId, QuoteModel model = null)
    {
        return await PrepareQuoteModelAsync(await _rfqService.GetQuoteByIdAsync(quoteId), model: model);
    }

    #endregion
}