using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
	// JsonProperty is used only for swagger
	[ModelBinder(typeof(ParametersModelBinder<ProductCategoriesParametersModel>))]
	public class ProductCategoriesParametersModel
	{
		/// <summary>
		///     list of product ids to include in response
		/// </summary>
		[JsonProperty("product_ids")]
		public List<int> ProductIds { get; set; }
	}
}
