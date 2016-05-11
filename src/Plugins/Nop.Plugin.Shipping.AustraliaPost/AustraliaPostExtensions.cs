using Newtonsoft.Json.Linq;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Shipping.AustraliaPost
{
    public static class AustraliaPostExtensions
    {
        public static ShippingOption ParseShippingOption(this JObject obj)
        {
            if (obj.HasValues)
            {
                var shippingOption = new ShippingOption();
                foreach (var property in obj.Properties())
                {
                    switch (property.Name.ToLower())
                    {
                        case "name":
                            shippingOption.Name = string.Format("Australia Post. {0}", property.Value);
                            break;
                        case "price":
                            decimal rate;
                            if (decimal.TryParse(property.Value.ToString(), out rate))
                                shippingOption.Rate = rate;
                            break;
                    }
                }
                return shippingOption;
            }

            return null;
        }
    }
}