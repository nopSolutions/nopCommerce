using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            //trim entered values
            //check if data type of value is System.String
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                //developers can mark properties to be excluded from trimming with [NoTrim] attribute
                if (!propertyDescriptor.Attributes.Cast<object>().Any(a => a.GetType() == typeof(NoTrimAttribute)))
                {
                    //check if the property doesn't have [DataType(DataType.Password)] attribute, because we don't want to trim password
                    var dataTypeAttributes = propertyDescriptor.Attributes.OfType<DataTypeAttribute>().FirstOrDefault();
                    if (!(dataTypeAttributes != null && dataTypeAttributes.DataType == DataType.Password))
                    {
                        var stringValue = (string)value;
                        value = string.IsNullOrEmpty(stringValue) ? stringValue : stringValue.Trim();
                    }
                }
            }

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }
    }
}