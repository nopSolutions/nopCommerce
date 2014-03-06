using System.Web.Mvc;

namespace Nop.Web.Framework.Mvc
{
    public class NopModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BaseNopModel)
            {
                ((BaseNopModel)model).BindModel(controllerContext, bindingContext);
            }
            return model;
        }

        protected override System.ComponentModel.PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            bindingContext.PropertyFilter = new System.Predicate<string>(pred);
            var values = base.GetModelProperties(controllerContext, bindingContext);
            return values;
        }

        protected bool pred(string target)
        {
            return true;
        }
    }
}
