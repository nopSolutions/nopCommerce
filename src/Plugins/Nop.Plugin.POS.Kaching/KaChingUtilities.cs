using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Nop.Plugin.POS.Kaching
{
    internal class KaChingUtilities
    {
        internal static bool ImageExists(string imageUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageUrl);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (response.StatusCode == HttpStatusCode.OK);
                }
            }
            catch
            {
                return false;
            }
        }

        internal static string GetValidImageUrl(string imageUrl)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uri))
            {
                return "";
            }
            return uri.AbsoluteUri;
        }

        public enum LocaleKey
        {
            ShortDescription,
            Description,
            MetaTitle,
            MetaDescription,
            FullDescription,
            Name
        }

        public enum EntityName
        {
            Topic,
            Manufacturer,
            Product,
            Category,
            Vendor
        }

        public enum LocaleKeyGroup
        {
            Manufacturer,
            Product,
            ProductAttributeValue,
            ProductAttribute,
            Category,
            DeliveryDate
        }
    }
}