using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.DTO;
using Nop.Plugin.Api.DTO.Categories;
using Nop.Plugin.Api.DTO.Images;
using Nop.Plugin.Api.DTO.Languages;
using Nop.Plugin.Api.DTO.Manufacturers;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.DTO.ProductAttributes;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.DTO.ShoppingCarts;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.DTO.Stores;
using Nop.Plugin.Api.DTO.Warehouses;
using Nop.Plugin.Api.DTOs.Taxes;
using Nop.Plugin.Api.DTOs.Topics;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Services;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;

namespace Nop.Plugin.Api.Helpers
{
	public class DTOHelper : IDTOHelper
	{
		private readonly IAclService _aclService;
		private readonly ICustomerService _customerService;
		private readonly ILanguageService _languageService;
		private readonly ILocalizationService _localizationService;
		private readonly IPictureService _pictureService;
		private readonly IProductAttributeService _productAttributeService;
		private readonly IProductService _productService;
		private readonly IProductTagService _productTagService;
		private readonly IDiscountService _discountService;
		private readonly IManufacturerService _manufacturerService;
		private readonly IOrderService _orderService;
		private readonly IProductAttributeConverter _productAttributeConverter;
		private readonly IAddressService _addressService;
		private readonly IAuthenticationService _authenticationService;
		private readonly ICustomerApiService _customerApiService;
		private readonly ICurrencyService _currencyService;
		private readonly IStoreMappingService _storeMappingService;
		private readonly IStoreService _storeService;
		private readonly IUrlRecordService _urlRecordService;

		private readonly Lazy<Task<Language>> _customerLanguage;

		public DTOHelper(
			IProductService productService,
			IAclService aclService,
			IStoreMappingService storeMappingService,
			IPictureService pictureService,
			IProductAttributeService productAttributeService,
			ICustomerService customerService,
			ILanguageService languageService,
			IStoreService storeService,
			ILocalizationService localizationService,
			IUrlRecordService urlRecordService,
			IProductTagService productTagService,
			IDiscountService discountService,
			IManufacturerService manufacturerService,
			IOrderService orderService,
			IProductAttributeConverter productAttributeConverter,
			IAddressService addressService,
			IAuthenticationService authenticationService,
			ICustomerApiService customerApiService,
			ICurrencyService currencyService)
		{
			_productService = productService;
			_aclService = aclService;
			_storeMappingService = storeMappingService;
			_pictureService = pictureService;
			_productAttributeService = productAttributeService;
			_customerService = customerService;
			_languageService = languageService;
			_storeService = storeService;
			_localizationService = localizationService;
			_urlRecordService = urlRecordService;
			_productTagService = productTagService;
			_discountService = discountService;
			_manufacturerService = manufacturerService;
			_orderService = orderService;
			_productAttributeConverter = productAttributeConverter;
			_addressService = addressService;
			_authenticationService = authenticationService;
			_customerApiService = customerApiService;
			_currencyService = currencyService;

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

		public async Task<OrderDto> PrepareOrderDTOAsync(Order order)
		{
			var orderDto = order.ToDto();

			orderDto.OrderItems = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item => await PrepareOrderItemDTOAsync(item)).ToListAsync();

			orderDto.BillingAddress = (await _addressService.GetAddressByIdAsync(order.BillingAddressId))?.ToDto();
			orderDto.ShippingAddress = (await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0))?.ToDto();

			return orderDto;
		}
        public async Task<WarehouseDto> PrepareWarehouseDtoAsync(Warehouse warehouse)
        {
            var warehouseDto = warehouse.ToDto();

            warehouseDto.Address = (await _addressService.GetAddressByIdAsync(warehouse.AddressId))?.ToDto();

            return warehouseDto;
        }

        public TopicDto PrepareTopicDTO(Topic topic)
		{
			var topicDto = topic.ToDto();
			return topicDto;
		}

		public async Task<ShoppingCartItemDto> PrepareShoppingCartItemDTOAsync(ShoppingCartItem shoppingCartItem)
		{
			var dto = shoppingCartItem.ToDto();
			dto.ProductDto = await PrepareProductDTOAsync(await _productService.GetProductByIdAsync(shoppingCartItem.ProductId));
			dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
			return dto;
		}

		public async Task<OrderItemDto> PrepareOrderItemDTOAsync(OrderItem orderItem)
		{
			var dto = orderItem.ToDto();
			dto.Product = await PrepareProductDTOAsync(await _productService.GetProductByIdAsync(orderItem.ProductId));
			dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
			return dto;
		}

		public async Task<StoreDto> PrepareStoreDTOAsync(Store store)
		{
			var storeDto = store.ToDto();

			storeDto.Languages = (await _languageService.GetAllLanguagesAsync(storeId: store.Id)).Select(x => x.ToDto()).ToList();

			// localization
			if (await _customerLanguage.Value is { Id: var languageId })
			{
				storeDto.Name = await _localizationService.GetLocalizedAsync(store, x => x.Name, languageId);

				storeDto.Currencies = await (await _currencyService.GetAllCurrenciesAsync(storeId: store.Id)).SelectAwait(currency => prepareCurrencyDtoAsync(currency, languageId, _localizationService)).ToListAsync();

				static async ValueTask<CurrencyDto> prepareCurrencyDtoAsync(Currency currency, int languageId, ILocalizationService localizationService)
				{
					var currencyDto = currency.ToDto();
					// localization
					currencyDto.Name = await localizationService.GetLocalizedAsync(currency, x => x.Name, languageId);
					return currencyDto;
				}
			}
			else
			{
				storeDto.Currencies = (await _currencyService.GetAllCurrenciesAsync(storeId: store.Id)).Select(currency => currency.ToDto()).ToList();
			}

			return storeDto;

		}

		public async Task<LanguageDto> PrepareLanguageDtoAsync(Language language)
		{
			var languageDto = language.ToDto();

			languageDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(language)).Select(mapping => mapping.StoreId).ToList();

			if (languageDto.StoreIds.Count == 0)
			{
				languageDto.StoreIds = (await _storeService.GetAllStoresAsync()).Select(s => s.Id).ToList();
			}

			return languageDto;
		}

		public async Task<CurrencyDto> PrepareCurrencyDtoAsync(Currency currency)
		{
			var currencyDto = currency.ToDto();

			currencyDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(currency)).Select(mapping => mapping.StoreId).ToList();

			if (currencyDto.StoreIds.Count == 0)
			{
				currencyDto.StoreIds = (await _storeService.GetAllStoresAsync()).Select(s => s.Id).ToList();
			}

			// localization
			if (await _customerLanguage.Value is { Id: var languageId })
			{
				currencyDto.Name = await _localizationService.GetLocalizedAsync(currency, x => x.Name, languageId);
			}

			return currencyDto;
		}

		public ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute)
		{
			return productAttribute.ToDto();
		}

		public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute)
		{
			return productSpecificationAttribute.ToDto();
		}

		public SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute)
		{
			return specificationAttribute.ToDto();
		}

		public async Task<ManufacturerDto> PrepareManufacturerDtoAsync(Manufacturer manufacturer)
		{
			var manufacturerDto = manufacturer.ToDto();

			var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
			var imageDto = await PrepareImageDtoAsync(picture);

			if (imageDto != null)
			{
				manufacturerDto.Image = imageDto;
			}

			manufacturerDto.SeName = await _urlRecordService.GetSeNameAsync(manufacturer);
			manufacturerDto.DiscountIds = (await _discountService.GetAppliedDiscountsAsync(manufacturer)).Select(discount => discount.Id).ToList();
			manufacturerDto.RoleIds = (await _aclService.GetAclRecordsAsync(manufacturer)).Select(acl => acl.CustomerRoleId).ToList();
			manufacturerDto.StoreIds = (await _storeMappingService.GetStoreMappingsAsync(manufacturer)).Select(mapping => mapping.StoreId)
														   .ToList();

			var allLanguages = await _languageService.GetAllLanguagesAsync();

			// localization
			if (await _customerLanguage.Value is { Id: var languageId })
			{
				manufacturerDto.Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name, languageId);
				manufacturerDto.Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description, languageId);
			}

			return manufacturerDto;
		}

		public void PrepareProductSpecificationAttributes(IEnumerable<ProductSpecificationAttribute> productSpecificationAttributes, ProductDto productDto)
		{
			if (productDto.ProductSpecificationAttributes == null)
			{
				productDto.ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();
			}

			foreach (var productSpecificationAttribute in productSpecificationAttributes)
			{
				var productSpecificationAttributeDto = PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

				if (productSpecificationAttributeDto != null)
				{
					productDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);
				}
			}
		}

		public AddressDto PrepareAddressDTO(Address address)
		{
			var addressDto = address.ToDto();
			return addressDto;
		}
		public TaxCategoryDto prepareTaxCategoryDto(TaxCategory taxCategory)
		{
			var taxCategoryDto = taxCategory.ToDto();
			return taxCategoryDto;
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