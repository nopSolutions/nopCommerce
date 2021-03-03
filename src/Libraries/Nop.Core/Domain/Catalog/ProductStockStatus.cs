namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents product stock quantity status
    /// </summary>
    public enum ProductStockStatus
    {
        /// <summary>
        /// Product is out of stock
        /// </summary>
        OutOfStock = 0,

        /// <summary>
        /// Product has low stock
        /// </summary>
        LowStock = 1,

        /// <summary>
        /// Product is in stock
        /// </summary>
        InStock = 2,
    }
}