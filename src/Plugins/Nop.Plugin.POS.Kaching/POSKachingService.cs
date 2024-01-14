using LinqToDB.Common;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.POS.Kaching.Extensions;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nop.Plugin.POS.Kaching
{
    public class POSKachingService : IPOSKachingService
    {
        #region Private fields
        private readonly ILogger _logger;
        private POSKachingSettings _kachingSettings;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IWebHelper _webHelper;
        private IList<Category> _categories = null;
        private int _danishLanguageId = 2;

        /// Result for test: https://us-central1-ka-ching-base.cloudfunctions.net/imports/products?account=friliv_test&apikey=0690a383-68e0-4716-b27d-726987b6d31f&integration=friliv
        private static string _apiUrl = "https://[Host]/imports/[ItemType]?account=[Account]&apikey=[APIKey]&integration=[ImportQueueName]";
        private static string _pingUrl = "https://[Host]/info";
        private static readonly HttpClient _client = new(); 
        #endregion

        public POSKachingService(ILogger logger, IWebHelper webHelper, POSKachingSettings kachingSettings, IManufacturerService manufacturerService, IPictureService pictureService, ILocalizedEntityService localizedEntityService, IProductAttributeService productAttributeService, ICategoryService categoryService)
        {
            _logger = logger;
            _kachingSettings = kachingSettings;
            _pictureService = pictureService;            
            _productAttributeService = productAttributeService;
            _categoryService = categoryService;
            _localizedEntityService = localizedEntityService;
            _manufacturerService = manufacturerService;
            _webHelper = webHelper;

            _apiUrl = _apiUrl.Replace("[Host]", _kachingSettings.POSKaChingHost)
                             .Replace("[Account]", _kachingSettings.POSKaChingAccountToken)
                             .Replace("[APIKey]", _kachingSettings.POSKaChingAPIToken)
                             .Replace("[ImportQueueName]", _kachingSettings.POSKaChingImportQueueName);

            _pingUrl = _pingUrl.Replace("[Host]", _kachingSettings.POSKaChingHost);
        }

        public async Task SaveProductAsync(string json)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = _apiUrl.Replace("[ItemType]", "products");
                
                var result = await _client.PostAsync(endpoint, content);
                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                throw;
            }
        }

        public async Task SaveProductCategoryAsync(string json)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = _apiUrl.Replace("[ItemType]", "product_groups");

                var result = await _client.PostAsync(endpoint, content);
                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                throw;
            }
        }

        public async Task DeleteProductAsync(string[] ids)
        {
            try
            {
                var strIds = String.Join(",", ids);
                var url = $"{_apiUrl.Replace("[ItemType]", "products")}&ids={strIds}";
                var response = await _client.DeleteAsync(url);  
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                throw;
            }
        }

        public async Task<bool> TestConnection()
        {
            var result = await _client.GetAsync(_pingUrl);
            return result.IsSuccessStatusCode;
        }

        public async Task<string> BuildJSONStringAsync(Core.Domain.Catalog.Product product)
        {
            var danishName = await _localizedEntityService.GetLocalizedValueAsync(_danishLanguageId, product.Id, KaChingUtilities.LocaleKeyGroup.Product.ToString(), KaChingUtilities.LocaleKey.Name.ToString());
            var danishShortDescription = await _localizedEntityService.GetLocalizedValueAsync(_danishLanguageId, product.Id, KaChingUtilities.LocaleKeyGroup.Product.ToString(), KaChingUtilities.LocaleKey.ShortDescription.ToString());
            var danishDescription = await _localizedEntityService.GetLocalizedValueAsync(_danishLanguageId, product.Id, KaChingUtilities.LocaleKeyGroup.Product.ToString(), KaChingUtilities.LocaleKey.FullDescription.ToString());

            danishDescription = string.IsNullOrEmpty(danishDescription) ? product.FullDescription.StripHTML() : danishDescription.StripHTML();
            danishDescription = $"Webpris: Kr. {product.Price:N2}{Environment.NewLine}{Environment.NewLine}{danishDescription}";

            var kaChingProduct = new KachingProductModel
            {
                Product = new Models.Product
                {
                    Id = product.Id.ToString(),
                    Name = new Models.Description
                    {
                        Da = string.IsNullOrEmpty(danishName) ? product.Name : danishName,
                        En = product.Name
                    },
                    Description = new Models.Description
                    {
                        Da = danishDescription,
                        En = product.FullDescription.StripHTML()
                    },
                    ShortDescription = new Models.Description
                    {
                        Da = string.IsNullOrEmpty(danishShortDescription) ? product.ShortDescription.StripHTML() : danishShortDescription.StripHTML(),
                        En = product.ShortDescription.StripHTML()
                    },                    
                    RetailPrice = product.OldPrice <= 0 ? (long)product.Price : (long)product.OldPrice,
                    CostPrice = (long)product.ProductCost
                }
            };

            foreach (var pp in await _pictureService.GetPicturesByProductIdAsync(product.Id))
            {
                try
                {
                    var pictureUrl = await _pictureService.GetPictureUrlAsync(pp.Id);
                    pictureUrl = KaChingUtilities.GetValidImageUrl(pictureUrl);

                    if (KaChingUtilities.ImageExists(pictureUrl))
                    {
                        kaChingProduct.Product.ImageUrl = pictureUrl;
                        break;
                    }
                }
                catch (Exception ex) 
                {
                    await _logger.ErrorAsync(ex.Message, ex);
                }
            }

            var defaultImageUrl = kaChingProduct.Product.ImageUrl;
            var variantData = await GetDimensions(product, defaultImageUrl);

            var dimensions = variantData.Dimentions;
            var variants = variantData.Variants;

            if (variants.Count == 1)
            {
                kaChingProduct.Product.Barcode = variants[0].Barcode;
            }
            else if (variants.Count > 0)
            {
                kaChingProduct.Product.Variants = variants.ToArray();
                kaChingProduct.Product.Dimensions = dimensions.ToArray();
            }

            kaChingProduct.Metadata = new Metadata
            {
                Channels = new Channels(),
                Markets = new Markets()
            };
            kaChingProduct.Metadata.Channels.Pos = true;
            kaChingProduct.Metadata.Channels.Online = true;
            kaChingProduct.Metadata.Markets.Dk = true;
            kaChingProduct.Product.Tags = await SetTagsAsync(product);            

            string output = JsonConvert.SerializeObject(kaChingProduct, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });            
            return output;
        }

        public string BuildJSONStringForCategory(string productCategory)
        {
            var productGroupModel = new KachingProductGroupModel
            {
                Group = productCategory.ToLower(),
                Name = new Models.Description
                {
                    Da = productCategory,
                    En = productCategory
                }
            };

            string output = JsonConvert.SerializeObject(productGroupModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
            return output;
        }

        private async Task<Tags> SetTagsAsync(Core.Domain.Catalog.Product product)
        {
            var tags = new Tags();

            if (_categories == null)
            {
                _categories = await _categoryService.GetAllCategoriesAsync();
            }

            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);

            if (productCategories != null && _categories != null && _categories.Count > 0)
            {
                foreach (var cat in productCategories)
                {
                    Category category = GetParentCategory(cat.CategoryId);

                    if (category != null)
                    {
                        switch (category.Name)
                        {
                            case "Mens clothing":
                                tags.Herretoj = true;
                                break;
                            case "Womens clothing":
                                tags.Dametoj = true;
                                break;
                            case "Child wear":
                                tags.Bornetoj = true;
                                break;
                            case "Back packs":
                                tags.Rygsaekke = true;
                                break;
                            case "Sleeping bags":
                                tags.Soveposer = true;
                                break;
                            case "Tents":
                                tags.Telte = true;
                                break;
                            case "Cooking":
                                tags.Kogegrej = true;
                                break;
                            case "Travel accessories":
                                tags.Tilbehor = true;
                                break;
                            case "Footwear":
                                tags.Fodtoj = true;
                                break;
                            default:
                                tags.Diverse = true;
                                break;
                        }
                    }
                }
            }

            return tags;
        }

        private Category GetParentCategory(int categoryId)
        {
            var category = _categories.Where(c => c.Id == categoryId).FirstOrDefault();
            if (category.ParentCategoryId != 0)
                return GetParentCategory(category.ParentCategoryId);

            return category;
        }

        private async Task<(List<Dimension> Dimentions, List<Models.Variant> Variants)> GetDimensions(Core.Domain.Catalog.Product product, string defaultImageUrl)
        {
            List<Dimension> dimensions = new();
            var combinationValues = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
            int[] colorSizeAttributeId = await GetColorAndSizeId();


            var colorDimention = new Dimension()
            {
                Id = "color",
                Name = "Color"                
            };

            var sizeDimention = new Dimension()
            {
                Id = "size",
                Name = "Size"
            };

            List<Value> colorValues = new();
            List<Value> sizeValues = new();
            List<Models.Variant> variants = new();
            Value colorValue = null, sizeValue = null;

            string imageUrl = "";

            // Run through all combinations
            foreach (var combinationValue in combinationValues)
            {
                XmlDocument attributesXml = new XmlDocument();
                attributesXml.LoadXml(combinationValue.AttributesXml);

                // Run through both size and color
                foreach (XmlNode node in attributesXml.DocumentElement)
                {                   
                    var attributeId = Convert.ToInt32(node.Attributes["ID"].Value);
                    var attributeValueId = Convert.ToInt32(node.FirstChild.FirstChild.InnerText);

                    var attributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(attributeValueId);
                    var mapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(attributeId);

                    if (attributeValue == null || mapping == null)
                    {
                        continue;
                    }                    
               
                    if (colorValues.Any(c => c.Id == attributeValue.Id.ToString())
                         && mapping.ProductAttributeId == colorSizeAttributeId[0])
                    {
                        // Color is already added to dimension
                        colorValue = colorValues.Where(c => c.Id == attributeValue.Id.ToString()).FirstOrDefault();
                        continue;
                    }

                    if (sizeValues.Any(s => s.Id == attributeValue.Id.ToString())
                        && mapping.ProductAttributeId == colorSizeAttributeId[1])
                    {
                        // Size is already added to dimension
                        sizeValue = sizeValues.Where(s => s.Id == attributeValue.Id.ToString()).FirstOrDefault();
                        continue;
                    }
                   

                    if (mapping.ProductAttributeId == colorSizeAttributeId[0])
                    {
                        // Color Dimension
                        if (attributeValue.PictureId > 0)
                        {
                            imageUrl = await _pictureService.GetPictureUrlAsync(attributeValue.PictureId.Value);
                            imageUrl = KaChingUtilities.GetValidImageUrl(imageUrl);
                        }

                        if (string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = defaultImageUrl;
                        }

                        colorValue = new Value
                        {
                            Id = attributeValue.Id.ToString(),
                            ImageUrl = imageUrl,
                            Name = attributeValue.Name,
                            ColorCode = attributeValue.ColorSquaresRgb
                        };

                        colorValues.Add(colorValue);
                    }
                    else if (mapping.ProductAttributeId == colorSizeAttributeId[1])
                    {
                        // Size dimension
                        sizeValue = new Value()
                        {
                            Id = attributeValue.Id.ToString(),
                            Name = attributeValue.Name
                        };
                        sizeValues.Add(sizeValue);                       
                    }
                }

                Models.Variant variant = new Variant
                {
                    Barcode = combinationValue.Gtin,
                    Id = combinationValue.Id.ToString(),
                    ImageUrl = imageUrl,
                    DimensionValues = new DimensionValues()
                };

                variant.DimensionValues.Color = colorValue != null ? colorValue.Id : "0";
                variant.DimensionValues.Size = sizeValue != null ? sizeValue.Id : "0";

                variants.Add(variant);
            }
           
            colorDimention.Values = colorValues.ToArray();
            sizeDimention.Values = sizeValues.ToArray();

            dimensions.Add(colorDimention);
            dimensions.Add(sizeDimention);

            return (dimensions, variants);            
        }

        private async Task<int[]> GetColorAndSizeId()
        {
            int[] colorSizeAttributeId = new int[2];

            foreach (var att in await _productAttributeService.GetAllProductAttributesAsync())
            {
                if (att.Name.ToUpper() == "COLOR")
                {
                    colorSizeAttributeId[0] = att.Id;
                }
                else if (att.Name.ToUpper() == "SIZE")
                {
                    colorSizeAttributeId[1] = att.Id;
                }
            }

            return colorSizeAttributeId;
        }
    }
}