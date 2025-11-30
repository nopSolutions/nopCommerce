using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.ModelBinders
{
	public class JsonQueryModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			try
			{
				var name = bindingContext.ModelName;
				var json = bindingContext.HttpContext.Request.Query.FirstOrDefault(x => x.Key == name).Value;
				var targetType = bindingContext.ModelType;
				var model = JsonConvert.DeserializeObject(json, targetType);
				if (model is null)
				{
					model = Activator.CreateInstance(targetType);
				}
				bindingContext.Model = model;
				bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
			}
			catch (JsonException)
			{
				bindingContext.Result = ModelBindingResult.Failed();
			}
			return Task.CompletedTask;
		}
	}
}
