using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core;
using Nop.Services.Logging;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Seo;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcExportOrder.Services
{
    public class IsamOrderService : IIsamOrderService
    {
        private readonly string HEADER_TABLE_NAME = "YAHOO_HEADER";
        private readonly string DETAIL_TABLE_NAME = "YAHOO_DETAIL";
        private readonly string SHIPTO_TABLE_NAME = "YAHOO_SHIPTO";

        private List<string> _headerCols = new List<string>();
        private List<string> _detailCols = new List<string>();
        private List<string> _shiptoCols = new List<string>();

        private List<OdbcParameter> _headerParams = new List<OdbcParameter>();
        private List<OdbcParameter> _detailParams = new List<OdbcParameter>();
        private List<OdbcParameter> _shiptoParams = new List<OdbcParameter>();
        private HashSet<string> _canBeNullSet = new HashSet<string> { "PICKUP_BRANCH", "PICKUP_DROP", "GIFT_CARD", "AUTH" };


        private ISettingService _settingService;
        private IEncryptionService _encryptionService;
        private IBaseService _baseIsamService;
        private IProductAttributeParser _productAttributeParser;
        private IAttributeUtilities _attributeUtilities;
        private IRepository<ShopAbc> _shopAbcRepository;
        private IRepository<CustomerShopMapping> _customerShopMappingRepository;
        private IRepository<WarrantySku> _warrantySkuRepository;
        private IRepository<ProductAbcDescription> _productAbcDescriptionRepository;
        private IRepository<GiftCardUsageHistory> _giftCardUsageHistoryRepository;
        private IWorkContext _workContext;
        private IStoreContext _storeContext;
        private IGiftCardService _giftCardService;
        private readonly ExportOrderSettings _settings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICountryService _countryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IRepository<Product> _productRepository;
        private readonly ICustomShopService _customShopService;
        private readonly IIsamGiftCardService _isamGiftCardService;
        private readonly IYahooService _yahooService;

        public IsamOrderService(
            ISettingService settingService,
            IEncryptionService encryptionService,
            IBaseService baseIsamService,
            IProductAttributeParser productAttributeParser,
            IAttributeUtilities attributeUtilities,
            IRepository<ShopAbc> shopAbcRepository,
            IRepository<CustomerShopMapping> customerShopMappingRepository,
            IRepository<WarrantySku> warrantySkuRepository,
            IRepository<ProductAbcDescription> productAbcDescriptionRepository,
            IRepository<GiftCardUsageHistory> giftCardUsageHistoryRepository,
            IStoreContext storeContext,
            IWorkContext workContext,
            IGiftCardService giftCardService,
            ExportOrderSettings settings,
            ILogger logger,
            IOrderService orderService,
            IPriceCalculationService priceCalculationService,
            IAddressService addressService,
            IProductService productService,
            IStateProvinceService stateProvinceService,
            ICountryService countryService,
            IUrlRecordService urlRecordService,
            IProductAttributeService productAttributeService,
            IRepository<Product> productRepository,
            ICustomShopService customShopService,
            IIsamGiftCardService isamGiftCardService,
            IYahooService yahooService
        )
        {
            _settingService = settingService;
            _encryptionService = encryptionService;
            _baseIsamService = baseIsamService;
            _productAttributeParser = productAttributeParser;
            _attributeUtilities = attributeUtilities;
            _shopAbcRepository = shopAbcRepository;
            _customerShopMappingRepository = customerShopMappingRepository;
            _warrantySkuRepository = warrantySkuRepository;
            _productAbcDescriptionRepository = productAbcDescriptionRepository;
            _giftCardUsageHistoryRepository = giftCardUsageHistoryRepository;
            _storeContext = storeContext;
            _workContext = workContext;
            _giftCardService = giftCardService;
            _settings = settings;
            _logger = logger;
            _orderService = orderService;
            _priceCalculationService = priceCalculationService;
            _addressService = addressService;
            _productService = productService;
            _stateProvinceService = stateProvinceService;
            _countryService = countryService;
            _urlRecordService = urlRecordService;
            _productAttributeService = productAttributeService;
            _productRepository = productRepository;
            _customShopService = customShopService;
            _isamGiftCardService = isamGiftCardService;
            _yahooService = yahooService;

            InitializeAllColParams();
        }

        /// <summary>
        /// initializes parameters to  be ready for insert
        /// </summary>
        private void InitializeAllColParams()
        {
            _headerCols.Add("ID");
            _headerParams.Add(new OdbcParameter("@ID", OdbcType.VarChar));
            _headerCols.Add("DATESTAMP");
            _headerParams.Add(new OdbcParameter("@DATESTAMP", OdbcType.VarChar));
            _headerCols.Add("BILL_FULL");
            _headerParams.Add(new OdbcParameter("@BILL_FULL", OdbcType.VarChar));
            _headerCols.Add("BILL_FIRST");
            _headerParams.Add(new OdbcParameter("@BILL_FIRST", OdbcType.VarChar));
            _headerCols.Add("BILL_LAST");
            _headerParams.Add(new OdbcParameter("@BILL_LAST", OdbcType.VarChar));
            _headerCols.Add("BILL_ADDRESS_1");
            _headerParams.Add(new OdbcParameter("@BILL_ADDRESS_1", OdbcType.VarChar));
            _headerCols.Add("BILL_ADDRESS_2");
            _headerParams.Add(new OdbcParameter("@BILL_ADDRESS_2", OdbcType.VarChar));
            _headerCols.Add("BILL_CITY");
            _headerParams.Add(new OdbcParameter("@BILL_CITY", OdbcType.VarChar));
            _headerCols.Add("BILL_STATE");
            _headerParams.Add(new OdbcParameter("@BILL_STATE", OdbcType.VarChar));
            _headerCols.Add("BILL_ZIP");
            _headerParams.Add(new OdbcParameter("@BILL_ZIP", OdbcType.VarChar));
            _headerCols.Add("BILL_COUNTRY");
            _headerParams.Add(new OdbcParameter("@BILL_COUNTRY", OdbcType.VarChar));
            _headerCols.Add("BILL_PHONE");
            _headerParams.Add(new OdbcParameter("@BILL_PHONE", OdbcType.VarChar));
            _headerCols.Add("BILL_EMAIL");
            _headerParams.Add(new OdbcParameter("@BILL_EMAIL", OdbcType.VarChar));
            _headerCols.Add("CARD_NAME");
            _headerParams.Add(new OdbcParameter("@CARD_NAME", OdbcType.VarChar));
            _headerCols.Add("CARD_NUMBER");
            _headerParams.Add(new OdbcParameter("@CARD_NUMBER", OdbcType.VarChar));
            _headerCols.Add("CARD_EXPIRY");
            _headerParams.Add(new OdbcParameter("@CARD_EXPIRY", OdbcType.VarChar));
            _headerCols.Add("TAX_CHARGE");
            _headerParams.Add(new OdbcParameter("@TAX_CHARGE", OdbcType.Decimal));
            _headerCols.Add("SHIPPING_CHARGE");
            _headerParams.Add(new OdbcParameter("@SHIPPING_CHARGE", OdbcType.Decimal));
            _headerCols.Add("TOTAL");
            _headerParams.Add(new OdbcParameter("@TOTAL", OdbcType.Decimal));
            _headerCols.Add("BILL_CVS");
            _headerParams.Add(new OdbcParameter("@BILL_CVS", OdbcType.VarChar));
            _headerCols.Add("IP");
            _headerParams.Add(new OdbcParameter("@IP", OdbcType.VarChar));
            _headerCols.Add("GIFT_CARD");
            _headerParams.Add(new OdbcParameter("@GIFT_CARD", OdbcType.VarChar));
            _headerCols.Add("GIFT_AMT_USED");
            _headerParams.Add(new OdbcParameter("@GIFT_AMT_USED", OdbcType.Decimal));
            _headerCols.Add("AUTH");
            _headerParams.Add(new OdbcParameter("@AUTH", OdbcType.VarChar));
            _headerCols.Add("HOME_DELIVERY");
            _headerParams.Add(new OdbcParameter("@HOME_DELIVERY", OdbcType.VarChar));
            _headerCols.Add("CC_REFNO");
            _headerParams.Add(new OdbcParameter("@CC_REFNO", OdbcType.VarChar));

            _detailCols.Add("ID");
            _detailParams.Add(new OdbcParameter("@ID", OdbcType.VarChar));
            _detailCols.Add("ITEM_LINE");
            _detailParams.Add(new OdbcParameter("@ITEM_LINE", OdbcType.Int));
            _detailCols.Add("PKG_CNTR");
            _detailParams.Add(new OdbcParameter("@PKG_CNTR", OdbcType.VarChar));
            _detailCols.Add("ITEM_ID");
            _detailParams.Add(new OdbcParameter("@ITEM_ID", OdbcType.VarChar));
            _detailCols.Add("ITEM_CODE");
            _detailParams.Add(new OdbcParameter("@ITEM_CODE", OdbcType.VarChar));
            _detailCols.Add("ITEM_QUANTITY");
            _detailParams.Add(new OdbcParameter("@ITEM_QUANTITY", OdbcType.Int));
            _detailCols.Add("ITEM_UNIT_PRICE");
            _detailParams.Add(new OdbcParameter("@ITEM_UNIT_PRICE", OdbcType.Decimal));
            _detailCols.Add("ITEM_DESCRIPTION");
            _detailParams.Add(new OdbcParameter("@ITEM_DESCRIPTION", OdbcType.VarChar));
            _detailCols.Add("ITEM_URL");
            _detailParams.Add(new OdbcParameter("@ITEM_URL", OdbcType.VarChar));
            _detailCols.Add("PICKUP_BRANCH");
            _detailParams.Add(new OdbcParameter("@PICKUP_BRANCH", OdbcType.VarChar));
            _detailCols.Add("PICKUP_DROP");
            _detailParams.Add(new OdbcParameter("@PICKUP_DROP", OdbcType.VarChar));

            _shiptoCols.Add("ID");
            _shiptoParams.Add(new OdbcParameter("@ID", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_FULL");
            _shiptoParams.Add(new OdbcParameter("@SHIP_FULL", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_FIRST");
            _shiptoParams.Add(new OdbcParameter("@SHIP_FIRST", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_LAST");
            _shiptoParams.Add(new OdbcParameter("@SHIP_LAST", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_ADDRESS_1");
            _shiptoParams.Add(new OdbcParameter("@SHIP_ADDRESS_1", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_ADDRESS_2");
            _shiptoParams.Add(new OdbcParameter("@SHIP_ADDRESS_2", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_CITY");
            _shiptoParams.Add(new OdbcParameter("@SHIP_CITY", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_STATE");
            _shiptoParams.Add(new OdbcParameter("@SHIP_STATE", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_ZIP");
            _shiptoParams.Add(new OdbcParameter("@SHIP_ZIP", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_COUNTRY");
            _shiptoParams.Add(new OdbcParameter("@SHIP_COUNTRY", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_PHONE");
            _shiptoParams.Add(new OdbcParameter("@SHIP_PHONE", OdbcType.VarChar));
            _shiptoCols.Add("SHIP_EMAIL");
            _shiptoParams.Add(new OdbcParameter("@SHIP_EMAIL", OdbcType.VarChar));
            _shiptoCols.Add("SHIPPING");
            _shiptoParams.Add(new OdbcParameter("@SHIPPING", OdbcType.VarChar));
            _shiptoCols.Add("COMMENTS");
            _shiptoParams.Add(new OdbcParameter("@COMMENTS", OdbcType.VarChar));
        }

        /// <summary>
        /// Inserts an entire order from nopcommerce into isam
        /// </summary>
        /// <param name="order"></param>
        public async Task InsertOrderAsync(Order order)
        {
            var shipToRows = await _yahooService.GetYahooShipToRowsAsync(order);
            foreach (var row in shipToRows)
            {
                InsertUsingService(
                    SHIPTO_TABLE_NAME,
                    _shiptoCols,
                    _shiptoParams,
                    row.ToStringValues()
                );
            }

            var headerRows = await _yahooService.GetYahooHeaderRowsAsync(order);
            foreach (var row in headerRows)
            {
                InsertUsingService(
                    HEADER_TABLE_NAME,
                    _headerCols,
                    _headerParams,
                    row.ToStringValues()
                );
            }

            var detailRows = await _yahooService.GetYahooDetailRowsAsync(order);
            foreach (var row in detailRows)
            {
                InsertUsingService(
                    DETAIL_TABLE_NAME,
                    _detailCols,
                    _detailParams,
                    row.ToStringValues()
                );
            }

            _baseIsamService.ExecuteBatch();

            // store amount used before insert order
            // insert order changes the amount in the object memory to properly calculate gift card amounts
            // for multiple orders
            decimal giftCardAmtUsed = 0;
            var giftCardUsageHistory = await _giftCardService.GetGiftCardUsageHistoryAsync(order);
            if (giftCardUsageHistory.Any())
            {
                giftCardAmtUsed = giftCardUsageHistory.OrderByDescending(gcu => gcu.CreatedOnUtc).FirstOrDefault().UsedValue;
            }

            // if there is a gift card, update gift card amt in isam
            if (giftCardUsageHistory.Any())
            {
                GiftCardUsageHistory orderGcUsage = giftCardUsageHistory.FirstOrDefault();
                GiftCard orderGiftCard = await _giftCardService.GetGiftCardByIdAsync(orderGcUsage.GiftCardId);

                var isamGiftCardInfo = _isamGiftCardService.GetGiftCardInfo(orderGiftCard.GiftCardCouponCode);
                GiftCard isamGiftCard = isamGiftCardInfo.GiftCard;
                decimal isamGiftCardAmtUsed = isamGiftCardInfo.AmountUsed;
                _isamGiftCardService.UpdateGiftCardAmt(isamGiftCard, isamGiftCardAmtUsed + giftCardAmtUsed);

                await _giftCardUsageHistoryRepository.DeleteAsync(orderGcUsage);
                await _giftCardService.UpdateGiftCardAsync(orderGiftCard);
            }
        }

        private void InsertUsingService(string tableName, List<string> cols, List<OdbcParameter> colParams, List<string> values)
        {
            if (colParams.Count != values.Count)
            {
                throw new Exception("coder you messed up, colparam size must equal value size, c: " + colParams.Count + " v: " + values.Count);
            }

            List<string> insertCols = new List<string>();
            List<OdbcParameter> insertParams = new List<OdbcParameter>();

            for (int i = 0; i < values.Count; ++i)
            {
                if (!String.IsNullOrEmpty(values[i]))
                {
                    colParams[i].Value = values[i];
                    insertParams.Add(colParams[i]);
                    insertCols.Add(cols[i]);
                }
                else if (colParams[i].OdbcType == OdbcType.VarChar && !_canBeNullSet.Contains(cols[i]))
                {
                    colParams[i].Value = " ";
                    insertParams.Add(colParams[i]);
                    insertCols.Add(cols[i]);
                }
            }

            _baseIsamService.Insert(_settings.TablePrefix + tableName, insertCols, insertParams, true);
        }
    }
}
