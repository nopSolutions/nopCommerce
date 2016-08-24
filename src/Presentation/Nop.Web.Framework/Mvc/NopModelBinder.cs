using System.ComponentModel;
using System.Linq;
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

        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, object value)
        {
            //check if data type of value is System.String
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                //developers can mark properties to be excluded from trimming with [NoTrim] attribute
                if (propertyDescriptor.Attributes.Cast<object>().All(a => a.GetType() != typeof (NoTrimAttribute)))
                {
                        var stringValue = (string)value;
                        value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim();
                }
            }

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }
    }
}