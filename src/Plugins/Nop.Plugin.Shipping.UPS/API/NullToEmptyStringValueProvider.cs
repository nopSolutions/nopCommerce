using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Shipping.UPS.API;

public class NullToEmptyStringValueProvider : IValueProvider
{
    private readonly PropertyInfo _memberInfo;

    public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
    {
        _memberInfo = memberInfo;
    }

    public object GetValue(object target)
    {
        var result = _memberInfo.GetValue(target);

        if (_memberInfo.PropertyType != typeof(string))
            return result;

        var attributes = _memberInfo
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute)).FirstOrDefault() as System.ComponentModel.DataAnnotations.RequiredAttribute;
            
        if ((attributes?.AllowEmptyStrings ?? false) && result == null)
            result = "";

        return result;
    }

    public void SetValue(object target, object value)
    {
        _memberInfo.SetValue(target, value);
    }
}