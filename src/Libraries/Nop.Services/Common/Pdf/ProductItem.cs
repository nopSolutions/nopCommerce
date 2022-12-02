using System.Collections.Generic;
using System.ComponentModel;

namespace Nop.Services.Common.Pdf
{
    /// <summary>
    /// Represents product entry
    /// </summary>
    public partial class ProductItem
    {
        #region Ctor

        public ProductItem()
        {
            ProductAttributes = new();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        [DisplayName("Pdf.Product.Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the product SKU
        /// </summary>
        [DisplayName("Pdf.Product.Sku")]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets a vendor name
        /// </summary>
        [DisplayName("Pdf.Product.VendorName")]
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the product price
        /// </summary>
        [DisplayName("Pdf.Product.Price")]
        public string Price { get; set; }

        /// <summary>
        /// Gets or sets the product quantity
        /// </summary>
        [DisplayName("Pdf.Product.Quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the product total
        /// </summary>
        [DisplayName("Pdf.Product.Total")]
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the product attribute description
        /// </summary>
        public List<string> ProductAttributes { get; set; }

        #endregion
    }
}