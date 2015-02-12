namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Product review approved event
    /// </summary>
    public class ProductReviewApprovedEvent
    {
        private readonly ProductReview _productReview;

        public ProductReviewApprovedEvent(ProductReview productReview)
        {
            this._productReview = productReview;
        }

        public ProductReview ProductReview
        {
            get { return _productReview; }
        }
    }
}