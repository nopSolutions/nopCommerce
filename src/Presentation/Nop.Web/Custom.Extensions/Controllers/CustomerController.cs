using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Controllers
{
    public partial class CustomerController
    {

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
                // Set product vendor id to customer id. AKA VendorId means Customer Id
                VendorId = customer.Id
            };

            var product = productModel.ToEntity<Product>();
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;

            //product creation
            await _productService.InsertProductAsync(product);

            //product categories mappings
            await SaveCategoryMappingsAsync(product, productModel);

            //set SEO settings otherwise the product wont be visible in front end
            productModel.SeName = await _urlRecordService.ValidateSeNameAsync(product, productModel.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, productModel.SeName, 0);

            //Update customer with Productid as VendorId. Here Vendor Id means Product Id
            customer.VendorId = product.Id;
            await _customerService.UpdateCustomerAsync(customer);

            //create product specification attribute mappings
            await CreateProductSpecificationAttributeMappingsAsync(product, customerAttributesXml, gender);

            //update customer availability in order to send notifications to other similar customers
            await CreateOrUpdateCustomerCurrentAvailabilityAsync(customer, customerAttributesXml);

        }

        private async Task UpdateProductAsync(Customer customer, string customerAttributesXml, string firstName, string lastName)
        {
            var customerProfileTypeId = GetCustomerProfileTypeId(customerAttributesXml);

            var shortDescriptionId = (int)ProductAndCustomerAttributeEnum.ShortDescription;
            var fullDescriptionId = (int)ProductAndCustomerAttributeEnum.FullDescription;

            // Update product with customer data
            var productModel = new Nop.Web.Areas.Admin.Models.Catalog.ProductModel()
            {
                Id = customer.VendorId,
                Name = firstName + " " + lastName,
                Published = true,
                ShortDescription = _customerAttributeParser.ParseValues(customerAttributesXml, shortDescriptionId).FirstOrDefault(),
                FullDescription = _customerAttributeParser.ParseValues(customerAttributesXml, fullDescriptionId).FirstOrDefault(),
                ShowOnHomepage = false,
                AllowCustomerReviews = true,
                IsShipEnabled = false,
                Price = 500,
                SelectedCategoryIds = new List<int>() { customerProfileTypeId },
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 1,
                IsTaxExempt = true,
                // Below are mandatory feilds otherwise the product will not be visible in front end store
                Sku = $"SKU_{firstName}_{lastName}",
                ProductTemplateId = 1, // Simple product template
                ProductTypeId = 5, // simple or grouped here we have only simple products
                VisibleIndividually = true,
                // Set product vendor id to customer id. AKA VendorId means Customer Id
                VendorId = customer.Id
            };


            //update product with out productmodel

            //var product1 = await _productService.GetProductByIdAsync(customer.VendorId);
            //product1.ShortDescription = _customerAttributeParser.ParseValues(customerAttributesXml, shortDescriptionId).FirstOrDefault();
            //product1.FullDescription = _customerAttributeParser.ParseValues(customerAttributesXml, fullDescriptionId).FirstOrDefault();
            //product1.Name = firstName + " " + lastName;
            //product1.Sku = $"SKU_{firstName}_{lastName}";


            var product = productModel.ToEntity<Product>();
            product.UpdatedOnUtc = DateTime.UtcNow;

            //Product updation
            await _productService.UpdateProductAsync(product);

            //Product categories mappings
            //SaveCategoryMappings(product, productModel);

            //Set SEO settings otherwise the product wont be visible in front end
            productModel.SeName = await _urlRecordService.ValidateSeNameAsync(product, productModel.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, productModel.SeName, 0);

            //Update Product Specification Attributes which is product to specification attribute mapping
            await UpdateProductSpecificationAttributeMappingsAsync(product, customerAttributesXml);

            //Update Product Tags
            //UpdateProductTags(product);

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

        protected virtual async Task UpdateProductSpecificationAttributeMappingsAsync(Product product, string customerAttributesXml)
        {
            var spectAttributes = await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync();

            foreach (var attribute in spectAttributes)
            {
                var selectedAttributeOptionIds = _customerAttributeParser.ParseValues(customerAttributesXml, attribute.Id).ToList();

                var existingProductSpecAttributeOptions = await _specificationAttributeService.GetProductSpecificationsBySpecificationAttributeIdAsync(product.Id, attribute.Id);

                //delete product spec attribute mappings
                foreach (var existingSpecAttributOption in existingProductSpecAttributeOptions)
                {
                    if (!selectedAttributeOptionIds.Contains(existingSpecAttributOption.SpecificationAttributeOptionId.ToString()))
                        await _specificationAttributeService.DeleteProductSpecificationAttributeAsync(existingSpecAttributOption);
                }

                //gender specification attribute mapping
                //if (attribute.Id == _customerSettings.GenderSpecificationAttributeId)
                //{
                //    var psaGender = new ProductSpecificationAttribute
                //    {
                //        AllowFiltering = true,
                //        ProductId = product.Id,
                //        SpecificationAttributeOptionId = (gender == "M") ? _customerSettings.GenderMaleSpecificationAttributeOptionId : _customerSettings.GenderFeMaleSpecificationAttributeOptionId,
                //        ShowOnProductPage = true
                //    };
                //    await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psaGender);
                //}

                //create product to spec attribute mapping
                foreach (var attributeOptionId in selectedAttributeOptionIds)
                {
                    if ((await _specificationAttributeService.GetProductSpecificationAttributesAsync(product.Id, Convert.ToInt32(attributeOptionId))).Count == 0)
                    {
                        //assign specification attribute to product(customer)
                        var psa = new ProductSpecificationAttribute
                        {
                            AllowFiltering = true,
                            ProductId = product.Id,
                            SpecificationAttributeOptionId = Convert.ToInt32(attributeOptionId),
                            ShowOnProductPage = true
                        };

                        await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psa);
                    }
                }
            }
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

        [HttpPost]
        public virtual Task<JsonResult> GetAvailableFlagFileNames()
        {
            var flagNames = _fileProvider
                .EnumerateFiles(_fileProvider.GetAbsolutePath(FLAGS_PATH), "*.png")
                .Select(_fileProvider.GetFileName).Take(10)
                .ToList();

            //get only supported countris USA,India,Austrailia,UK,Canada
            //flagNames=flagNames.Select(x=>x.Contains())

            var availableFlagFileNames = flagNames.Select(async flagName => new SelectListItem
            {
                Text = await GetCountryByTwoLetterIsoCode(flagName),
                Value = flagName
            }).ToList();

            return Task.FromResult(Json(availableFlagFileNames));
        }

        public async Task<string> GetCountryByTwoLetterIsoCode(string countryImageName)
        {
            var countryIsoCode = countryImageName.Replace(".png", "");
            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryIsoCode);
            return country?.Name ?? "";
        }

        public int GetCustomerProfileTypeId(string customerAttributesXml)
        {
            var profileTypeId = (int)ProductAndCustomerAttributeEnum.ProfileType;
            var profileType = _customerAttributeParser.ParseValues(customerAttributesXml, profileTypeId).FirstOrDefault();

            var customerProfileTypeId = profileType != null ? Convert.ToInt32(profileType) : 0;
            return customerProfileTypeId;
        }

        [HttpGet]
        public async Task<bool> UpdateCustomerProfileType(int customerProfileTypeId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            customer.CustomerProfileTypeId = customerProfileTypeId;
            await _customerService.UpdateCustomerAsync(customer);
            return true;
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

        public async Task<bool> CreateCustomerAfflicateAsync(Customer customer)
        {

            return true;
        }

        public virtual async Task Create(AffiliateModel model, bool continueEditing)
        {
            await Task.FromResult(0);
        }

        public virtual async Task<IActionResult> Affiliations()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (customer == null)
                return RedirectToRoute("Homepage");

            var model = await _customerModelFactory.PrepareAffiliatedCustomersModelAsync(customer);

            return View(model);
        }
    }
}
