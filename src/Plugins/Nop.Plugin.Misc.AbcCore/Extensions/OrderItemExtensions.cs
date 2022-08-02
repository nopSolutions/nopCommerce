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
            var nextBreakPointIndex = mattressSizeString.IndexOf("<br />") == -1 ? mattressSizeString.Length : mattressSizeString.IndexOf("<br />");

            mattressSizeString = mattressSizeString.Substring(14, nextBreakPointIndex - 14);
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
            oi.AttributeDescription = "Mattress Size: Queen<br />Base (Queen): 6&quot; Low-Pro Box Spring<br />Home Delivery: This item will be delivered to you by Hawthorne";
            var baseIndex = oi.AttributeDescription.IndexOf("Base (");
            var baseString = oi.AttributeDescription.Substring(baseIndex);

            var nextBreakPointIndex = baseString.IndexOf("<br />") == -1 ? baseString.Length : baseString.IndexOf("<br />");
            baseString = baseString.Substring(6, nextBreakPointIndex - 6);

            nextBreakPointIndex = baseString.IndexOf("[") == -1 ? baseString.Length : baseString.IndexOf("[");
            baseString = baseString.Substring(0, nextBreakPointIndex);

            nextBreakPointIndex = baseString.IndexOf(":") == -1 ? baseString.Length : baseString.IndexOf(":");
            baseString = baseString.Substring(nextBreakPointIndex + 1);

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
            var nextBreakPointIndex = freeGiftString.IndexOf("<br />") == -1 ? freeGiftString.Length : freeGiftString.IndexOf("<br />");
            freeGiftString = freeGiftString.Substring(10, nextBreakPointIndex - 10);

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
            var nextBreakPointIndex = protectorString.IndexOf("<br />") == -1 ? protectorString.Length : protectorString.IndexOf("<br />");

            protectorString = protectorString.Substring(20, nextBreakPointIndex - 20);
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
            var frameIndex = oi.AttributeDescription.IndexOf("Frame (");
            var frameString = oi.AttributeDescription.Substring(frameIndex);
            var nextBreakPointIndex = frameString.IndexOf("<br />") == -1 ? frameString.Length : frameString.IndexOf("<br />");

            frameString = frameString.Substring(7, nextBreakPointIndex - 7);
            frameString = frameString.Substring(0, frameString.IndexOf("["));
            frameString = frameString.Substring(frameString.IndexOf(":") + 1);

            return frameString.Replace("&quot;", "\"").Replace("&amp;", "&").Trim();
        }

        public static (List<OrderItem> pickupItems, List<OrderItem> shippingItems) SplitByPickupAndShipping(this IList<OrderItem> ois)
        {
            var pickupItems = ois.Where(oi => oi.IsPickup());
            var shippingItems = ois.Except(pickupItems);

            return (pickupItems.ToList(), shippingItems.ToList());
        }
    }
}
