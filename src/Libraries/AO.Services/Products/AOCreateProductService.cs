using AO.Services.Domain;
using AO.Services.Products.Models;
using AO.Services.Services;
using Microsoft.Extensions.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static AO.Services.Utilities;

namespace AO.Services.Products
{
    public class AOCreateProductService : IAOCreateProductService
    {
        #region Private variables
        private swaggerClient _client;
        private readonly IConfiguration _configuration;
        private int _danishLanguageId = 2;
        private readonly ILogger _logger;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IUrlRecordService _urlRecordService;
        private IList<AOProductExtensionData> _productExtensionData;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IRepository<AOProductExtensionData> _productExtensionDataRepository;
        int _createdCount = 0, _updatedCount = 0, _variantsNodeList = 0, _skippedVariant = 0, _productCreated = 0;
        #endregion

        public AOCreateProductService(IConfiguration configuration, ILogger logger, IProductAttributeService productAttributeService, IUrlRecordService urlRecordService, IList<AOProductExtensionData> productExtensionData, IRepository<AOProductExtensionData> productExtensionDataRepository, IProductService productService, IProductAttributeParser productAttributeParser)
        {
            _configuration = configuration;
            _logger = logger;
            _client = new swaggerClient(_configuration["ApiSettings:ApiUrl"], SwaggerClientWrapper.Instance);
            _productAttributeService = productAttributeService;

            SetClient();
            _urlRecordService = urlRecordService;
            _productExtensionData = productExtensionData;
            _productExtensionDataRepository = productExtensionDataRepository;
            _productService = productService;
            _productAttributeParser = productAttributeParser;
        }

        public async Task<ProductDto> CreateProductAsync(VariantData variantData, string orgItemNumber, string updaterName)
        {
            ProductDto product = new ProductDto()
            {
                Name = variantData.Title,
                Price = Convert.ToDouble(variantData.RetailPrice),
                Admin_comment = $"Product created through sync ({updaterName}) with use of variant data from '{variantData.Brand}'.{Environment.NewLine}{DateTime.Now:dd-MM-yyyy tt:mm:ss}",
                Short_description = variantData.OriginalTitle,
                Sku = orgItemNumber,
                Manufacturer_part_number = variantData.OrgItemNumber,
                Published = false,
                Mark_as_new = true,
                Visible_individually = true,
                Product_type_id = 5, // 5==Simple product type
                Product_template_id = 1,
                Baseprice_unit_id = 3,
                Baseprice_base_unit_id = 3,
                Created_on_utc = DateTime.UtcNow,
                Manage_inventory_method_id = 2, // 2==Track inventory by product attributes
                Order_maximum_quantity = 1000,
                Allow_customer_reviews = true,
                Display_stock_availability = true,
                Display_stock_quantity = true,
                Is_ship_enabled = true,
                Tax_category_id = 6, // 6==Dansk moms'
                Delivery_date_id = 2 // 2 = 2-4 days
            };

            var newProduct = await _client.Create57Async(product);

            await _productExtensionDataRepository.InsertAsync(new AOProductExtensionData()
            {
                ForceOutOfGoogleShopping = false,
                ForceOutOfPriceRunner = false,
                ProductId = product.Id,
                StatusId = (int)ProductStatus.Crawled,
                InventoryCountLastDone = Convert.ToDateTime("01-01-1970")
            });

            await AddUrlRecord(newProduct.Name, "Product", newProduct.Id, 0);
            await AddUrlRecord(newProduct.Name, "Product", newProduct.Id, _danishLanguageId);

            await _client.SaveLocalizedValueAsync(_danishLanguageId, newProduct.Id, "Product", "Name", variantData.Title);

            newProduct.Updated_on_utc = DateTime.UtcNow;
            await _client.Update48Async(newProduct);

            return newProduct;
        }

        public async Task<ProductDto> GetProductAsync(string frilivSKU)
        {
            ProductDto product = null;
            try
            {
                //swaggerClient client = await GetClientAsync();
                product = await _client.GetProductBySkuAsync(frilivSKU);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404)
                {
                    return null;
                }
                throw;
            }
            return product;
        }

        public async Task CreateVariantAsync(VariantData variantData)
        {
            var orgItemNumber = GetOrgItemNumber(variantData.OrgItemNumber);
            var product = await GetOrCreateProduct(variantData, orgItemNumber);

            if (IsStatusValidForSyncJob(product.Id) == false)
            {
                // We should be creating a new variant here, but the product has status to not do that
                _skippedVariant++;
                return;
            }

            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);

            ProductAttributeMapping productAttributeMappingSize = await GetOrCreateProductSizeMapping(product, productAttributeMappings);
            ProductAttributeMapping productAttributeMappingColor = await GetOrCreateProductColorMapping(product, productAttributeMappings);

            ProductAttributeValue sizeValue = await GetOrCreateSizeValue(variantData, product, productAttributeMappingSize);
            ProductAttributeValue colorValue = await GetOrCreateColorValue(variantData, product, productAttributeMappingColor);

            var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true, new List<int> { sizeValue.Id, colorValue.Id });
  

            // Do we now have any?
            productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);

            foreach (var attributesXml in allAttributesXml)
            {
                var doc = new XmlDocument();
                doc.LoadXml(attributesXml);
                var productAttributeNode = doc.GetElementsByTagName("ProductAttribute");
                if (productAttributeNode.Count != 2)
                {
                    continue;
                }

                int nopColorId = 0, nopsizeId = 0;
                int productAttributeId = Int32.Parse(productAttributeNode[0].Attributes["ID"].Value);
                var attributeType = productAttributeMappings.Where(p => p.Id == productAttributeId).Select(p => p.ProductAttributeId).FirstOrDefault();
                if (attributeType == 2)
                {
                    // ProductAttribute is Color
                    nopColorId = Int32.Parse(productAttributeNode[0].ChildNodes[0].InnerText);

                    // So next one must be Size
                    nopsizeId = Int32.Parse(productAttributeNode[1].ChildNodes[0].InnerText);
                }
                else if (attributeType == 1)
                {
                    // ProductAttribute is Size
                    nopsizeId = Int32.Parse(productAttributeNode[0].ChildNodes[0].InnerText);

                    // So next one must be color
                    nopColorId = Int32.Parse(productAttributeNode[1].ChildNodes[0].InnerText);
                }

                // Save combination
                var combination = new ProductAttributeCombination
                {
                    ProductId = product.Id,
                    AttributesXml = attributesXml,                    
                    AllowOutOfStockOrders = false,
                    Sku = product.Sku,
                    ManufacturerPartNumber = product.ManufacturerPartNumber,
                    Gtin = variantData.EAN,
                    OverriddenPrice = null,
                    NotifyAdminForQuantityBelow = 1
                };
                await _productAttributeService.InsertProductAttributeCombinationAsync(combination);
                _createdCount++;
                // Now this one combination has been created
                // IMPORTANT to step out here
                break;

            }
        }

        #region Private methods
        private AuthenticateAdminRequest CurrentAuthenticateAdminRequest
        {
            get
            {
                AuthenticateAdminRequest request = new AuthenticateAdminRequest()
                {
                    Email = _configuration["ApiSettings:AuthenticateAdminRequest:Email"],
                    Username = _configuration["ApiSettings:AuthenticateAdminRequest:Email"],
                    Password = _configuration["ApiSettings:AuthenticateAdminRequest:Password"]
                };

                return request;
            }
        }

        private async Task SetClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var token = await _client.GetTokenAsync(CurrentAuthenticateAdminRequest);

            SwaggerClientWrapper.Instance.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);
        }

        private async Task AddUrlRecord(string newName, string entityName, int entityId, int languageId)
        {
            string seName = GetNewSeName(newName);

            UrlRecordDto urlRecordDto = new UrlRecordDto()
            {
                Entity_id = entityId,
                Entity_name = entityName,
                Language_id = languageId,
                Is_active = true,
                Slug = seName
            };

            await _client.Create95Async(urlRecordDto);
        }

        private string GetNewSeName(string newName)
        {
            newName = newName.Replace("ø", "oe")
                             .Replace("Ø", "Oe")
                             .Replace("æ", "ae")
                             .Replace("Æ", "Ae")
                             .Replace("å", "aa")
                             .Replace("Å", "Aa")
                             .Replace("ö", "oe")
                             .Replace("Ö", "Oe")
                             .Replace("é", "e")
                             .Replace("É", "E");

            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            var name = newName.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in name.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                {
                    sb.Append(c2);
                }
            }

            var name2 = sb.ToString();
            name2 = name2.Replace(" ", "-");
            while (name2.Contains("--"))
                name2 = name2.Replace("--", "-");
            while (name2.Contains("__"))
                name2 = name2.Replace("__", "_");

            return name2;
        }

        private bool IsStatusValidForSyncJob(int productId)
        {
            var productExtension = _productExtensionData.Where(p => p.ProductId == productId).FirstOrDefault();
            if (productExtension != null)
            {
                if ((ProductStatus)productExtension.StatusId != ProductStatus.ControlledBySync
                    && (ProductStatus)productExtension.StatusId != ProductStatus.OnHold)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<Product> GetOrCreateProduct(VariantData variantData, string orgItemNumber)
        {
            try
            {
                var product = await _productService.GetProductBySkuAsync(orgItemNumber);

                if (product == null)
                {
                    product = new Product()
                    {
                        Name = variantData.Title,
                        Price = variantData.RetailPrice,
                        AdminComment = $"Product created through sync (STM) with use of variant data from '{variantData.Brand}'.{Environment.NewLine}{DateTime.Now:dd-MM-yyyy tt:mm:ss}",
                        ShortDescription = variantData.OriginalTitle,
                        Sku = orgItemNumber,
                        ManufacturerPartNumber = variantData.OrgItemNumber,
                        Published = false,
                        MarkAsNew = true,
                        VisibleIndividually = true,
                        ProductTypeId = 5, // 5==Simple product type
                        ProductTemplateId = 1,
                        BasepriceUnitId = 3,
                        BasepriceBaseUnitId = 3,
                        CreatedOnUtc = DateTime.UtcNow,
                        ManageInventoryMethodId = 2, // 2==Track inventory by product attributes
                        OrderMaximumQuantity = 1000,
                        AllowCustomerReviews = true,
                        DisplayStockAvailability = true,
                        DisplayStockQuantity = true,
                        IsShipEnabled = true,
                        TaxCategoryId = 6, // 6==Dansk moms'
                        DeliveryDateId = 2, // 2 = 2-4 days
                        VendorId = 2 // 2 = STM                        
                    };
                    await _productService.InsertProductAsync(product);

                    await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, string.Empty, variantData.Title, false), 0);
                    await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, string.Empty, variantData.Title, false), 2);

                    product.UpdatedOnUtc = DateTime.UtcNow;
                    await _productService.UpdateProductAsync(product);

                    await _productExtensionDataRepository.InsertAsync(new AOProductExtensionData()
                    {
                        ForceOutOfGoogleShopping = false,
                        ForceOutOfPriceRunner = false,
                        ProductId = product.Id,
                        StatusId = (int)ProductStatus.Crawled,
                        InventoryCountLastDone = Convert.ToDateTime("01-01-1970")
                    });

                    _productCreated++;
                }
                return product;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return null;
        }

        private static string GetOrgItemNumber(string orgItemNumber)
        {
            if (orgItemNumber.Contains('-'))
            {
                // STM uses first part of itemnumber as actual itemnumber, the rest is color and size
                orgItemNumber = orgItemNumber.Substring(0, orgItemNumber.IndexOf('-'));
            }

            orgItemNumber = "STM-" + orgItemNumber;
            return orgItemNumber;
        }

        private async Task<ProductAttributeMapping> GetOrCreateProductColorMapping(Product product, IList<ProductAttributeMapping> productAttributeMappings)
        {
            var productAttributeMappingColor = productAttributeMappings.Where(p => p.ProductAttributeId == 2).FirstOrDefault();
            if (productAttributeMappingColor == null)
            {
                productAttributeMappingColor = await CreateProductAttributeMappingColor(product.Id);
            }

            return productAttributeMappingColor;
        }

        private async Task<ProductAttributeMapping> GetOrCreateProductSizeMapping(Product product, IList<ProductAttributeMapping> productAttributeMappings)
        {
            var productAttributeMappingSize = productAttributeMappings.Where(p => p.ProductAttributeId == 1).FirstOrDefault();
            if (productAttributeMappingSize == null)
            {
                productAttributeMappingSize = await CreateProductAttributeMappingSize(product.Id);
            }

            return productAttributeMappingSize;
        }

        private async Task<ProductAttributeValue> GetOrCreateColorValue(VariantData variantData, Product product, ProductAttributeMapping productAttributeMappingColor)
        {
            var colorValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingColor.Id);
            ProductAttributeValue colorValue = colorValues.Where(c => c.Name.ToLower().Trim() == variantData.ColorStr.ToLower().Trim()).FirstOrDefault();
            if (colorValue == null && string.IsNullOrEmpty(variantData.ColorStr) == false)
            {
                // No color found in Nop and we have string, lets create it
                colorValue = await AddColorValueAsync(product.Id, productAttributeMappingColor.Id, variantData);
            }

            return colorValue;
        }

        private async Task<ProductAttributeMapping> CreateProductAttributeMappingColor(int productId)
        {
            ProductAttributeMapping productAttributeMappingColor = new()
            {
                AttributeControlTypeId = 1,
                IsRequired = true,
                ProductAttributeId = 2, // Color
                ProductId = productId
            };

            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMappingColor);
            return productAttributeMappingColor;
        }

        private async Task<ProductAttributeMapping> CreateProductAttributeMappingSize(int productId)
        {
            ProductAttributeMapping productAttributeMappingColor = new()
            {
                AttributeControlTypeId = 1,
                IsRequired = true,
                ProductAttributeId = 1, // Size
                ProductId = productId
            };

            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMappingColor);
            return productAttributeMappingColor;
        }

        private async Task<ProductAttributeValue> GetOrCreateSizeValue(VariantData variantData, Product product, ProductAttributeMapping productAttributeMappingSize)
        {
            var sizeValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingSize.Id);
            ProductAttributeValue sizeValue = sizeValues.Where(s => s.Name.ToLower().Trim() == variantData.SizeStr.ToLower().Trim()).FirstOrDefault();
            if (sizeValue == null && string.IsNullOrEmpty(variantData.SizeStr) == false)
            {
                // No size found in Nop and we have string, lets create it
                sizeValue = await AddSizeValueAsync(product.Id, productAttributeMappingSize.Id, variantData);
            }

            return sizeValue;
        }

        private async Task GenerateNewCombinationAsync(ProductDto product, VariantData data, string updaterName)
        {
            ICollection<string> allAttributesXml = new List<string>();
            try
            {
                allAttributesXml = await _client.GenerateAllCombinationsAsync(product.Id, true, "");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("timeout"))
                {
                    // Log and do nothing, move on
                    await _logger.ErrorAsync($"SyncJob Exception ({updaterName}): {ex.Message}{Environment.NewLine}On ProductId: {product.Id}{Environment.NewLine}Timeout exception trying to GenerateAllCombinations, maybe clean up colors and sizes on this product");
                }
                else
                {
                    throw;
                }
            }

            foreach (var attributesXml in allAttributesXml)
            {
                var doc = new XmlDocument();
                doc.LoadXml(attributesXml);
                var productAttributeNode = doc.GetElementsByTagName("ProductAttribute");
                if (productAttributeNode.Count != 2)
                {
                    continue;
                }

                var productMappings = await _client.GetAllByProductIdAsync(product.Id);

                int nopColorId = 0, nopsizeId = 0;
                int productAttributeId = Int32.Parse(productAttributeNode[0].Attributes["ID"].Value);
                var attributeType = productMappings.Where(p => p.Id == productAttributeId).Select(p => p.Product_attribute_id).FirstOrDefault();
                if (attributeType == 2)
                {
                    // ProductAttribute is Color
                    nopColorId = Int32.Parse(productAttributeNode[0].ChildNodes[0].InnerText);

                    // So next one must be Size
                    nopsizeId = Int32.Parse(productAttributeNode[1].ChildNodes[0].InnerText);
                }
                else if (attributeType == 1)
                {
                    // ProductAttribute is Size
                    nopsizeId = Int32.Parse(productAttributeNode[0].ChildNodes[0].InnerText);

                    // So next one must be color
                    nopColorId = Int32.Parse(productAttributeNode[1].ChildNodes[0].InnerText);
                }

                // Save combination
                var combination = new ProductAttributeCombinationDto
                {
                    Product_id = product.Id,
                    Attributes_xml = attributesXml,
                    Stock_quantity = data.StockCount,
                    Allow_out_of_stock_orders = false,
                    Sku = product.Sku,
                    Manufacturer_part_number = product.Manufacturer_part_number,
                    Gtin = data.EAN,
                    Overridden_price = null,
                    Notify_admin_for_quantity_below = 1
                };

                var combinationResult = await _client.Create59Async(combination);
                if (combinationResult != null)
                {
                    // Now this one combination has been created
                    // IMPORTANT to step out here
                    break;
                }
            }

            allAttributesXml = null;
        }

        private async Task<ProductAttributeValue> AddColorValueAsync(int productId, int productAttributeMappingSizeId, VariantData variantData)
        {
            var productAttributeValue = new ProductAttributeValue()
            {
                AssociatedProductId = productId,
                Name = Utilities.FirstUpperCase(variantData.ColorStr),
                Quantity = variantData.StockCount,
                AttributeValueTypeId = 2, // 2==Color                        
                ProductAttributeMappingId = productAttributeMappingSizeId
            };

            await _productAttributeService.InsertProductAttributeValueAsync(productAttributeValue);
            return productAttributeValue;
        }

        private async Task<ProductAttributeValue> AddSizeValueAsync(int productId, int productAttributeMappingSizeId, VariantData variantData)
        {
            var productAttributeValue = new ProductAttributeValue()
            {
                AssociatedProductId = productId,
                Name = Utilities.FirstUpperCase(variantData.SizeStr),
                Quantity = variantData.StockCount,
                AttributeValueTypeId = 1, // 1==Size                        
                ProductAttributeMappingId = productAttributeMappingSizeId
            };

            await _productAttributeService.InsertProductAttributeValueAsync(productAttributeValue);
            return productAttributeValue;
        }

        #endregion
    }
}