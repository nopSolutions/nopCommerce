using System;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a request for tax calculation
    /// </summary>
    public partial class CalculateTaxRequest
    {
        /// <summary>
        /// Gets or sets a customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets a product
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public TaxAddress Address { get; set; }

        /// <summary>
        /// Gets or sets a tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a current store identifier
        /// </summary>
        public int CurrentStoreId { get; set; }

        /// <summary>
        /// Represents information about address for tax calculation
        /// </summary>
        [Serializable]
        public class TaxAddress
        {
            /// <summary>
            /// Gets or sets an indentifier of appropriate "Address" entity (if available). Otherwise, 0
            /// </summary>
            public int AddressId { get; set; }

            /// <summary>
            /// Gets or sets the country identifier
            /// </summary>
            public int? CountryId { get; set; }

            /// <summary>
            /// Gets or sets the state/province identifier
            /// </summary>
            public int? StateProvinceId { get; set; }

            /// <summary>
            /// Gets or sets the county
            /// </summary>
            public string County { get; set; }

            /// <summary>
            /// Gets or sets the city
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// Gets or sets the address 1
            /// </summary>
            public string Address1 { get; set; }

            /// <summary>
            /// Gets or sets the address 2
            /// </summary>
            public string Address2 { get; set; }

            /// <summary>
            /// Gets or sets the zip/postal code
            /// </summary>
            public string ZipPostalCode { get; set; }
        }
    }
}
