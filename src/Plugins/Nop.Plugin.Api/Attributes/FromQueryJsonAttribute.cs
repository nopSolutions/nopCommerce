using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Attributes
{
	public class FromQueryJsonAttribute : ModelBinderAttribute
	{
        public FromQueryJsonAttribute()
        {
            BinderType = typeof(JsonQueryModelBinder);
        }

        public FromQueryJsonAttribute(string paramName)
            : this()
        {
            Name = paramName;
        }
    }
}
