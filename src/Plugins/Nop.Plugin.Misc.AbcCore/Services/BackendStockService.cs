using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Seo;
using SevenSpikes.Nop.Plugins.StoreLocator.Services;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class BackendStockService : IBackendStockService
    {
        public static readonly int WAREHOUSE_INT = -1;
        public static string API_URL = "http://160.160.160.20:63000";

        // STOCK TABLE CONSTANTS
        private static readonly string _backendStockTable = "DA7_INVENTORY_STORE";
        private static readonly string _backendStockItemNum = "KEY_ITEM_NUMBER";
        private static readonly string _backendStockBranch = "KEY_BRANCH";
        private static readonly string _backendStockQuantity = "QTY_ON_HAND";

        // INVENTORY TABLE CONSTANTS
        private static readonly string _backendInvTable = BackendDbConstants.InvTable;
        private static readonly string _backendInvItemNum = BackendDbConstants.InvItemNumber;
        private static readonly string _backendInvModel = BackendDbConstants.InvModel;

        private IShopService _shopService;
        private IProductService _productService;
        private IUrlRecordService _urlRecordService;
        private IRepository<ShopAbc> _shopAbcRepository;
        private IRepository<ProductAbcDescription> _backendIdRepository;
        private CoreSettings _settings;
        private ILogger _logger;
        private IStoreService _storeService;

        public BackendStockService(IShopService shopService,
            IProductService productService,
            IUrlRecordService urlRecordService,
            IRepository<ShopAbc> shopAbcRepository,
            IRepository<ProductAbcDescription> backendIdRepository,
            ILogger logger,
            CoreSettings settings,
            IStoreService storeService)
        {
            _shopService = shopService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _shopAbcRepository = shopAbcRepository;
            _backendIdRepository = backendIdRepository;
            _logger = logger;
            _settings = settings;
            _storeService = storeService;
        }

        public async Task<Dictionary<int, int>> GetStockAsync(int productId)
        {
            Product product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return null;
            }
            string upc = product.Sku;
            if (String.IsNullOrEmpty(upc))
            {
                return null;
            }

            Dictionary<int, int> branchQuantityDict = new Dictionary<int, int>();
            // go to backend and get quantity
            using (OdbcConnection dbConnection = _settings.GetBackendDbConnection())
            {
                OdbcCommand dbCommand = dbConnection.CreateCommand();

                dbConnection.Open();
                dbCommand.CommandText =
                    "SELECT DISTINCT" +
                        " St." + _backendStockBranch +
                        ", St." + _backendStockQuantity +
                    " FROM " + _backendStockTable + " St" +
                    " LEFT JOIN " + _backendInvTable + " Inv ON St." + _backendStockItemNum + " = Inv." + _backendInvItemNum +
                        " WHERE" +
                        " Inv." + _backendInvModel + " = '" + upc + "';";
                using (OdbcDataReader reader = dbCommand.ExecuteReader())
                {
                    // read in the information
                    while (reader.Read())
                    {
                        string backendBranchId = reader.GetString(0);
                        var shopIdEnumerable = _shopAbcRepository.Table
                            .Where(s => s.AbcId == backendBranchId)
                            .Select(s => s.ShopId);
                        if (shopIdEnumerable.Any() || backendBranchId.Trim().ToLower() == "abc")
                        {
                            int shopId = shopIdEnumerable.First();
                            int backendStockQuantity = reader.GetInt32(1);
                            branchQuantityDict.Add(shopId, backendStockQuantity);
                        }
                        else if (backendBranchId.Trim().ToLower() == "abc")
                        {
                            int backendStockQuantity = reader.GetInt32(1);
                            branchQuantityDict.Add(WAREHOUSE_INT, backendStockQuantity);
                        }
                    }
                }

                dbCommand.Dispose();
                dbConnection.Close();
                // return a list of stock + corresponding shop item
            }
            return branchQuantityDict;
        }

        public async Task<StockResponse> GetApiStockAsync(int productId)
        {
            Product product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return null;
            }

            string backendId = _backendIdRepository.Table
                .Where(pad => pad.Product_Id == productId)
                .Select(pad => pad.AbcItemNumber)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(backendId))
            {
                return null;
            }

            if (_settings.AreExternalCallsSkipped)
            {
                return await ReturnMockData();
            }

            // call backend service
            string xmlRequestString = await BuildXmlRequestStringAsync(backendId);
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(xmlRequestString, Encoding.UTF8, "text/xml");
                try
                {
                    var response = client.PostAsync(API_URL, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string xmlResponse = response.Content.ReadAsStringAsync().Result;

                        byte[] byteArray = Encoding.UTF8.GetBytes(xmlResponse);
                        StreamReader reader = new StreamReader(new MemoryStream(byteArray));
                        XmlSerializer serializer = new XmlSerializer(typeof(StockResponse));
                        StockResponse stockResponse = (StockResponse)serializer.Deserialize(reader);

                        // do additional parsing: add nop information
                        stockResponse.ProductId = productId;

                        for (int i = stockResponse.ProductStocks.Count - 1; i >= 0; i--)
                        {
                            string branchId = stockResponse.ProductStocks[i].BranchId;
                            ShopAbc shopAbc = _shopAbcRepository.Table
                                .Where(s => s.AbcId == branchId)
                                .Select(s => s).FirstOrDefault();

                            // remove stores that were not imported
                            if (shopAbc == null)
                            {
                                stockResponse.ProductStocks.RemoveAt(i);
                            }
                            else
                            {
                                // add shop & url information
                                stockResponse.ProductStocks[i].Shop = await _shopService.GetShopByIdAsync(shopAbc.ShopId);
                                var urlRecords = await _urlRecordService.GetAllUrlRecordsAsync();
                                stockResponse.ProductStocks[i].ShopUrl
                                    = urlRecords.Where(u => u.EntityName == "Shop" && u.EntityId == shopAbc.ShopId)
                                        .Select(u => u.Slug).FirstOrDefault();
                            }
                        }
                        return stockResponse;
                    }
                    return null;
                }
                // TODO: catch multiple exceptions and handle them all
                catch (Exception exception)
                {
                    string internalException = "";
                    if (exception.InnerException != null)
                    {
                        internalException = exception.InnerException.Message;
                    }
                    await _logger.InsertLogAsync(Core.Domain.Logging.LogLevel.Information, "pickup api, error: " + exception.Message, internalException);
                    return null;
                }
            }
        }

        private async Task<StockResponse> ReturnMockData()
        {
            await _logger.WarningAsync("BackendStockService: External call skipped, mocking response.");

            StockResponse stockResponse = new StockResponse();
            stockResponse.ProductStocks = new List<ProductStock>();

            ShopAbc shopAbc = _shopAbcRepository.Table
                            .Where(s => s.AbcId == "1")
                            .Select(s => s).FirstOrDefault();
            var urlRecords = await _urlRecordService.GetAllUrlRecordsAsync();
            var shopUrl = urlRecords.Where(u => u.EntityName == "Shop" && u.EntityId == shopAbc.ShopId)
                                    .Select(u => u.Slug)
                                    .FirstOrDefault();

            // Bloomfield Township
            ProductStock bloomfieldTownshipMockShop = new ProductStock()
            {
                Shop = await _shopService.GetShopByIdAsync(440),
                Available = true,
                Quantity = 1
            };
            stockResponse.ProductStocks.Add(bloomfieldTownshipMockShop);

            ProductStock rochesterMockShop = new ProductStock()
            {
                Shop = await _shopService.GetShopByIdAsync(477),
                Available = false,
                Quantity = 0
            };
            stockResponse.ProductStocks.Add(rochesterMockShop);

            ProductStock troyMockShop = new ProductStock()
            {
                Shop = await _shopService.GetShopByIdAsync(445),
                Available = true,
                Quantity = 1
            };
            stockResponse.ProductStocks.Add(troyMockShop);

            ProductStock farmingtonHillsMockShop = new ProductStock()
            {
                Shop = await _shopService.GetShopByIdAsync(443),
                Available = true,
                Quantity = 1
            };
            stockResponse.ProductStocks.Add(farmingtonHillsMockShop);

            return stockResponse;
        }

        public async Task<bool> AvailableAtStore(int shopId, int productId)
        {
            string backendId = _shopAbcRepository.Table
                .Where(sabc => sabc.ShopId == shopId)
                .Select(sabc => sabc.AbcId).FirstOrDefault();

            StockResponse response = await GetApiStockAsync(productId);

            if (response != null)
            {
                bool availableAtStore = response.ProductStocks
                    .Where(ps => ps.BranchId == backendId)
                    .Select(ps => ps.Available).FirstOrDefault();

                return availableAtStore;
            }
            return false;
        }
        private async Task<string> BuildXmlRequestStringAsync(string backendId)
        {
            // Mickey Shorr needs a specific code. ABC/Haw doesn't need it, so A is default.
            var company = (await _storeService.GetAllStoresAsync()).Any(s => s.Name.ToLower().Contains("mickey")) ? "S" : "A";

            XElement xml = new XElement("Request",
                new XElement("InventoryPickup",
                    new XElement("ItemNumber", backendId),
                    new XElement("Company", company)
                )
            );
            return xml.ToString();
        }
    }
}
