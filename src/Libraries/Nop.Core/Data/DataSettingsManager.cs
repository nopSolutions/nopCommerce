using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Nop.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Nop.Core.Data
{
    /// <summary>
    /// Represents the data settings manager
    /// </summary>
    public partial class DataSettingsManager
    {
        private static bool? _databaseIsInstalled;

        private static readonly string[] _tableNames = new string[]
        {
            "AclRecord",
            "ActivityLog",
            "ActivityLogType",
            "Address",
            "AddressAttribute",
            "AddressAttributeValue",
            "Affiliate",
            "BackInStockSubscription",
            "BlogComment",
            "BlogPost",
            "Campaign",
            "Category",
            "CategoryTemplate",
            "CheckoutAttribute",
            "CheckoutAttributeValue",
            "Country",
            "CrossSellProduct",
            "Currency",
            "Customer",
            "Customer_CustomerRole_Mapping",
            "CustomerAddresses",
            "CustomerAttribute",
            "CustomerAttributeValue",
            "CustomerPassword",
            "CustomerRole",
            "DeliveryDate",
            "Discount",
            "Discount_AppliedToCategories",
            "Discount_AppliedToManufacturers",
            "Discount_AppliedToProducts",
            "DiscountRequirement",
            "DiscountUsageHistory",
            "Download",
            "EmailAccount",
            "ExternalAuthenticationRecord",
            "Forums_Forum",
            "Forums_Group",
            "Forums_Post",
            "Forums_PostVote",
            "Forums_PrivateMessage",
            "Forums_Subscription",
            "Forums_Topic",
            "GdprConsent",
            "GdprLog",
            "GenericAttribute",
            "GiftCard",
            "GiftCardUsageHistory",
            "Language",
            "LocaleStringResource",
            "LocalizedProperty",
            "Log",
            "Manufacturer",
            "ManufacturerTemplate",
            "MeasureDimension",
            "MeasureWeight",
            "MessageTemplate",
            "News",
            "NewsComment",
            "NewsLetterSubscription",
            "Order",
            "OrderItem",
            "OrderNote",
            "PermissionRecord",
            "PermissionRecord_Role_Mapping",
            "Picture",
            "PictureBinary",
            "Poll",
            "PollAnswer",
            "PollVotingRecord",
            "PredefinedProductAttributeValue",
            "Product",
            "Product_Category_Mapping",
            "Product_Manufacturer_Mapping",
            "Product_Picture_Mapping",
            "Product_ProductAttribute_Mapping",
            "Product_ProductTag_Mapping",
            "Product_SpecificationAttribute_Mapping",
            "ProductAttribute",
            "ProductAttributeCombination",
            "ProductAttributeValue",
            "ProductAvailabilityRange",
            "ProductReview",
            "ProductReview_ReviewType_Mapping",
            "ProductReviewHelpfulness",
            "ProductTag",
            "ProductTemplate",
            "ProductWarehouseInventory",
            "QueuedEmail",
            "RecurringPayment",
            "RecurringPaymentHistory",
            "RelatedProduct",
            "ReturnRequest",
            "ReturnRequestAction",
            "ReturnRequestReason",
            "ReviewType",
            "RewardPointsHistory",
            "ScheduleTask",
            "SearchTerm",
            "Setting",
            "Shipment",
            "ShipmentItem",
            "ShippingByWeightByTotalRecord",
            "ShippingMethod",
            "ShippingMethodRestrictions",
            "ShoppingCartItem",
            "SpecificationAttribute",
            "SpecificationAttributeOption",
            "StateProvince",
            "StockQuantityHistory",
            "Store",
            "StoreMapping",
            "StorePickupPoint",
            "TaxCategory",
            "TaxRate",
            "TaxTransactionLog",
            "TierPrice",
            "Topic",
            "TopicTemplate",
            "UrlRecord",
            "Vendor",
            "VendorAttribute",
            "VendorAttributeValue",
            "VendorNote",
            "Warehouse"
        };

        private static async Task<bool> IsDatabaseInitialized(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new EmptyConnectionStringException();

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                if (string.IsNullOrEmpty(builder.InitialCatalog))
                    throw new ConnectionStringBadFormatException(connectionString);

                var tables = new List<string>();

                //just try to connect
                using (var conn = new SqlConnection(connectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT TABLE_NAME
                            FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = @PARAM_CATALOG";
                        cmd.Parameters.AddWithValue("@PARAM_CATALOG", builder.InitialCatalog);

                        await cmd.Connection.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    tables.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                }

                return tables.Count == _tableNames.Length && _tableNames.SequenceEqual(tables);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load data settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use the default settings file</param>
        /// <param name="reloadSettings">Whether to reload data, if they already loaded</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>Data settings</returns>
        public static DataSettings LoadSettings(string filePath = null, bool reloadSettings = false, INopFileProvider fileProvider = null)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
            filePath = filePath ?? fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //check whether file exists
            if (!fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = fileProvider.MapPath(NopDataSettingsDefaults.ObsoleteFilePath);
                if (!fileProvider.FileExists(filePath))
                    return new DataSettings();

                //get data settings from the old txt file
                var dataSettings = new DataSettings();
                using (var reader = new StringReader(fileProvider.ReadAllText(filePath, Encoding.UTF8)))
                {
                    string settingsLine;
                    while ((settingsLine = reader.ReadLine()) != null)
                    {
                        var separatorIndex = settingsLine.IndexOf(':');
                        if (separatorIndex == -1)
                            continue;

                        var key = settingsLine.Substring(0, separatorIndex).Trim();
                        var value = settingsLine.Substring(separatorIndex + 1).Trim();

                        switch (key)
                        {
                            case "DataProvider":
                                dataSettings.DataProvider = Enum.TryParse(value, true, out DataProviderType providerType) ? providerType : DataProviderType.Unknown;
                                continue;
                            case "DataConnectionString":
                                dataSettings.DataConnectionString = value;
                                continue;
                            default:
                                dataSettings.RawDataSettings.Add(key, value);
                                continue;
                        }
                    }
                }

                //save data settings to the new file
                SaveSettings(dataSettings, fileProvider);

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                Singleton<DataSettings>.Instance = dataSettings;
                return Singleton<DataSettings>.Instance;
            }

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);

            return Singleton<DataSettings>.Instance;
        }

        /// <summary>
        /// Save data settings to the file
        /// </summary>
        /// <param name="settings">Data settings</param>
        /// <param name="fileProvider">File provider</param>
        public static void SaveSettings(DataSettings settings, INopFileProvider fileProvider = null)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(NopDataSettingsDefaults.FilePath);

            //create file if not exists
            fileProvider.CreateFile(filePath);

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Reset "database is installed" cached information
        /// </summary>
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }

        /// <summary>
        /// Gets a value indicating whether database is already installed
        /// </summary>
        [Obsolete("Use method version instead")]
        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                {
                    var dbConfig = EngineContext.Current.Resolve<IOptions<DataSettings>>() ?? throw new NopDbConfigOptionNotConfiguredException();

                    _databaseIsInstalled = IsDatabaseInitialized(dbConfig.Value.DataConnectionString).GetAwaiter().GetResult();
                }
                //_databaseIsInstalled = !string.IsNullOrEmpty(LoadSettings(reloadSettings: true)?.DataConnectionString);

                return _databaseIsInstalled.Value;
            }
        }

        public static bool GetDatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var dbConfig = EngineContext.Current.Resolve<IOptions<DataSettings>>();
                return GetDatabaseIsInstalled(dbConfig.Value);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(IServiceProvider serviceProvider)
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var dbConfig = (IOptions<DataSettings>)serviceProvider.GetService(typeof(IOptions<DataSettings>));

                return GetDatabaseIsInstalled(dbConfig.Value);
            }

            return _databaseIsInstalled.Value;
        }

        public static bool GetDatabaseIsInstalled(DataSettings dbConfig)
        {
            if (!_databaseIsInstalled.HasValue)
            {
                _databaseIsInstalled = IsDatabaseInitialized(dbConfig.DataConnectionString).GetAwaiter().GetResult();
            }

            return _databaseIsInstalled.Value;
        }
    }
}