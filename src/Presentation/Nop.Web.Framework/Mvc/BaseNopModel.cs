using System.Web.Mvc;

namespace Nop.Web.Framework.Mvc
{
    public partial class BaseNopModel
    {
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }
    }
    public partial class BaseNopEntityModel : BaseNopModel
    {
        public virtual int Id { get; set; }
    }
}
