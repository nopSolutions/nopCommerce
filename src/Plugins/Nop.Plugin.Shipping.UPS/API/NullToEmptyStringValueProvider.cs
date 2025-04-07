using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Shipping.UPS.API;

public class NullToEmptyStringValueProvider(PropertyInfo memberInfo) : IValueProvider
{
    public object GetValue(object target)
    {
        var result = memberInfo.GetValue(target);

        if (memberInfo.PropertyType != typeof(string))
            return result;

        var attributes = memberInfo
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)).FirstOrDefault() as System.ComponentModel.DataAnnotations.RequiredAttribute;
            
        if ((attributes?.AllowEmptyStrings ?? false) && result == null)
            result = "";

        return result;
    }

    public void SetValue(object target, object value)
    {
        memberInfo.SetValue(target, value);
    }
}