using System.Collections.Generic;

namespace Nop.Services.Common.Pdf
{
    /// <summary>
    /// Represents the data source for an catalog document
    /// </summary>
    public partial class CatalogSource : DocumentSource
    {
        #region Properties

        /// <summary>
        /// Gets or sets entries of the catalog
        /// </summary>
        public List<CatalogItem> Products { get; set; }

        #endregion
    }
}