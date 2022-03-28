using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.AbcCore.Extensions
{
    public static class OrderItemExtensions
    {
        public static bool IsPickup(this OrderItem oi)
        {
            return !string.IsNullOrWhiteSpace(oi.AttributeDescription) &&
                   oi.AttributeDescription.Contains("Pickup: ");
        }

        public static bool IsHomeDelivery(this OrderItem oi)
        {
            return !string.IsNullOrWhiteSpace(oi.AttributeDescription) &&
                   oi.AttributeDescription.Contains("Home Delivery: ");
        }

        public static bool HasWarranty(this OrderItem oi)
        {
            return !string.IsNullOrWhiteSpace(oi.AttributeDescription) &&
                   oi.AttributeDescription.Contains("Warranty: ");
        }

        public static string GetMattressSize(this OrderItem oi)
        {
            if (oi.AttributeDescription == null || !oi.AttributeDescription.Contains("Mattress Size:"))
            {
                return null;
            }
            var mattressSizeIndex = oi.AttributeDescription.IndexOf("Mattress Size:");
            var mattressSizeString = oi.AttributeDescription.Substring(mattressSizeIndex);
            mattressSizeString = mattressSizeString.Substring(14, mattressSizeString.IndexOf("<br />") - 14);
            var sizeAdjustIndex = mattressSizeString.IndexOf("[");
            if (sizeAdjustIndex > 0)
            {
                mattressSizeString = mattressSizeString.Substring(0, sizeAdjustIndex);
            }

            return mattressSizeString.Replace("&quot;", "\"").Replace("&amp;", "&").Trim();
        }

        public static string GetBase(this OrderItem oi)
        {
            if (oi.AttributeDescription == null || !oi.AttributeDescription.Contains("Base ("))
            {
                return null;
            }
            var baseIndex = oi.AttributeDescription.IndexOf("Base (");
            var baseString = oi.AttributeDescription.Substring(baseIndex);
            baseString = baseString.Substring(6, baseString.IndexOf("<br />") - 6);
            baseString = baseString.Substring(0, baseString.IndexOf("["));
            baseString = baseString.Substring(baseString.IndexOf(":") + 1);

            return baseString.Replace("&quot;", "\"").Replace("&amp;", "&").Trim();
        }

        public static string GetFreeGift(this OrderItem oi)
        {
            if (oi.AttributeDescription == null || !oi.AttributeDescription.Contains("Free Gift:"))
            {
                return null;
            }
            var freeGiftIndex = oi.AttributeDescription.IndexOf("Free Gift:");
            var freeGiftString = oi.AttributeDescription.Substring(freeGiftIndex);
            freeGiftString = freeGiftString.Substring(10, freeGiftString.IndexOf("<br />") - 10);

            return freeGiftString.Replace("&quot;", "\"")
                                 .Replace("&amp;", "&")
                                 .Replace("&gt;", ">")
                                 .Replace("&lt;", "<").Trim();
        }

        public static string GetMattressProtector(this OrderItem oi)
        {
            if (oi.AttributeDescription == null || !oi.AttributeDescription.Contains("Mattress Protector ("))
            {
                return null;
            }
            var protectorIndex = oi.AttributeDescription.IndexOf("Mattress Protector (");
            var protectorString = oi.AttributeDescription.Substring(protectorIndex);
            protectorString = protectorString.Substring(20, protectorString.IndexOf("<br />") - 20);
            protectorString = protectorString.Substring(0, protectorString.IndexOf("["));
            protectorString = protectorString.Substring(protectorString.IndexOf(":") + 1);

            return protectorString.Replace("&quot;", "\"").Replace("&amp;", "&").Trim();
        }

        public static string GetFrame(this OrderItem oi)
        {
            if (oi.AttributeDescription == null || !oi.AttributeDescription.Contains("Frame ("))
            {
                return null;
            }
            var protectorIndex = oi.AttributeDescription.IndexOf("Frame (");
            var protectorString = oi.AttributeDescription.Substring(protectorIndex);
            protectorString = protectorString.Substring(7, protectorString.IndexOf("<br />") - 7);
            protectorString = protectorString.Substring(0, protectorString.IndexOf("["));
            protectorString = protectorString.Substring(protectorString.IndexOf(":") + 1);

            return protectorString.Replace("&quot;", "\"").Replace("&amp;", "&").Trim();
        }

        public static (List<OrderItem> pickupItems, List<OrderItem> shippingItems) SplitByPickupAndShipping(this IList<OrderItem> ois)
        {
            var pickupItems = ois.Where(oi => oi.IsPickup());
            var shippingItems = ois.Except(pickupItems);

            return (pickupItems.ToList(), shippingItems.ToList());
        }
    }
}
