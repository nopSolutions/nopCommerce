using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Authorization.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly IFactory<Product> _factory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductApiService _productApiService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IUrlRecordService _urlRecordService;

        public ProductsController(
            IProductApiService productApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IProductService productService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IFactory<Product> factory,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICustomerService customerService,
            IDiscountService discountService,
            IPictureService pictureService,
            IManufacturerService manufacturerService,
            IProductTagService productTagService,
            IProductAttributeService productAttributeService,
            IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService,
                                         customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _manufacturerService = manufacturerService;
            _productTagService = productTagService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products", Name = "GetProducts")]
        [AuthorizePermission("PublicStoreAllowNavigation")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProducts([FromQuery] ProductsParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                             parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId,
                                                             parameters.VendorName, parameters.PublishedStatus, parameters.ManufacturerPartNumbers, parameters.IsDownload)
                                                .WhereAwait(async p => await StoreMappingService.AuthorizeAsync(p));

            IList<ProductDto> productsAsDtos = await allProducts.SelectAwait(async product => await _dtoHelper.PrepareProductDTOAsync(product)).ToListAsync();

            var productsRootObject = new ProductsRootObjectDto
            {
                Products = productsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields ?? "");

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/count", Name = "GetProductsCount")]
        [AuthorizePermission("PublicStoreAllowNavigation")]
        [ProducesResponseType(typeof(ProductsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductsCount([FromQuery] ProductsCountParametersModel parameters)
        {
            var allProductsCount = await _productApiService.GetProductsCountAsync(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                       parameters.UpdatedAtMax, parameters.PublishedStatus, parameters.VendorName,
                                                                       parameters.CategoryId, manufacturerPartNumbers: null, parameters.IsDownload);

            var productsCountRootObject = new ProductsCountRootObject
            {
                Count = allProductsCount
            };

            return Ok(productsCountRootObject);
        }

        /// <summary>
        ///     Retrieve product by spcified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/{id}", Name = "GetProductById")]
        [AuthorizePermission("PublicStoreAllowNavigation")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, fields);
            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [Route("/api/products/categories", Name = "GetProductCategories")]
        [AuthorizePermission("PublicStoreAllowNavigation")]
        [ProducesResponseType(typeof(ProductCategoriesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductCategories([FromQuery] ProductCategoriesParametersModel parameters, [FromServices] ICategoryApiService categoryApiService)
        {
            if (parameters.ProductIds is null)
            {
                return Error(HttpStatusCode.BadRequest, "product_ids", "Product ids is null");
            }

            var productCategories = await categoryApiService.GetProductCategories(parameters.ProductIds);

            var productCategoriesRootObject = new ProductCategoriesRootObjectDto
            {
                ProductCategories = await productCategories.SelectAwait(async prodCats => new ProductCategoriesDto
                {
                    ProductId = prodCats.Key,
                    Categories = await prodCats.Value.SelectAwait(async cat => await _dtoHelper.PrepareCategoryDTOAsync(cat)).ToListAsync()
                }).ToListAsync()

                //ProductCategories = await productCategories.ToDictionaryAwaitAsync
                //(
                //	keySelector: prodCats => ValueTask.FromResult(prodCats.Key),
                //	elementSelector: async prodCats => await prodCats.Value.SelectAwait(async cat => await _dtoHelper.PrepareCategoryDTOAsync(cat)).ToListAsync()
                //)
            };

            return Ok(productCategoriesRootObject);
        }

        [HttpPost]
        [Route("/api/products", Name = "CreateProduct")]
        [AuthorizePermission("ManageProducts")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProduct(
            [FromBody]
            [ModelBinder(typeof(JsonModelBinder<ProductDto>))]
            Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var product = await _factory.InitializeAsync();
            productDelta.Merge(product);

            await _productService.InsertProductAsync(product);

            await UpdateProductPicturesAsync(product, productDelta.Dto.Images);

            await _productTagService.UpdateProductTagsAsync(product, productDelta.Dto.Tags?.ToArray() ?? Array.Empty<string>());

            await UpdateProductManufacturersAsync(product, productDelta.Dto.ManufacturerIds);

            await UpdateAssociatedProductsAsync(product, productDelta.Dto.AssociatedProductIds);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(product, productDelta.Dto.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, seName, 0);

            await UpdateAclRolesAsync(product, productDelta.Dto.RoleIds);

            await UpdateDiscountMappingsAsync(product, productDelta.Dto.DiscountIds);

            await UpdateStoreMappingsAsync(product, productDelta.Dto.StoreIds);

            UpdateRequiredProducts(product, productDelta.Dto.RequiredProductIds);

            await _productService.UpdateProductAsync(product);

            await CustomerActivityService.InsertActivityAsync("AddNewProduct", await LocalizationService.GetResourceAsync("ActivityLog.AddNewProduct"), product);

            // Preparing the result dto of the new product
            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/products/{id}", Name = "UpdateProduct")]
        [AuthorizePermission("ManageProducts")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProduct(
            [FromBody]
            [ModelBinder(typeof(JsonModelBinder<ProductDto>))]
            Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var product = _productApiService.GetProductById(productDelta.Dto.Id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            productDelta.Merge(product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            await _productService.UpdateProductAsync(product);

            await UpdateProductAttributesAsync(product, productDelta);

            await UpdateProductAttributeCombinationsAsync(product, productDelta);

            await UpdateProductPicturesAsync(product, productDelta.Dto.Images);

            await _productTagService.UpdateProductTagsAsync(product, productDelta.Dto.Tags != null ? productDelta.Dto.Tags.ToArray() : Array.Empty<string>());

            await UpdateProductManufacturersAsync(product, productDelta.Dto.ManufacturerIds);

            await UpdateAssociatedProductsAsync(product, productDelta.Dto.AssociatedProductIds);

            // Update the SeName if specified
            if (productDelta.Dto.SeName != null)
            {
                var seName = await _urlRecordService.ValidateSeNameAsync(product, productDelta.Dto.SeName, product.Name, true);
                await _urlRecordService.SaveSlugAsync(product, seName, 0);
            }

            await UpdateDiscountMappingsAsync(product, productDelta.Dto.DiscountIds);

            await UpdateStoreMappingsAsync(product, productDelta.Dto.StoreIds);

            await UpdateAclRolesAsync(product, productDelta.Dto.RoleIds);

            await _productService.UpdateProductAsync(product);

            await CustomerActivityService.InsertActivityAsync("UpdateProduct", await LocalizationService.GetResourceAsync("ActivityLog.UpdateProduct"), product);

            // Preparing the result dto of the new product
            var productDto = await _dtoHelper.PrepareProductDTOAsync(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/products/{id}", Name = "DeleteProduct")]
        [AuthorizePermission("ManageProducts")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            await _productService.DeleteProductAsync(product);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProduct", string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteProduct"), product.Name), product);

            return new RawJsonActionResult("{}");
        }

        #region Private methods

        private async Task UpdateProductPicturesAsync(Product entityToUpdate, List<ImageMappingDto> setPictures)
        {
            // If no pictures are specified means we don't have to update anything
            if (setPictures == null)
            {
                return;
            }

            // delete unused product pictures
            var productPictures = await _productService.GetProductPicturesByProductIdAsync(entityToUpdate.Id);
            var unusedProductPictures = productPictures.Where(x => setPictures.All(y => y.Id != x.Id)).ToList();
            foreach (var unusedProductPicture in unusedProductPictures)
            {
                var picture = await PictureService.GetPictureByIdAsync(unusedProductPicture.PictureId);
                if (picture == null)
                {
                    throw new ArgumentException("No picture found with the specified id");
                }
                await PictureService.DeletePictureAsync(picture);
            }

            foreach (var imageDto in setPictures)
            {
                if (imageDto.Id > 0)
                {
                    // update existing product picture
                    var productPictureToUpdate = productPictures.FirstOrDefault(x => x.Id == imageDto.Id);
                    if (productPictureToUpdate != null && imageDto.Position > 0)
                    {
                        productPictureToUpdate.DisplayOrder = imageDto.Position;
                        await _productService.UpdateProductPictureAsync(productPictureToUpdate);
                    }
                }
                else
                {
                    // add new product picture
                    var newPicture = await PictureService.InsertPictureAsync(imageDto.Binary, imageDto.MimeType, string.Empty);
                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        PictureId = newPicture.Id,
                        ProductId = entityToUpdate.Id,
                        DisplayOrder = imageDto.Position
                    });
                }
            }
        }

        private async Task UpdateProductAttributesAsync(Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute mappings are specified means we don't have to update anything
            if (productDtoDelta.Dto.ProductAttributeMappings == null)
            {
                return;
            }

            // delete unused product attribute mappings
            var toBeUpdatedIds = productDtoDelta.Dto.ProductAttributeMappings.Where(y => y.Id != 0).Select(x => x.Id);
            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(entityToUpdate.Id);
            var unusedProductAttributeMappings = productAttributeMappings.Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeMapping in unusedProductAttributeMappings)
            {
                await _productAttributeService.DeleteProductAttributeMappingAsync(unusedProductAttributeMapping);
            }

            foreach (var productAttributeMappingDto in productDtoDelta.Dto.ProductAttributeMappings)
            {
                if (productAttributeMappingDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeMappingToUpdate = productAttributeMappings.FirstOrDefault(x => x.Id == productAttributeMappingDto.Id);
                    if (productAttributeMappingToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeMappingDto, productAttributeMappingToUpdate, false);

                        await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMappingToUpdate);

                        await UpdateProductAttributeValuesAsync(productAttributeMappingDto, productDtoDelta);
                    }
                }
                else
                {
                    var newProductAttributeMapping = new ProductAttributeMapping
                    {
                        ProductId = entityToUpdate.Id
                    };

                    productDtoDelta.Merge(productAttributeMappingDto, newProductAttributeMapping);

                    // add new product attribute
                    await _productAttributeService.InsertProductAttributeMappingAsync(newProductAttributeMapping);
                }
            }

            //product attribute combinations
            if (productDtoDelta.Dto.ProductAttributeCombinations == null)
            {
                return;
            }
            //delete unused product attribute
            var toBeUpdatedAttributeCombinationIds = productDtoDelta.Dto.ProductAttributeCombinations.Where(y => y.Id != 0).Select(x => x.Id);
            var productAttributeCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(entityToUpdate.Id);
            var unusedProductAttributeCombinations = productAttributeCombinations.Where(x => !toBeUpdatedAttributeCombinationIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeCombination in unusedProductAttributeCombinations)
            {
                await _productAttributeService.DeleteProductAttributeCombinationAsync(unusedProductAttributeCombination);
            }

            foreach (var productAttributeCombination in productDtoDelta.Dto.ProductAttributeCombinations)
            {
                var productAttributeCombinationToUpdate = productAttributeCombinations.FirstOrDefault(x => x.Id == productAttributeCombination.Id);
                if (productAttributeCombination.Id > 0)
                {
                    productDtoDelta.Merge(productAttributeCombination, productAttributeCombinationToUpdate, false);
                    //update existing product attribute combination
                    await _productAttributeService.UpdateProductAttributeCombinationAsync(productAttributeCombinationToUpdate);
                }
                else
                {
                    var newProductAttributeCombination = new ProductAttributeCombination
                    {
                        ProductId = entityToUpdate.Id
                    };

                    productDtoDelta.Merge(productAttributeCombination, newProductAttributeCombination, false);

                    // add new product attribute
                    await _productAttributeService.InsertProductAttributeCombinationAsync(newProductAttributeCombination);
                }
            }

        }

        private async Task UpdateProductAttributeCombinationsAsync(Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {

            //product attribute combinations
            if (productDtoDelta.Dto.ProductAttributeCombinations == null)
            {
                return;
            }
            //delete unused product attribute
            var toBeUpdatedAttributeCombinationIds = productDtoDelta.Dto.ProductAttributeCombinations.Where(y => y.Id != 0).Select(x => x.Id);
            var productAttributeCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(entityToUpdate.Id);
            var unusedProductAttributeCombinations = productAttributeCombinations.Where(x => !toBeUpdatedAttributeCombinationIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeCombination in unusedProductAttributeCombinations)
            {
                await _productAttributeService.DeleteProductAttributeCombinationAsync(unusedProductAttributeCombination);
            }

            foreach (var productAttributeCombination in productDtoDelta.Dto.ProductAttributeCombinations)
            {
                var productAttributeCombinationToUpdate = productAttributeCombinations.FirstOrDefault(x => x.Id == productAttributeCombination.Id);
                if (productAttributeCombination.Id > 0)
                {
                    productDtoDelta.Merge(productAttributeCombination, productAttributeCombinationToUpdate, false);
                    //update existing product attribute combination
                    await _productAttributeService.UpdateProductAttributeCombinationAsync(productAttributeCombinationToUpdate);
                }
                else
                {
                    var newProductAttributeCombination = new ProductAttributeCombination
                    {
                        ProductId = entityToUpdate.Id
                    };

                    productDtoDelta.Merge(productAttributeCombination, newProductAttributeCombination, false);

                    // add new product attribute
                    await _productAttributeService.InsertProductAttributeCombinationAsync(newProductAttributeCombination);
                }
            }

        }

        private async Task UpdateProductAttributeValuesAsync(ProductAttributeMappingDto productAttributeMappingDto, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute values are specified means we don't have to update anything
            if (productAttributeMappingDto.ProductAttributeValues == null)
            {
                return;
            }

            // delete unused product attribute values
            var toBeUpdatedIds = productAttributeMappingDto.ProductAttributeValues.Where(y => y.Id != 0).Select(x => x.Id);

            var unusedProductAttributeValues = (await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingDto.Id)).Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (var unusedProductAttributeValue in unusedProductAttributeValues)
            {
                await _productAttributeService.DeleteProductAttributeValueAsync(unusedProductAttributeValue);
            }

            foreach (var productAttributeValueDto in productAttributeMappingDto.ProductAttributeValues)
            {
                if (productAttributeValueDto.Id > 0)
                {
                    // update existing product attribute mapping
                    var productAttributeValueToUpdate = await _productAttributeService.GetProductAttributeValueByIdAsync(productAttributeValueDto.Id);
                    if (productAttributeValueToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeValueDto, productAttributeValueToUpdate, false);

                        await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValueToUpdate);
                    }
                }
                else
                {
                    var newProductAttributeValue = new ProductAttributeValue();
                    productDtoDelta.Merge(productAttributeValueDto, newProductAttributeValue);

                    newProductAttributeValue.ProductAttributeMappingId = productAttributeMappingDto.Id;
                    // add new product attribute value
                    await _productAttributeService.InsertProductAttributeValueAsync(newProductAttributeValue);
                }
            }
        }

        private async Task UpdateDiscountMappingsAsync(Product product, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
            {
                return;
            }

            var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true);
            var appliedProductDiscount = await DiscountService.GetAppliedDiscountsAsync(product);
            foreach (var discount in allDiscounts)
            {
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (appliedProductDiscount.Count(d => d.Id == discount.Id) == 0)
                    {
                        appliedProductDiscount.Add(discount);
                    }
                }
                else
                {
                    //remove discount
                    if (appliedProductDiscount.Count(d => d.Id == discount.Id) > 0)
                    {
                        appliedProductDiscount.Remove(discount);
                    }
                }
            }

            await _productService.UpdateProductAsync(product);
            await _productService.UpdateHasDiscountsAppliedAsync(product);
        }

        private async Task UpdateProductManufacturersAsync(Product product, List<int> passedManufacturerIds)
        {
            // If no manufacturers specified then there is nothing to map 
            if (passedManufacturerIds == null)
            {
                return;
            }
            var productmanufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id);
            var unusedProductManufacturers = productmanufacturers.Where(x => !passedManufacturerIds.Contains(x.ManufacturerId)).ToList();



            foreach (var passedManufacturerId in passedManufacturerIds)
            {
                // not part of existing manufacturers so we will create a new one
                if (productmanufacturers.All(x => x.ManufacturerId != passedManufacturerId))
                {
                    // if manufacturer does not exist we simply ignore it, otherwise add it to the product
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(passedManufacturerId);
                    if (manufacturer != null)
                    {
                        await _manufacturerService.InsertProductManufacturerAsync(new ProductManufacturer
                        {
                            ProductId = product.Id,
                            ManufacturerId = manufacturer.Id
                        });
                    }
                }
            }
            // remove all manufacturers that are not passed
            foreach (var unusedProductManufacturer in unusedProductManufacturers)
            {
                await _manufacturerService.DeleteProductManufacturerAsync(unusedProductManufacturer);
            }
        }

        private async Task UpdateAssociatedProductsAsync(Product product, List<int> passedAssociatedProductIds)
        {
            // If no associated products specified then there is nothing to map 
            if (passedAssociatedProductIds == null)
            {
                return;
            }

            var noLongerAssociatedProducts = (await _productService.GetAssociatedProductsAsync(product.Id, showHidden: true))
                               .Where(p => !passedAssociatedProductIds.Contains(p.Id));

            // update all products that are no longer associated with our product
            foreach (var noLongerAssocuatedProduct in noLongerAssociatedProducts)
            {
                noLongerAssocuatedProduct.ParentGroupedProductId = 0;
                await _productService.UpdateProductAsync(noLongerAssocuatedProduct);
            }

            var newAssociatedProducts = await _productService.GetProductsByIdsAsync(passedAssociatedProductIds.ToArray());
            foreach (var newAssociatedProduct in newAssociatedProducts)
            {
                newAssociatedProduct.ParentGroupedProductId = product.Id;
                await _productService.UpdateProductAsync(newAssociatedProduct);
            }
        }

        private void UpdateRequiredProducts(Product product, IList<int> requiredProductIds)
        {
            if (requiredProductIds is null)
                product.RequiredProductIds = null;
            else
                product.RequiredProductIds = string.Join(',', requiredProductIds.Select(id => id.ToString()));
        }

        #endregion
    }
}
