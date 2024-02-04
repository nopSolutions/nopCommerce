using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO;
using Nop.Plugin.Api.DTO.Categories;

namespace Nop.Plugin.Api.DTO.Products
{
	public class ProductCategoriesRootObjectDto : ISerializableObject
	{
		[JsonProperty("product_categories")]
		public List<ProductCategoriesDto> ProductCategories { get; set; }

		public string GetPrimaryPropertyName()
		{
			return "product_categories";
		}

		public Type GetPrimaryPropertyType()
		{
			return ProductCategories.GetType();
		}
	}

	public class ProductCategoriesDto
	{
		[JsonProperty("product_id")]
		public int ProductId { get; set; }

		[JsonProperty("categories", Required = Required.Always)]
		public List<CategoryDto> Categories { get; set; }
	}
}
