using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nop.Web.Framework
{
    public class TelerikGridModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var context = new TelerikGridContext();
            int pageNumber;
            int pageSize;

            int.TryParse(controllerContext.RequestContext.HttpContext.Request.QueryString["Grid-page"], out pageNumber);
            int.TryParse(controllerContext.RequestContext.HttpContext.Request.QueryString["Grid-size"], out pageSize);
            
            context.PageNumber = (pageNumber < 1 ? 1 : pageNumber);
            context.PageSize = (pageSize == 0 ? 10 : pageSize);

            return context;
        }
    }
    public class TelerikGridContext
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
