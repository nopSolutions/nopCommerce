using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nop.Web.Framework.Mvc
{
    public class BaseNopModel
    {
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }
    }
    public class BaseNopEntityModel : BaseNopModel
    {
        public virtual int Id { get; set; }
    }
}
