using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.ScheduleTasks;
using Nop.Services.Attributes;

namespace Nop.Services.Custom.Extensions.ScheduleTask
{
    /// <summary>
    /// Represents a task for synching products and customers
    /// </summary>
    public partial class SynchronizeProductsAndCustomers : IScheduleTask
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;

        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductTagService _productTagService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;

        protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public SynchronizeProductsAndCustomers(CustomerSettings customerSettings,
            ILogger logger,
            ICustomerService customerService,
            IWorkContext workContext,
            IProductService productService,
            IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
            IWorkflowMessageService workflowMessageService,
            IGenericAttributeService genericAttributeService,
            ISpecificationAttributeService specificationAttributeService,
            LocalizationSettings localizationSettings,
            IOrderService orderService,
            ICustomerActivityService customerActivityService,
            IStoreContext storeContext
            )
        {
            _customerSettings = customerSettings;
            _customerService = customerService;
            _logger = logger;
            _customerActivityService = customerActivityService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _productService = productService;
            _customerAttributeParser = customerAttributeParser;
            _specificationAttributeService = specificationAttributeService;
            _genericAttributeService = genericAttributeService;
            _localizationSettings = localizationSettings;
            _orderService = orderService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _logger.InformationAsync("Synchronize Products And Customers ExecuteAsync method start");

            var customer = await _workContext.GetCurrentCustomerAsync();

            NotifyCustomerAvailabilityAsync();

            await _logger.InformationAsync("Synchronize Products And Customers ExecuteAsync method end");

        }

        public int GetCustomerProfileTypeId(string customerAttributesXml)
        {
            var profileTypeId = (int)ProductAndCustomerAttributeEnum.ProfileType;
            var profileType = _customerAttributeParser.ParseValues(customerAttributesXml, profileTypeId).FirstOrDefault();

            var customerProfileTypeId = Convert.ToInt32(profileType);
            return customerProfileTypeId;
        }

        protected async void CreateProductSpecificationAttributeMappingsAsync(Product product, string customerAttributesXml, string gender)
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

        public async void SyncAndVerifyCustomersAndProducts()
        {

            await _logger.InformationAsync("SyncAndVerifyCustomersAndProducts method start");
            var customers = await _customerService.GetAllCustomersAsync();

            foreach (var customer in customers.Where(x => x.VendorId > 0).ToList())
            {
                var customerAttributesXml = customer.CustomCustomerAttributesXML;
                var customerProfileTypeId = GetCustomerProfileTypeId(customerAttributesXml);

                customerProfileTypeId = customer.CustomerProfileTypeId;
                //int.TryParse(customer.CustomerProfileTypeId, out int? customerProfileId);

                var productId = customer.VendorId;
                var product = await _productService.GetProductByIdAsync(productId);

                if (product == null)
                {
                    // no associated product found for this customer
                    await _logger.ErrorAsync("No product found for the customer", null, customer);
                    continue;
                }

                if (customerProfileTypeId == 0)
                {
                    //These customers may be built in or admin accounts with out customer profiletypeid
                    continue;
                }

                var shortDescriptionId = (int)ProductAndCustomerAttributeEnum.ShortDescription;
                var fullDescriptionId = (int)ProductAndCustomerAttributeEnum.FullDescription;

                var primaryTechnologyAttributeValues = _customerAttributeParser.ParseValues(customerAttributesXml, (int)ProductAndCustomerAttributeEnum.PrimaryTechnology).ToList();
                var secondaryTechnologyAttributeValues = _customerAttributeParser.ParseValues(customerAttributesXml, (int)ProductAndCustomerAttributeEnum.SecondaryTechnology).ToList();

                var totalAttributeValues = primaryTechnologyAttributeValues.Select(int.Parse).ToList();
                totalAttributeValues.AddRange(secondaryTechnologyAttributeValues.Select(x => int.Parse(x)).Distinct().ToList());

                var attributeValuesFromSpec = (await _specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(totalAttributeValues.ToArray())).Select(x => x.Name).ToList();

                var firstName = customer.FirstName;
                var lastName = customer.LastName;
                var gender = customer.Gender;

                // create product model with customer data
                //var productModel = new Nop.Web.Areas.Admin.Models.Catalog.ProductModel()
                //{
                //    Name = firstName + " " + lastName,
                //    Published = true,
                //    ShortDescription = _customerAttributeParser.ParseValues(customerAttributesXml, shortDescriptionId).FirstOrDefault(),
                //    FullDescription = _customerAttributeParser.ParseValues(customerAttributesXml, fullDescriptionId).FirstOrDefault(),
                //    ShowOnHomepage = false,
                //    AllowCustomerReviews = true,
                //    IsShipEnabled = false,
                //    Price = 500,
                //    SelectedCategoryIds = categoryIds,
                //    OrderMinimumQuantity = 1,
                //    OrderMaximumQuantity = 1000,
                //    IsTaxExempt = true,
                //    //below are mandatory feilds otherwise the product will not be visible in front end store
                //    Sku = $"SKU_{firstName}_{lastName}",
                //    ProductTemplateId = 1, // simple product template
                //    ProductTypeId = (int)ProductType.SimpleProduct,
                //    VisibleIndividually = true,
                //    // Set product vendor id to customer id. AKA VendorId means Customer Id
                //    VendorId = customer.Id
                //};

                //var product = productModel.ToEntity<Product>();
                //product.CreatedOnUtc = DateTime.UtcNow;
                //product.UpdatedOnUtc = DateTime.UtcNow;

                ////product creation
                //await _productService.InsertProductAsync(product);

                //product categories mappings
                //await SaveCategoryMappingsAsync(product, productModel);

                //set SEO settings otherwise the product wont be visible in front end
                //productModel.SeName = await _urlRecordService.ValidateSeNameAsync(product, productModel.SeName, product.Name, true);
                //await _urlRecordService.SaveSlugAsync(product, productModel.SeName, 0);

                //Update customer with Productid as VendorId. Here Vendor Id means Product Id
                customer.VendorId = product.Id;
                await _customerService.UpdateCustomerAsync(customer);

                //create product specification attribute mappings
                CreateProductSpecificationAttributeMappingsAsync(product, customerAttributesXml, gender);
            }
        }

        public async void SyncCategoriesAndTechnologies()
        {
            await _logger.InformationAsync("SyncCategoriesAndTechnologies method execution started");

            var customers = await _customerService.GetAllCustomersAsync();

            foreach (var customer in customers.Where(x => x.VendorId > 0).ToList())
            {
                var customerAttributesXml = customer.CustomCustomerAttributesXML;
                var customerProfileTypeId = GetCustomerProfileTypeId(customerAttributesXml);

                customerProfileTypeId = customer.CustomerProfileTypeId;
                //int.TryParse(customer.CustomerProfileTypeId, out int? customerProfileId);

                var productId = customer.VendorId;
                var product = await _productService.GetProductByIdAsync(productId);

                if (product == null)
                {
                    // no associated product found for this customer
                    await _logger.ErrorAsync("No product found for the customer", null, customer);
                    continue;
                }

                if (customerProfileTypeId == 0)
                {
                    //These customers may be built in or admin accounts with out customer profiletypeid
                    continue;
                }

                var shortDescriptionId = (int)ProductAndCustomerAttributeEnum.ShortDescription;
                var fullDescriptionId = (int)ProductAndCustomerAttributeEnum.FullDescription;

                var primaryTechnologyAttributeValues = _customerAttributeParser.ParseValues(customerAttributesXml, (int)ProductAndCustomerAttributeEnum.PrimaryTechnology).ToList();
                var secondaryTechnologyAttributeValues = _customerAttributeParser.ParseValues(customerAttributesXml, (int)ProductAndCustomerAttributeEnum.SecondaryTechnology).ToList();

                var totalAttributeValues = primaryTechnologyAttributeValues.Select(int.Parse).ToList();
                totalAttributeValues.AddRange(secondaryTechnologyAttributeValues.Select(x => int.Parse(x)).Distinct().ToList());

                var attributeValuesFromSpec = (await _specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(totalAttributeValues.ToArray())).Select(x => x.Name).ToList();

                var firstName = customer.FirstName;
                var lastName = customer.LastName;
                var gender = customer.Gender;

                await _logger.InformationAsync("SyncCategoriesAndTechnologies method execution completed");
            }
        }

        public async void SyncRelatedProducts()
        {
            await _logger.InformationAsync("Sync Related Products method execution started");

            var customers = await _customerService.GetAllCustomersAsync();
            var products = await _productService.SearchProductsAsync();


            //get all updated customers in last a day
            await _customerService.GetAllCustomersAsync();

            foreach (var product in products.Where(x => x.VendorId > 0).ToList())
            {
                var specificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(product.Id);

                var primaryTechSpecAttributeId = (int)ProductAndCustomerAttributeEnum.PrimaryTechnology;

                var digits = new List<int>
                {
                    primaryTechSpecAttributeId
                };

                var specAttributeId = digits.ToArray();

                var primaryTechspecAttributeOptionIds = await _specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(specAttributeId);

                var finalSpecOptionIds = new List<int>();

                foreach (var item in primaryTechspecAttributeOptionIds)
                {
                    if (specificationAttributes.Any(x => x.SpecificationAttributeOptionId == item.Id))
                    {
                        finalSpecOptionIds.Add(item.Id);
                    }
                }

                var relatedProducts = await _specificationAttributeService.GetProductsBySpecificationAttributeIdAsync(primaryTechSpecAttributeId, 0, 10);
            }

            await _logger.InformationAsync("Sync Related Products method execution completed");
        }

        private async void NotifyCustomerAvailabilityAsync()
        {
            await _logger.InformationAsync("Notify Customer Availability Start");

            //get customer activity log
            var customerActivity = await _customerActivityService.GetAllActivitiesAsync(activityLogTypeId: 155, entityName: "Customer");

            //customers who changed status back to available
            //var customers = customerActivity.Where(x => x.EntityId != 1).Select(x => new { x.CustomerId, x.Id });

            var cus = await _genericAttributeService.GetAttributesAsync("Customer", NopCustomerDefaults.NotifiedAboutCustomerAvailabilityAttribute, "False");
            var customers = cus.Select(x => new { CustomerId = x.EntityId, GenericAttributeId = x.Id });

            //notify interested customers.
            //i.e customers who have similar primary technologies (specification attribute options)
            foreach (var customer in customers)
            {
                var currentCustomer = await _customerService.GetCustomerByIdAsync(customer.CustomerId);

                var customerAttributeXml = currentCustomer.CustomCustomerAttributesXML;
                var primaryTechAttributeIds = _customerAttributeParser.ParseValues(customerAttributeXml, (int)ProductAndCustomerAttributeEnum.PrimaryTechnology)
                                                                      .ToList().Select(int.Parse);

                var specOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(primaryTechAttributeIds.ToArray());

                // get similar target customers
                var targetProducts = (await _productService.SearchProductsAsync(filteredSpecOptions: specOptions)).ToList();
                var targetCustomerIds = targetProducts.Where(x => x.VendorId > 0).Select(x => x.VendorId).ToArray();

                var targetCustomers = await _customerService.GetCustomersByIdsAsync(targetCustomerIds);

                var product = await _productService.GetProductByIdAsync(currentCustomer.VendorId);

                //notify the target customers that current customer is avialable
                foreach (var customerToNotify in targetCustomers)
                {
                    await _workflowMessageService.SendCustomerAvilableNotificationToOtherCustomersAsync(product, customerToNotify, _localizationSettings.DefaultAdminLanguageId, specOptions);
                }

                //update activity log EntityId column to 1 so that notification is sent only one time when this customer is available back
                //var activity = await _customerActivityService.GetActivityByIdAsync(customer.Id);
                //activity.EntityId = 1;
                //await _customerActivityService.UpdateActivityLogAsync(activity);

                // update ga to say that notifications are sent
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.NotifiedAboutCustomerAvailabilityAttribute, true, (await _storeContext.GetCurrentStoreAsync()).Id);
            }

            await _logger.InformationAsync("Notify Customer Availability End");
        }


        #endregion
    }
}