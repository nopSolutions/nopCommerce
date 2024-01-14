using AO.Services.Products.Models;
using System.Collections.Generic;
using System.Linq;

namespace AO.Services.Extensions
{
    public static class VariantExtensions
    {
        public static void SetSupplierProductId(this VariantData data)
        {
            string brand = data.Brand.ToLower();
            switch (brand)
            {
                case "tatonka":
                    {
                        if (data.SupplierProductId.Contains("-"))
                            data.SupplierProductId = data.SupplierProductId.Substring(0, data.SupplierProductId.IndexOf("-"));
                        break;
                    }
                default:
                    {
                        // This is to fix that STM often has itemnumbers specific for each variant, we need the actual productnumber
                        // We need to check for 2 "-" as Gerber is using 1 "-" in some products not otherwise connected
                        if (data.SupplierProductId.Contains("-") && data.SupplierProductId.IndexOf("-") != data.SupplierProductId.LastIndexOf("-"))
                            data.SupplierProductId = data.SupplierProductId.Substring(0, data.SupplierProductId.IndexOf("-"));

                        break;
                    }
            }
        }

        public static void SetSizeString(this VariantData data)
        {
            if (string.IsNullOrEmpty(data.SizeStr) && data.OriginalTitle.Contains("\"\""))
            {
                string tmpSize = data.OriginalTitle.Substring(data.OriginalTitle.IndexOf("\"\""));
                tmpSize = tmpSize.Replace("\"", "").Trim();
                data.SizeStr = tmpSize;
            }
        }

        /// <summary>
        /// Will remove items without valid EAN and remove duplicate EANs
        /// </summary>
        /// <param name="variantData"></param>
        /// <returns></returns>
        public static List<VariantData> Cleanup(this List<VariantData> variantData)
        {
            // This will remove rows with invalid EAN numbers
            var cleanedItems = variantData.Where(p => p.EAN != null && p.EAN.Length > 10).ToList();

            // This will remove any EAN duplicates
            cleanedItems = cleanedItems.GroupBy(x => x.EAN).Select(y => y.First()).ToList();
            
            return cleanedItems;
        }

        /// <summary>
        /// Will remove items without valid EAN and remove duplicate EANs
        /// </summary>
        /// <param name="variantData"></param>
        /// <returns></returns>
        public static List<VariantData> CleanupForCreation(this List<VariantData> variantData)
        {
            // This will remove rows with invalid EAN numbers
            var cleanedItems = Cleanup(variantData);
            
            // This will remove all with missing brand name            
            cleanedItems = cleanedItems.Where(p => p.Brand?.Trim() != "").ToList();

            // This will remove all thats no longer available
            cleanedItems = cleanedItems.Where(p => p.OriginalTitle.ToLower().Trim() != "udgået" && p.OriginalTitle.ToLower().Trim() != "spærret").ToList();

            // This will remove all without salesprice
            cleanedItems = cleanedItems.Where(p => p.RetailPrice > 0).ToList();

            // This will remove all without any one in stock. (this removal should only be performed before creation, not stockupdate).
            cleanedItems = cleanedItems.Where(p => p.StockCount > 0).ToList();

            return cleanedItems;
        }

        public static string Clean(this string input)
        {
            string stripped = input.Replace("\"", "");

            return stripped.Trim();
        }
    }
}
