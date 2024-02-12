using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.DTO.Categories;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Helpers
{
    public class DTOHelperCustomized : IDTOHelper
	{
		private readonly IAclService _aclService;
		private readonly ILanguageService _languageService;
		private readonly ILocalizationService _localizationService;
		private readonly IPictureService _pictureService;
		private readonly IProductAttributeService _productAttributeService;
		private readonly IProductService _productService;
		private readonly IProductTagService _productTagService;
		private readonly IDiscountService _discountService;
		private readonly IManufacturerService _manufacturerService;
		private readonly IAuthenticationService _authenticationService;
		private readonly ICustomerApiService _customerApiService;
		private readonly IStoreMappingService _storeMappingService;
		private readonly IUrlRecordService _urlRecordService;

		private readonly Lazy<Task<Language>> _customerLanguage;

		public DTOHelperCustomized(
			IProductService productService,
			IAclService aclService,
			IStoreMappingService storeMappingService,
			IPictureService pictureService,
			IProductAttributeService productAttributeService,
			ILanguageService languageService,
			ILocalizationService localizationService,
			IUrlRecordService urlRecordService,
			IProductTagService productTagService,
			IDiscountService discountService,
			IManufacturerService manufacturerService,
			IAuthenticationService authenticationService,
			ICustomerApiService customerApiService)
		{
			_productService = productService;
			_aclService = aclService;
			_storeMappingService = storeMappingService;
			_pictureService = pictureService;
			_productAttributeService = productAttributeService;
			_languageService = languageService;
			_localizationService = localizationService;
			_urlRecordService = urlRecordService;
			_productTagService = productTagService;
			_discountService = discountService;
			_manufacturerService = manufacturerService;
			_authenticationService = authenticationService;
			_customerApiService = customerApiService;

			_customerLanguage = new Lazy<Task<Language>>(GetAuthenticatedCustomerLanguage);
		}

		public async Task<ProductDto> PrepareProductDTOAsync(Product product)
		{
			var productDto = product.ToDto();
			var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);
			await PrepareProductImagesAsync(productPictures, productDto);

			productDto.SeName = await _urlRecordService.GetSeNameAsync(product);
			productDto.DiscountIds = (await _discountService.GetAppliedDiscountsAsync(product)).Select(discount => discount.Id).ToList();
			productDto.ManufacturerIds = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).Select(pm => pm.Id).ToList();
			productDto.RoleIds = (await _aclService.GetAclRecordsAsync(product)).Select(acl => acl.CustomerRoleId).ToList();
			productDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(product)).Select(mapping => mapping.StoreId).ToList();
			productDto.Tags = (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Select(tag => tag.Name).ToList();

			productDto.AssociatedProductIds = (await _productService.GetAssociatedProductsAsync(product.Id, showHidden: true))
							   .Select(associatedProduct => associatedProduct.Id)
							   .ToList();

			var allLanguages = await _languageService.GetAllLanguagesAsync();

			productDto.RequiredProductIds = _productService.ParseRequiredProductIds(product);

			await PrepareProductAttributesAsync(productDto);

			// localization
			if (await _customerLanguage.Value is { Id: var languageId })
			{
				productDto.Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);
				productDto.ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription, languageId);
				productDto.FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId);
			}

			return productDto;
		}

		public async Task<CategoryDto> PrepareCategoryDTOAsync(Category category)
		{
			var categoryDto = category.ToDto();

			var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
			var imageDto = await PrepareImageDtoAsync(picture);

			if (imageDto != null)
			{
				categoryDto.Image = imageDto;
			}

			categoryDto.SeName = await _urlRecordService.GetSeNameAsync(category);
			categoryDto.DiscountIds = (await _discountService.GetAppliedDiscountsAsync(category)).Select(discount => discount.Id).ToList();
			categoryDto.RoleIds = (await _aclService.GetAclRecordsAsync(category)).Select(acl => acl.CustomerRoleId).ToList();
			categoryDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(category)).Select(mapping => mapping.StoreId)
													   .ToList();

			// localization
			if (await _customerLanguage.Value is { Id: var languageId })
			{
				categoryDto.Name = await _localizationService.GetLocalizedAsync(category, x => x.Name, languageId);
				categoryDto.Description = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId);
			}

			return categoryDto;
		}

		#region Private methods

		private async Task PrepareProductImagesAsync(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
		{
			if (productDto.Images == null)
			{
				productDto.Images = new List<ImageMappingDto>();
			}

			// Here we prepare the resulted dto image.
			foreach (var productPicture in productPictures)
			{
				var imageDto = await PrepareImageDtoAsync(await _pictureService.GetPictureByIdAsync(productPicture.PictureId));

				if (imageDto != null)
				{
					var productImageDto = new ImageMappingDto
					{
						Id = productPicture.Id,
						PictureId = productPicture.PictureId,
						Position = productPicture.DisplayOrder,
						Src = imageDto.Src,
						Attachment = imageDto.Attachment
					};

					productDto.Images.Add(productImageDto);
				}
			}
		}

		private async Task<ImageDto> PrepareImageDtoAsync(Picture picture)
		{
			ImageDto image = null;

			if (picture != null)
			{
				(string url, _) = await _pictureService.GetPictureUrlAsync(picture);

				// We don't use the image from the passed dto directly 
				// because the picture may be passed with src and the result should only include the base64 format.
				image = new ImageDto
				{
					//Attachment = Convert.ToBase64String(picture.PictureBinary),
					Src = url
				};
			}

			return image;
		}

		private async Task PrepareProductAttributesAsync(ProductDto productDto)
		{
			var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productDto.Id);
			var productAttributeCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(productDto.Id);

			if (productDto.ProductAttributeMappings == null)
			{
				productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();
			}

			foreach (var productAttributeMapping in productAttributeMappings)
			{
				var productAttributeMappingDto = await PrepareProductAttributeMappingDtoAsync(productAttributeMapping);

				if (productAttributeMappingDto != null)
				{
					productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
				}
			}

			PrepareProductAttributeCombinations(productAttributeCombinations, productDto);

		}

		private async Task<ProductAttributeMappingDto> PrepareProductAttributeMappingDtoAsync(
			ProductAttributeMapping productAttributeMapping)
		{
			ProductAttributeMappingDto productAttributeMappingDto = null;

			if (productAttributeMapping != null)
			{
				productAttributeMappingDto = new ProductAttributeMappingDto
				{
					Id = productAttributeMapping.Id,
					ProductAttributeId = productAttributeMapping.ProductAttributeId,
					ProductAttributeName = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)).Name,
					TextPrompt = productAttributeMapping.TextPrompt,
					DefaultValue = productAttributeMapping.DefaultValue,
					AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
					DisplayOrder = productAttributeMapping.DisplayOrder,
					IsRequired = productAttributeMapping.IsRequired,
					ProductAttributeValues = await (await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id))
													.SelectAwait(async attributeValue => await PrepareProductAttributeValueDtoAsync(attributeValue,
														await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)))
													.ToListAsync()
				};
			}

			return productAttributeMappingDto;
		}

		private async Task<ProductAttributeValueDto> PrepareProductAttributeValueDtoAsync(
			ProductAttributeValue productAttributeValue,
			Product product)
		{
			ProductAttributeValueDto productAttributeValueDto = null;

			if (productAttributeValue != null)
			{
				productAttributeValueDto = productAttributeValue.ToDto();
				if (productAttributeValue.ImageSquaresPictureId > 0)
				{
					var imageSquaresPicture = await _pictureService.GetPictureByIdAsync(productAttributeValue.ImageSquaresPictureId);
					var imageDto = await PrepareImageDtoAsync(imageSquaresPicture);
					productAttributeValueDto.ImageSquaresImage = imageDto;
				}

				if (productAttributeValue.PictureId > 0)
				{
					// make sure that the picture is mapped to the product
					// This is needed since if you delete the product picture mapping from the nopCommerce administrationthe
					// then the attribute value is not updated and it will point to a picture that has been deleted
					var productPicture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault(pp => pp.PictureId == productAttributeValue.PictureId);
					if (productPicture != null)
					{
						productAttributeValueDto.ProductPictureId = productPicture.Id;
					}
				}
			}

			return productAttributeValueDto;
		}

		private void PrepareProductAttributeCombinations(
			IEnumerable<ProductAttributeCombination> productAttributeCombinations,
			ProductDto productDto)
		{
			productDto.ProductAttributeCombinations = productDto.ProductAttributeCombinations ?? new List<ProductAttributeCombinationDto>();

			foreach (var productAttributeCombination in productAttributeCombinations)
			{
				var productAttributeCombinationDto = PrepareProductAttributeCombinationDto(productAttributeCombination);
				if (productAttributeCombinationDto != null)
				{
					productDto.ProductAttributeCombinations.Add(productAttributeCombinationDto);
				}
			}
		}

		private ProductAttributeCombinationDto PrepareProductAttributeCombinationDto(ProductAttributeCombination productAttributeCombination)
		{
			return productAttributeCombination.ToDto();
		}

		private async Task<Language> GetAuthenticatedCustomerLanguage()
		{
			var customer = await _authenticationService.GetAuthenticatedCustomerAsync();
			return (customer is not null) ? await _customerApiService.GetCustomerLanguageAsync(customer) : null;
		}

		#endregion
	}
}