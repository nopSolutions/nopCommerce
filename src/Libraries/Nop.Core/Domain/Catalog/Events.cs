namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Product review approved event
    /// </summary>
    public class ProductReviewApprovedEvent
    {
        public ProductReviewApprovedEvent(ProductReview productReview)
        {
            this.ProductReview = productReview;
        }

        /// <summary>
        /// Product review
        /// </summary>
        public ProductReview ProductReview { get; private set; }
    }
}