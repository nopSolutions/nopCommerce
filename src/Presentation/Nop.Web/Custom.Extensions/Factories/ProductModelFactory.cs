using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services;
using Nop.Services.Logging;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Web.Models.PrivateMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial interface IProductModelFactory
    {
        Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IList<ProductCustom> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        Task<IEnumerable<int>> FilterRelatedProductsAsync(IList<int> productIds);

        Task<bool> CanCurrentCustomerViewTargetProfileAsync(Product product);
    }

    public partial class ProductModelFactory
    {
        private ICustomerActivityService _customerActivityService;

        #region Methods

        public virtual async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IList<ProductCustom> products,
            bool preparePriceModel = false, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    ShortDescription = product.ShortDescription,
                    FullDescription = product.FullDescription,
                    //SeName = await _urlRecordService.GetSeNameAsync(new Product { Id = product.Id }),
                    SeName = product.Slug,
                    ProductType = product.ProductType,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow),

                    //ProfileType = GetCustomerProfileType(product.Id),
                    Name = product.FirstName,
                    MobileNumber = product.Phone,
                    //Location = $"{product.Country} , {product.StateProvince}, {product.City}",
                    ProfileType = product.ProfileType,
                    LastLoginDateTime = await GetCustomerLastLoginDate(product),

                    PrimaryTechnology = product.PrimaryTechnology,
                    SecondaryTechnology = product.SecondaryTechnology,
                    CurrentAvalibility = product.CurrentAvalibility,

                    ProfileShortListed = product.ProfileShortListed,
                    InterestSent = product.InterestSent,
                    MotherTounge = product.MotherTongue ?? "English",

                    PremiumCustomer = product.PremiumCustomer,
                    Gender = product.Gender,
                    WorkExperience = product.WorkExperience,
                    CustomerProfileTypeId = product.CustomerProfileTypeId

                };

                var location = string.Empty;
                if (!string.IsNullOrEmpty(product.City))
                    location = $"{product.City}, ";
                if (!string.IsNullOrEmpty(product.StateProvince))
                    location += $"{product.StateProvince}, ";
                if (!string.IsNullOrEmpty(product.Country))
                    location += $"{product.Country}";

                model.Location = location;

                //picture
                if (preparePictureModel)
                {
                    var productOriginal = await _productService.GetProductByIdAsync(product.Id);
                    model.PictureModels = await PrepareProductOverviewPicturesModelAsync(productOriginal, productThumbPictureSize);
                    //model.DefaultPictureModel = await PrepareProductOverviewPictureModel(product, productThumbPictureSize);
                }

                //reviews
                //model.ReviewOverviewModel = await PrepareProductReviewOverviewModel(product);

                models.Add(model);
            }
            return models;
        }

        protected virtual async Task<PictureModel> PrepareProductOverviewPictureModel(ProductCustom product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDetailsPicturesModelKey,
                product, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(),
                await _storeContext.GetCurrentStoreAsync());

            var defaultPictureModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                Picture picture = null;

                if (product.AvatarPictureId > 0)
                    picture = await _pictureService.GetPictureByIdAsync(product.AvatarPictureId);

                string fullSizeImageUrl, imageUrl;
                (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);
                (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);

                var pictureModel = new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

        public string GetPrimaryTechnology(ProductSpecificationModel specModel)
        {
            foreach (var group in specModel.Groups)
            {
                var ptList = group.Attributes
                                 .Where(x => x.Name == ProductAndCustomerAttributeEnum.PrimaryTechnology.ToString())
                                 .Select(o => o.Values.Select(a => a.ValueRaw))
                                 .FirstOrDefault();

                return ptList != null ? string.Join<string>(" ,", ptList) : string.Empty;
            }

            return string.Empty;
        }

        public string GetSpecificationAttributeValues(ProductSpecificationModel specModel, ProductAndCustomerAttributeEnum psEnum)
        {
            foreach (var group in specModel.Groups)
            {
                var ptList = group.Attributes
                                 .Where(x => x.Name.Replace(" ", "") == psEnum.ToString())
                                 .Select(o => o.Values.Select(a => a.ValueRaw))
                                 .FirstOrDefault();

                return ptList != null ? string.Join<string>(" , ", ptList) : string.Empty;
            }

            return string.Empty;
        }

        public async Task<string> GetCustomerLastLoginDate(Customer customer)
        {
            if (customer == null)
                return string.Empty;

            return await _forumModelFactory.ConvertDateTimeToHumanString(customer.LastActivityDateUtc);
        }

        public async Task<string> GetCustomerLastLoginDate(ProductCustom product)
        {
            if (product == null)
                return string.Empty;

            return await _forumModelFactory.ConvertDateTimeToHumanString(product.LastActivityDateUtc);
        }

        public async Task<string> GetCustomerProfileType(int productId)
        {
            //Current Customer(product) profile type.
            var category = (await _categoryService.GetProductCategoriesByProductIdAsync(productId)).FirstOrDefault();

            //if (category != null && category.Category.Name.Replace(" ", "") == CustomerProfileTypeEnum.TakeSupport.ToString())
            //    return CustomerProfileTypeEnum.TakeSupport.ToString();

            var customer = (await _customerService.GetAllCustomersAsync(vendorId: productId)).FirstOrDefault();

            if (customer != null)
            {
                var customAttributes = customer.CustomCustomerAttributesXML;
            }

            return CustomerProfileTypeEnum.GiveSupport.ToString();
        }

        public async Task<string> GetCustomerMobileNumber(Customer customer)
        {
            if (customer != null)
            {
                var mobileNumber = customer.Phone;
                return mobileNumber;
            }
            return string.Empty;
        }

        public async Task<string> GetCustomerFirstName(Customer customer)
        {
            if (customer != null)
            {
                var firstName = customer.FirstName;
                return firstName;
            }
            return string.Empty;
        }

        public async Task<string> GetCustomerLocation(Customer customer)
        {
            if (customer != null)
            {
                var countryId = customer.CountryId;

                var country = await _countryService.GetCountryByIdAsync(countryId);
                //var state = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Name;
                var city = customer.City;

                var location = string.Empty;

                if (!string.IsNullOrEmpty(city))
                    location = $"{city}, ";
                //if (!string.IsNullOrEmpty(state))
                //    location += $"{state}, ";
                if (!string.IsNullOrEmpty(country?.Name))
                    location += $"{country.Name}";

                return location;
            }
            return string.Empty;
        }

        public async Task<bool> GetIsProfileShortListed(int productId)
        {
            var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.Wishlist, (await _storeContext.GetCurrentStoreAsync()).Id, productId);
            return shoppingCarts.Any();
        }

        public async Task<bool> GetIsInterestSent(int productId)
        {
            var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.InterestSent, (await _storeContext.GetCurrentStoreAsync()).Id, productId);
            return shoppingCarts.Any();
        }

        public async Task<string> GetProfileType(Customer customer)
        {
            var profileTypeId = (int)ProductAndCustomerAttributeEnum.ProfileType;

            if (customer != null)
            {
                var customerAttributesXml = customer.CustomCustomerAttributesXML;
                var profileType = _customerAttributeParser.ParseValues(customerAttributesXml, profileTypeId).FirstOrDefault();

                if (profileType != string.Empty)
                {
                    var userProfileTypeId = Convert.ToInt32(profileType);
                    var profileTypeCategory = await _customerAttributeService.GetAttributeValueByIdAsync(userProfileTypeId);
                    return profileTypeCategory != null ? profileTypeCategory.Name : string.Empty;
                }
            }
            return string.Empty;
        }

        public async Task<string> GetCustomerGender(Customer customer)
        {
            if (customer != null)
            {
                var gender = customer.Gender;
                return gender;
            }
            return string.Empty;
        }

        protected async Task<PrivateMessageModel> PreparePrivateMessageViewModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var customer = await _workContext.GetCurrentCustomerAsync();
            var pm = (await _forumService.GetAllPrivateMessagesAsync(customer.RegisteredInStoreId, product.VendorId, customer.Id,
                                                                     isRead: null, null, null, string.Empty))
                                                                     .ToList().FirstOrDefault();

            if (pm != null)
            {
                var pmModel = await _privateMessagesModelFactory.PreparePrivateMessageModelAsync(pm);
                return pmModel;
            }

            var model = new PrivateMessageModel { };
            return model;
        }

        protected virtual async Task<PictureModel> PrepareProductOverviewPictureModel(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDetailsPicturesModelKey,
                product, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(),
                await _storeContext.GetCurrentStoreAsync());

            var defaultPictureModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                Picture picture = null;

                int avatarPictureAttributeId = 0;
                var customer = (await _customerService.GetAllCustomersAsync(vendorId: product.Id)).FirstOrDefault();
                if (customer != null)
                    avatarPictureAttributeId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);

                if (avatarPictureAttributeId > 0)
                    picture = await _pictureService.GetPictureByIdAsync(avatarPictureAttributeId);

                string fullSizeImageUrl, imageUrl;
                (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);
                (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);

                var pictureModel = new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl,
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

        public virtual async Task CustomizeProductDetailModel(ProductDetailsModel model, Product product)
        {
            //'Private message' model
            model.PrivateMessageModel = await PreparePrivateMessageViewModelAsync(product);

            //picture model
            var productThumbPictureSize = _mediaSettings.ProductThumbPictureSize;
            model.DefaultPictureModel = await PrepareProductOverviewPictureModel(product, productThumbPictureSize);

            model.PrimaryTechnology = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.PrimaryTechnology);
            model.SecondaryTechnology = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.SecondaryTechnology);
            model.CurrentAvalibility = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.CurrentAvalibility);
            model.ProfileType = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.ProfileType);
            model.RelaventExperiance = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.RelaventExperiance);
            model.MotherTongue = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.MotherTongue);
            model.Gender = GetSpecificationAttributeValues(model.ProductSpecificationModel, ProductAndCustomerAttributeEnum.Gender);

            //get target customer
            var customer = (await _customerService.GetAllCustomersAsync(vendorId: model.Id)).FirstOrDefault();

            //Note: Pricing products throw error as they dont have any customer associated
            if (customer != null)
            {
                var customerName = await GetCustomerFirstName(customer);

                model.Name = customerName == string.Empty ? await _urlRecordService.GetSeNameAsync(product) : customerName;
                model.MobileNumber = await GetCustomerMobileNumber(customer);
                model.Location = await GetCustomerLocation(customer);
                model.LastLoginDateTime = await GetCustomerLastLoginDate(customer);

                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

                var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(currentCustomer, ShoppingCartType.Wishlist, storeId, product.Id);
                model.AddToCart.ProfileShortListed = shoppingCarts.Any();
                model.AddToCart.ProfileShortListedOn = shoppingCarts.Any() ? shoppingCarts.First().CreatedOnUtc.ToString() : string.Empty;

                var interests = await _shoppingCartService.GetShoppingCartAsync(currentCustomer, ShoppingCartType.InterestSent, storeId, product.Id);
                model.AddToCart.InterestSent = interests.Any();

                model.EmailId = customer.Email;
                model.Gender = await GetCustomerGender(customer);
            }
        }

        public virtual async Task CustomizeProductModel(ProductOverviewModel model, Product product)
        {
            if (product.VendorId > 0)
            {
                var customer = await _customerService.GetCustomerByIdAsync(product.VendorId);

                if (customer == null)
                    return;

                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
                model.Phone = customer.Phone;

                model.Gender = customer.Gender;
                model.Company = customer.Company;
                model.CountryId = customer.CountryId.ToString();
                //model.Country = customer.Country;

                model.StateProvinceId = customer.StateProvinceId.ToString();
                model.LanguageId = customer.LanguageId.ToString();
                model.TimeZoneId = customer.TimeZoneId;
                model.AvatarPictureId = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);

                model.CustomerProfileTypeId = customer.CustomerProfileTypeId;
            }

        }

        public virtual async Task CustomizeProductReviewModel(ProductReviewsModel model, Product product)
        {
            if (product.VendorId > 0)
            {
                var targetCustomer = await _customerService.GetCustomerByIdAsync(product.VendorId);
                //display customer first name instead of fullname.Proudct name is always full name.
                //model.ProductName = await _customerService.FormatUsernameAsync(targetCustomer);
            }

        }

        public virtual async Task<(string fullSizeImageUrl, string imageUrl)> CustomizeProductPictureAsync(Product product)
        {
            int pictureAttributeId = 0;

            // get customer by product id
            var customer = (await _customerService.GetAllCustomersAsync(vendorId: product.Id)).FirstOrDefault();
            if (customer != null)
                pictureAttributeId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);

            Picture picture = null;
            if (pictureAttributeId > 0)
                picture = await _pictureService.GetPictureByIdAsync(pictureAttributeId);


            var fullSizeImageUrl = await _pictureService.GetDefaultPictureUrlAsync(0, PictureType.Avatar, null);

            return (fullSizeImageUrl, fullSizeImageUrl);

        }

        #endregion

        #region Related products

        public virtual async Task<IEnumerable<int>> FilterRelatedProductsAsync(IList<int> productIds)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (_customerActivityService == null)
                _customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();

            //get current customer viewed product list in the last 15/30 days
            var createdOnFrom = DateTime.Now.AddDays(-15);
            var customerViewedProducts = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id, activityLogTypeId: 133, entityName: "Product", createdOnFrom: createdOnFrom);
            var viewedProductIds = customerViewedProducts.Select(x => Convert.ToInt32(x.EntityId)).Distinct();

            var unviewedProductIds = productIds.Except(viewedProductIds);

            if (unviewedProductIds.Count() == 0)
                return productIds;

            if (unviewedProductIds.Count() < 3)
            {
                //insert warning log. We need to make sure at any point there are atleast 10 itmes in related product list
            }

            return unviewedProductIds;
        }

        #endregion

        public virtual async Task<bool> CanCurrentCustomerViewTargetProfileAsync(Product product)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var targetProfile = await _customerService.GetCustomerByIdAsync(product.VendorId);

            //if logged in customer is admin then he can see any profile
            if (await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName))
                return true;

            if (targetProfile == null || (customer.CustomerProfileTypeId == targetProfile.CustomerProfileTypeId))
                return false;

            return true;
        }
    }
}
