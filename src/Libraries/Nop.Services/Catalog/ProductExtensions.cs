using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Finds a related product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Related product</returns>
        public static RelatedProduct FindRelatedProduct(this IList<RelatedProduct> source,
            int productId1, int productId2)
        {
            foreach (RelatedProduct relatedProduct in source)
                if (relatedProduct.ProductId1 == productId1 && relatedProduct.ProductId2 == productId2)
                    return relatedProduct;
            return null;
        }

        /// <summary>
        /// Finds a cross-sell product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Cross-sell product</returns>
        public static CrossSellProduct FindCrossSellProduct(this IList<CrossSellProduct> source,
            int productId1, int productId2)
        {
            foreach (CrossSellProduct crossSellProduct in source)
                if (crossSellProduct.ProductId1 == productId1 && crossSellProduct.ProductId2 == productId2)
                    return crossSellProduct;
            return null;
        }

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="localizationService">Localization service</param>
        /// <returns>The stock message</returns>
        public static string FormatStockMessage(this Product product, ILocalizationService localizationService)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            string stockMessage = string.Empty;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock
                && product.DisplayStockAvailability)
            {
                switch (product.BackorderMode)
                {
                    case BackorderMode.NoBackorders:
                        {
                            if (product.StockQuantity > 0)
                            {
                                if (product.DisplayStockQuantity)
                                {
                                    //display "in stock" with stock quantity
                                    stockMessage = string.Format(localizationService.GetResource("Products.Availability.InStockWithQuantity"), product.StockQuantity);
                                }
                                else
                                {
                                    //display "in stock" without stock quantity
                                    stockMessage = localizationService.GetResource("Products.Availability.InStock");
                                }
                            }
                            else
                            {
                                //display "out of stock"
                                stockMessage = localizationService.GetResource("Products.Availability.OutOfStock");
                            }
                        }
                        break;
                    case BackorderMode.AllowQtyBelow0:
                        {
                            if (product.StockQuantity > 0)
                            {
                                if (product.DisplayStockQuantity)
                                {
                                    //display "in stock" with stock quantity
                                    stockMessage = string.Format(localizationService.GetResource("Products.Availability.InStockWithQuantity"), product.StockQuantity);
                                }
                                else
                                {
                                    //display "in stock" without stock quantity
                                    stockMessage = localizationService.GetResource("Products.Availability.InStock");
                                }
                            }
                            else
                            {
                                //display "in stock" without stock quantity
                                stockMessage = localizationService.GetResource("Products.Availability.InStock");
                            }
                        }
                        break;
                    case BackorderMode.AllowQtyBelow0AndNotifyCustomer:
                        {
                            if (product.StockQuantity > 0)
                            {
                                if (product.DisplayStockQuantity)
                                {
                                    //display "in stock" with stock quantity
                                    stockMessage = string.Format(localizationService.GetResource("Products.Availability.InStockWithQuantity"), product.StockQuantity);
                                }
                                else
                                {
                                    //display "in stock" without stock quantity
                                    stockMessage = localizationService.GetResource("Products.Availability.InStock");
                                }
                            }
                            else
                            {
                                //display "backorder" without stock quantity
                                stockMessage = localizationService.GetResource("Products.Availability.Backordering");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return stockMessage;
        }
        
        /// <summary>
        /// Indicates whether a product tag exists
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Result</returns>
        public static bool ProductTagExists(this Product product,
            int productTagId)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            bool result = product.ProductTags.ToList().Find(pt => pt.Id == productTagId) != null;
            return result;
        }

        /// <summary>
        /// Get a list of allowed quanities (parse 'AllowedQuanities' property)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Result</returns>
        public static int[] ParseAllowedQuatities(this Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var result = new List<int>();
            if (!String.IsNullOrWhiteSpace(product.AllowedQuantities))
            {
                product
                    .AllowedQuantities
                    .Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .ForEach(qtyStr =>
                                 {
                                     int qty = 0;
                                     if (int.TryParse(qtyStr.Trim(), out qty))
                                     {
                                         result.Add(qty);
                                     }
                                 } );
            }

            return result.ToArray();
        }



        /// <summary>
        /// Gets SKU, Manufacturer part number and GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes (XML format)</param>
        /// <param name="productAttributeParser">Product attribute service (used when attributes are specified)</param>
        /// <param name="sku">SKU</param>
        /// <param name="manufacturerPartNumber">Manufacturer part number</param>
        /// <param name="gtin">GTIN</param>
        private static void GetSkuMpnGtin(this Product product, string selectedAttributes, IProductAttributeParser productAttributeParser,
            out string sku, out string manufacturerPartNumber, out string gtin)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            sku = null;
            manufacturerPartNumber = null;
            gtin = null;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                //manage stock by attribute combinations
                if (productAttributeParser == null)
                    throw new ArgumentNullException("productAttributeParser");

                //let's find appropriate record
                var combination = product
                    .ProductVariantAttributeCombinations
                    .FirstOrDefault(x => productAttributeParser.AreProductAttributesEqual(x.AttributesXml, selectedAttributes));
                if (combination != null)
                {
                    sku = combination.Sku;
                    manufacturerPartNumber = combination.ManufacturerPartNumber;
                    gtin = combination.Gtin;
                }
            }

            if (String.IsNullOrEmpty(sku))
                sku = product.Sku;
            if (String.IsNullOrEmpty(manufacturerPartNumber))
                manufacturerPartNumber = product.ManufacturerPartNumber;
            if (String.IsNullOrEmpty(gtin))
                gtin = product.Gtin;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes (XML format)</param>
        /// <param name="productAttributeParser">Product attribute service (used when attributes are specified)</param>
        /// <returns>SKU</returns>
        public static string FormatSku(this Product product, string selectedAttributes = null, IProductAttributeParser productAttributeParser = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string sku = null;
            string manufacturerPartNumber = null;
            string gtin = null;

            product.GetSkuMpnGtin(selectedAttributes, productAttributeParser,
                out sku, out manufacturerPartNumber, out gtin);

            return sku;
        }

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes (XML format)</param>
        /// <param name="productAttributeParser">Product attribute service (used when attributes are specified)</param>
        /// <returns>Manufacturer part number</returns>
        public static string FormatMpn(this Product product, string selectedAttributes = null, IProductAttributeParser productAttributeParser = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string sku = null;
            string manufacturerPartNumber = null;
            string gtin = null;

            product.GetSkuMpnGtin(selectedAttributes, productAttributeParser,
                out sku, out manufacturerPartNumber, out gtin);

            return manufacturerPartNumber;
        }

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="selectedAttributes">Selected attributes (XML format)</param>
        /// <param name="productAttributeParser">Product attribute service (used when attributes are specified)</param>
        /// <returns>GTIN</returns>
        public static string FormatGtin(this Product product, string selectedAttributes = null, IProductAttributeParser productAttributeParser = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string sku = null;
            string manufacturerPartNumber = null;
            string gtin = null;

            product.GetSkuMpnGtin(selectedAttributes, productAttributeParser,
                out sku, out manufacturerPartNumber, out gtin);

            return gtin;
        }
    }
}
