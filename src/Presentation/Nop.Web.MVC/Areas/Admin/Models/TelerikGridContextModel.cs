using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    public class TelerikGridContextModel : BaseNopModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public override void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int pageNumber;
            int pageSize;

            int.TryParse(controllerContext.RequestContext.HttpContext.Request.QueryString["Grid-page"], out pageNumber);
            int.TryParse(controllerContext.RequestContext.HttpContext.Request.QueryString["Grid-size"], out pageSize);

            PageNumber = (pageNumber < 1 ? 1 : pageNumber);
            PageSize = (pageSize == 0 ? 10 : pageSize);
        }
    }
}
