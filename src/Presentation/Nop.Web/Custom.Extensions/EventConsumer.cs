using DocumentFormat.OpenXml.EMMA;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Web.Models.Media;


namespace Nop.CustomExtensions.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer : IConsumer<OrderPaidEvent>,
        IConsumer<CustomerRegisteredEvent>,
        IConsumer<EntityInsertedEvent<GenericAttribute>>,
        IConsumer<EntityUpdatedEvent<GenericAttribute>>,
        IConsumer<EntityDeletedEvent<GenericAttribute>>,
        IConsumer<ModelPreparedEvent<BaseNopModel>>
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly Nop.Services.Logging.ILogger _logger;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IAffiliateService _affiliateService;
        private readonly IAddressService _addressService;
        private readonly ILocalizationService _localizationService;

        protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductService _productService;
        private readonly CustomerSettings _customerSettings;
        private readonly IWorkContext _workContext;
        protected readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public EventConsumer(IGenericAttributeService genericAttributeService,
             ICustomerService customerService,
             Nop.Services.Logging.ILogger logger,
             IStoreContext storeContext,
             ShoppingCartSettings shoppingCartSettings,
             IOrderService orderService,
             IAffiliateService affiliateService,
             IAddressService addressService,
             ILocalizationService localizationService,
             ICustomerActivityService customerActivityService,

             IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
             ISpecificationAttributeService specificationAttributeService,
             ICategoryService categoryService,
             IUrlRecordService urlRecordService,
             IProductService productService,
             CustomerSettings customerSettings,
             IWorkContext workContext,
             IWebHelper webHelper
            )
        {
            _genericAttributeService = genericAttributeService;
            _customerService = customerService;
            _logger = logger;
            _customerActivityService = customerActivityService;
            _shoppingCartSettings = shoppingCartSettings;
            _orderService = orderService;
            _storeContext = storeContext;
            _affiliateService = affiliateService;
            _addressService = addressService;
            _localizationService = localizationService;

            _customerAttributeParser = customerAttributeParser;
            _specificationAttributeService = specificationAttributeService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _customerSettings = customerSettings;
            _workContext = workContext;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            await AddCustomerToPaidCustomerRole(eventMessage.Order.CustomerId);
            await AddCustomerSubscriptionInfoToGenericAttributes(eventMessage.Order);
        }

        public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
        {
            var customer = eventMessage.Customer;

            //update customer customerprofiletypeid
            await UpdateCustomerCustomerProfileTypeIdAsync(customer);

            //add customer to givesupport/take support roles
            await AddCustomerToJobSupportRoleAsync(customer);

            //create product immediatly after customer registered
            await CreateProductAsync(customer, customer.CustomCustomerAttributesXML, customer.FirstName, customer.LastName, customer.Gender);

            //create customer as customer affliate so that he can refer his friends.
            await CreateCustomerAffliateAsync(customer);

        }

        public async Task HandleEventAsync(EntityInsertedEvent<Customer> eventMessage)
        {
            await Task.FromResult(0);
        }

        public async Task HandleEventAsync(EntityInsertedEvent<GenericAttribute> eventMessage)
        {
            await CreateOrUpdateProductPictureMappingAsync(eventMessage.Entity);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<GenericAttribute> eventMessage)
        {
            await CreateOrUpdateProductPictureMappingAsync(eventMessage.Entity);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<GenericAttribute> eventMessage)
        {
            await Task.FromResult(0);
        }

        public async Task AddCustomerToPaidCustomerRole(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            var isCustomerInPaidCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.PaidCustomerRoleName, true);

            if (!isCustomerInPaidCustomerRole)
            {
                //add customer to paidcustomer role. CustomerRoleId= 9 - PaidCustomer
                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customerId, CustomerRoleId = 9 });

                //customer activity
                await _customerActivityService.InsertActivityAsync(customer, "PublicStore.CustomerSubscriptionInfo", "Customer Has Been Added To PaidCustomer Role ", customer);

            }
            else
                await _customerActivityService.InsertActivityAsync(customer, "PublicStore.CustomerSubscriptionInfo", "Customer already having PaidCustomer Role.Paid again may be by mistake.", customer);

        }

        public async Task AddCustomerSubscriptionInfoToGenericAttributes(Order order)
        {
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            //get ordered product id
            var activeOrderItems = await _orderService.GetOrderItemsAsync(order.Id);
            var customerSubscribedProductId = activeOrderItems.FirstOrDefault().ProductId;

            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var allottedCount = 00;

            if (customerSubscribedProductId == _shoppingCartSettings.ThreeMonthSubscriptionProductId)
            {
                allottedCount = _shoppingCartSettings.ThreeMonthSubscriptionAllottedCount;
            }
            else if (customerSubscribedProductId == _shoppingCartSettings.SixMonthSubscriptionProductId)
            {
                allottedCount = _shoppingCartSettings.SixMonthSubscriptionAllottedCount;
            }
            else if (customerSubscribedProductId == _shoppingCartSettings.OneYearSubscriptionProductId)
            {
                allottedCount = _shoppingCartSettings.OneYearSubscriptionAllottedCount;
            }

            // get the subscription details from generic attribute table
            var subscriptionId = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SubscriptionId, storeId);
            var subscriptionAllottedCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionAllottedCount, storeId);
            var subscriptionDate = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SubscriptionDate, storeId);

            // carry forward previous credits
            allottedCount += subscriptionAllottedCount;

            var oldSubscriptionInfo = string.Format("Old Subscription Info - Customer Email:{0} ; SubscriptionId: {1} ; Credits: {2} ; SubscriptionDate: {3}",
                                        customer.Email,
                                        subscriptionId,
                                        subscriptionAllottedCount,
                                        subscriptionDate);

            //customer activity : Before updating the new subscription , save the old subscription details
            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.CustomerSubscriptionInfo", oldSubscriptionInfo, customer);

            //save SubscriptionId, credits , subscription date 
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SubscriptionId, customerSubscribedProductId, storeId);
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SubscriptionAllottedCount, allottedCount, storeId);
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SubscriptionDate, order.CreatedOnUtc, storeId);


            var newSubscriptionInfo = string.Format("New Subscription Info - Customer Email:{0} ; SubscriptionId: {1} ; Credits: {2} ; SubscriptionDate: {3}",
                                        customer.Email,
                                        customerSubscribedProductId,
                                        allottedCount,
                                        order.CreatedOnUtc.ToString());

            //customer activity
            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.CustomerSubscriptionInfo", newSubscriptionInfo, customer);

        }

        private async Task CreateProductAsync(Customer customer, string customerAttributesXml, string firstName, string lastName, string gender)
        {
            var customerProfileTypeId = GetCustomerProfileTypeId(customerAttributesXml);

            var shortDescriptionId = (int)ProductAndCustomerAttributeEnum.ShortDescription;
            var fullDescriptionId = (int)ProductAndCustomerAttributeEnum.FullDescription;

            var primaryTechnologyAttributeValues = _customerAttributeParser.ParseValues(customerAttributesXml, (int)ProductAndCustomerAttributeEnum.PrimaryTechnology).ToList();
            var secondaryTechnologyAttributeValues = _customerAttributeParser.ParseValues(customerAttributesXml, (int)ProductAndCustomerAttributeEnum.SecondaryTechnology).ToList();

            var totalAttributeValues = primaryTechnologyAttributeValues.Select(int.Parse).ToList();
            totalAttributeValues.AddRange(secondaryTechnologyAttributeValues.Select(x => int.Parse(x)).Distinct().ToList());

            var attributeValuesFromSpec = (await _specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(totalAttributeValues.ToArray())).Select(x => x.Name).ToList();

            var categories = await _categoryService.GetAllCategoriesAsync(attributeValuesFromSpec);
            var missedCategories = attributeValuesFromSpec.Except(categories.Select(x => x.Name)).ToList();

            var categoryIds = new List<int>();

            categoryIds.Add(customerProfileTypeId); //give support or take support
            categoryIds.AddRange(categories.Select(x => x.Id).ToList()); //primary technologies & secondary technologies selected

            //create technical category if it doesnt exist in the selected primary technology list
            var newlyAddedCategoryIds = await CreateCategoryAsync(missedCategories);
            categoryIds.AddRange(newlyAddedCategoryIds);

            // create product model with customer data
            var productModel = new Nop.Web.Areas.Admin.Models.Catalog.ProductModel()
            {
                Name = firstName + " " + lastName,
                Published = true,
                ShortDescription = _customerAttributeParser.ParseValues(customerAttributesXml, shortDescriptionId).FirstOrDefault(),
                FullDescription = _customerAttributeParser.ParseValues(customerAttributesXml, fullDescriptionId).FirstOrDefault(),
                ShowOnHomepage = false,
                AllowCustomerReviews = true,
                IsShipEnabled = false,
                Price = 500,
                SelectedCategoryIds = categoryIds,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 1000,
                IsTaxExempt = true,
                //below are mandatory feilds otherwise the product will not be visible in front end store
                Sku = $"SKU_{firstName}_{lastName}",
                ProductTemplateId = 1, // simple product template
                ProductTypeId = (int)ProductType.SimpleProduct,
                VisibleIndividually = true,
                //set product vendor id to customer id. AKA VendorId means Customer Id
                VendorId = customer.Id
            };

            var product = productModel.ToEntity<Product>();
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;

            //product creation
            await _productService.InsertProductAsync(product);

            //product categories mappings (map this product to its cateogories)
            await SaveCategoryMappingsAsync(product, productModel);

            //set SEO settings. Otherwise the product wont be visible in front end
            productModel.SeName = await _urlRecordService.ValidateSeNameAsync(product, productModel.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, productModel.SeName, 0);

            //Update customer with Productid as VendorId. Here Vendor Id means Product Id
            customer.VendorId = product.Id;
            await _customerService.UpdateCustomerAsync(customer);

            //create product specification attribute mappings
            await CreateProductSpecificationAttributeMappingsAsync(product, customerAttributesXml, gender);

            //update customer availability in order to send notifications to other similar customers
            //await CreateOrUpdateCustomerCurrentAvailabilityAsync(customer, customerAttributesXml);

        }

        public int GetCustomerProfileTypeId(string customerAttributesXml)
        {
            var profileTypeId = (int)ProductAndCustomerAttributeEnum.ProfileType;
            var profileType = _customerAttributeParser.ParseValues(customerAttributesXml, profileTypeId).FirstOrDefault();

            var customerProfileTypeId = Convert.ToInt32(profileType);
            return customerProfileTypeId;
        }

        private async Task<List<int>> CreateCategoryAsync(List<string> categories)
        {
            var newlyAddedcategoryIds = new List<int>();
            foreach (var category in categories)
            {
                var newCategory = new Category
                {
                    Name = category,
                    CategoryTemplateId = 1,
                    IncludeInTopMenu = false,
                    ShowOnHomepage = false,
                    Published = true,
                    PriceRangeFiltering = false,
                    PageSize = 20 //default page size otherwise cateogory wont appear 
                };
                await _categoryService.InsertCategoryAsync(newCategory);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(newCategory, category, category, true);
                await _urlRecordService.SaveSlugAsync(newCategory, seName, 0);
                newlyAddedcategoryIds.Add(newCategory.Id);
            }

            return newlyAddedcategoryIds;
        }

        protected virtual async Task SaveCategoryMappingsAsync(Product product, ProductModel model)
        {
            var existingProductCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    await _categoryService.DeleteProductCategoryAsync(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (_categoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;

                    await _categoryService.InsertProductCategoryAsync(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task CreateProductSpecificationAttributeMappingsAsync(Product product, string customerAttributesXml, string gender)
        {
            var spectAttributes = await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync();

            foreach (var attribute in spectAttributes)
            {
                //gender specification attribute mapping
                if (attribute.Id == _customerSettings.GenderSpecificationAttributeId)
                {
                    var psaGender = new ProductSpecificationAttribute
                    {
                        AllowFiltering = true,
                        ProductId = product.Id,
                        SpecificationAttributeOptionId = (gender == "M") ? _customerSettings.GenderMaleSpecificationAttributeOptionId : _customerSettings.GenderFeMaleSpecificationAttributeOptionId,
                        ShowOnProductPage = true
                    };
                    await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psaGender);
                }

                //attribute.Id means primary tech,secondary tect etc.
                var attributeOptionIds = _customerAttributeParser.ParseValues(customerAttributesXml, attribute.Id).ToList();

                foreach (var attributeOptionId in attributeOptionIds)
                {
                    //create product to spec attribute mapping
                    var psa = new ProductSpecificationAttribute
                    {
                        //attribute id 1 means profile type. Do not Show Profile Type filter on product filters page
                        AllowFiltering = attribute.Id == 1 ? false : true,
                        ProductId = product.Id,
                        SpecificationAttributeOptionId = Convert.ToInt32(attributeOptionId),
                        ShowOnProductPage = true
                    };
                    await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psa);
                }
            }
        }

        private async Task CreateOrUpdateCustomerCurrentAvailabilityAsync(Customer customer, string newCustomerAttributesXml)
        {
            var oldCustomAttributeXml = customer.CustomCustomerAttributesXML;

            if (!string.IsNullOrEmpty(oldCustomAttributeXml))
            {
                //existing customer
                var OldCustomerAvailability = _customerAttributeParser.ParseValues(oldCustomAttributeXml, (int)ProductAndCustomerAttributeEnum.CurrentAvalibility)
                                                                  .ToList().Select(int.Parse).FirstOrDefault();
                var newCustomerAvailability = _customerAttributeParser.ParseValues(newCustomerAttributesXml, (int)ProductAndCustomerAttributeEnum.CurrentAvalibility)
                                                                      .ToList().Select(int.Parse).FirstOrDefault();

                // SpecificationAttributeOption: 3 - Available ; 4 - UnAvailable
                if (OldCustomerAvailability == 4 && newCustomerAvailability == 3)
                {
                    //customer changed from UnAvailable to Available
                    await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.NotifiedAboutCustomerAvailabilityAttribute, false, (await _storeContext.GetCurrentStoreAsync()).Id);

                    await _customerActivityService.InsertActivityAsync(await _workContext.GetCurrentCustomerAsync(), "PublicStore.EditCustomerAvailabilityToTrue",
                    "Public Store. Customer changed from UnAvailable to Available", await _workContext.GetCurrentCustomerAsync());
                }
            }
            else
            {
                //new customer
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.NotifiedAboutCustomerAvailabilityAttribute, false, (await _storeContext.GetCurrentStoreAsync()).Id);

                await _customerActivityService.InsertActivityAsync(await _workContext.GetCurrentCustomerAsync(), "PublicStore.EditCustomerAvailabilityToTrue",
                        "Public Store. New Customer Registered", await _workContext.GetCurrentCustomerAsync());
            }

        }

        public async Task CreateOrUpdateProductPictureMappingAsync(GenericAttribute entity)
        {
            //check if customer deleted the profile picture
            if (entity.Key == NopCustomerDefaults.AvatarPictureIdAttribute && entity.KeyGroup == "Customer" && entity.Value == "0")
            {
                var customerId = entity.EntityId;
                var customer = await _customerService.GetCustomerByIdAsync(customerId);

                var pictures = await _productService.GetProductPicturesByProductIdAsync(customer.VendorId);

                //delete existing product to picture mappings
                foreach (var picture in pictures)
                    await _productService.DeleteProductPictureAsync(picture);
            }

            //check if customer updated/created the profile picture
            if (entity.Key == NopCustomerDefaults.AvatarPictureIdAttribute && entity.KeyGroup == "Customer" && Convert.ToUInt32(entity.Value) > 0)
            {
                var customerId = entity.EntityId;
                var pictureId = Convert.ToInt32(entity.Value);
                var customer = await _customerService.GetCustomerByIdAsync(customerId);

                var pictures = await _productService.GetProductPicturesByProductIdAsync(customer.VendorId);

                //delete existing product to picture mappings
                foreach (var picture in pictures)
                    await _productService.DeleteProductPictureAsync(picture);

                //create product to picture mappings
                await _productService.InsertProductPictureAsync(new ProductPicture
                {
                    ProductId = customer.VendorId,
                    PictureId = pictureId,
                    DisplayOrder = 1
                });
            }
        }

        public async Task AddCustomerToJobSupportRoleAsync(Customer customer)
        {
            if (customer.CustomerProfileTypeId == (int)CustomerProfileTypeEnum.GiveSupport)
            {
                var giveSupportRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GiveSupportRoleName);
                if (giveSupportRole == null)
                    throw new NopException("'Give Support' role could not be loaded");

                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = giveSupportRole.Id });
            }
            else if (customer.CustomerProfileTypeId == (int)CustomerProfileTypeEnum.TakeSupport)
            {
                var takeSupportRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.TakeSupportRoleName);
                if (takeSupportRole == null)
                    throw new NopException("'Take Support' role could not be loaded");

                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = takeSupportRole.Id });
            }
        }

        public async Task CreateCustomerAffliateAsync(Customer customer)
        {
            var address = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);

            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var firstName = customer.FirstName;
            var lastName = customer.LastName;

            //affiliate.AddressId = address.Id;

            var affiliate = new Affiliate
            {
                Active = true,
                AdminComment = "Affiliate created for customer",
                FriendlyUrlName = ""
            };

            //validate friendly URL name
            var freindlyName = string.Format("referral-{0}-{1}", firstName, lastName);
            var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(affiliate, freindlyName);

            affiliate.FriendlyUrlName = friendlyUrlName;
            //affiliate.AddressId = address.FirstOrDefault().Id;

            //await _affiliateService.InsertAffiliateAsync(affiliate);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewAffiliate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);
        }

        public async Task UpdateCustomerCustomerProfileTypeIdAsync(Customer customer)
        {
            customer.CustomerProfileTypeId = GetCustomerProfileTypeId(customer.CustomCustomerAttributesXML);
            await _customerService.UpdateCustomerAsync(customer);
        }

        /// <summary>
        /// This method is used to modify the model and its properties via events
        /// -- Adding custom navigation items to My Account page with out modifying customermodelfactory class
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
        {
            if (eventMessage.Model is CustomerNavigationModel model)
            {
                Console.WriteLine("Passed in event");

                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "PrivateMessages",
                    Title = "Mails and Messages ",
                    Tab = (int)CustomerNavigationEnum.PrivateMessages,
                    ItemClass = "customer-PrivateMessages"
                });

                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "ShortListed",
                    Title = "Short Listed",
                    Tab = (int)CustomerNavigationEnum.ShortListed,
                    ItemClass = "customer-shortlisted"
                });

                //remove address item
                model.CustomerNavigationItems.RemoveAt(1);

                //sort by name
            }

            if (eventMessage.Model is ProductDetailsModel productModel)
            {
                //remove last part after space which is surname
                //var strTrimmed = productModel.DefaultPictureModel.Title.Trim();
                //var finalString = strTrimmed.Substring(strTrimmed.LastIndexOf(" ", strTrimmed.Length));

                //productModel.DefaultPictureModel.Title = finalString;
                //productModel.DefaultPictureModel.AlternateText = finalString;

                if (productModel.PictureModels.Count == 0)
                {
                    if (productModel.Gender.ToLower() == "F".ToLower())
                    {
                        //change picture to women image
                        productModel.DefaultPictureModel.ImageUrl = "https://localhost:54077/images/thumbs/default-women-image_615.png";

                    }
                }
            }

            if (eventMessage.Model is ProductEmailAFriendModel emailAFriendModel)
            {
                //customization
                var orders = await _orderService.SearchOrdersAsync(customerId: (await _workContext.GetCurrentCustomerAsync()).Id);

                //check order status code
                var isValid = orders.Where(a => a.OrderStatus == OrderStatus.OrderActive).SingleOrDefault();

                if (isValid == null)
                {
                    //Dispaly Upgrade View
                    emailAFriendModel.Result = await _localizationService.GetResourceAsync("Orders.UpgradeSubscription.Message");
                    //ModelState.AddModelError("", await _localizationService.GetResourceAsync("Orders.UpgradeSubscription.Message"));
                    //return View("_UpgradeSubscription.cshtml", model);
                }
            }

            //this is for related products 
            if (eventMessage.Model is ProductOverviewModel productOverviewModel)
            {

            }

            //this is for header links model .
            //Can be used to hide and show shopping cart link based on catogory
            //show shopping cart in pricing page and hide in all other categories
            if (eventMessage.Model is HeaderLinksModel headerLinksModel)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(false);

                if (currentPageUrl.Contains("pricing", StringComparison.InvariantCultureIgnoreCase))
                    headerLinksModel.ShoppingCartEnabled = true;
                else
                    headerLinksModel.ShoppingCartEnabled = false;

            }

            if (eventMessage.Model is SearchBoxModel searchBoxModel)
            {

            }

            //return Task.FromResult(0);
        }

        #endregion
    }

}