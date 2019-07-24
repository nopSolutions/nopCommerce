using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product review
    /// </summary>
    public partial class ProductReview : BaseEntity
    {
        private ICollection<ProductReviewHelpfulness> _productReviewHelpfulnessEntries;
        private ICollection<ProductReviewReviewTypeMapping> _productReviewReviewTypeMappingEntries;

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the review text
        /// </summary>
        public string ReviewText { get; set; }

        /// <summary>
        /// Gets or sets the reply text
        /// </summary>
        public string ReplyText { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the customer is already notified of the reply to review
        /// </summary>
        public bool CustomerNotifiedOfReply { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the store
        /// </summary>
        public virtual Store Store { get; set; }

        /// <summary>
        /// Gets the entries of product review helpfulness
        /// </summary>
        public virtual ICollection<ProductReviewHelpfulness> ProductReviewHelpfulnessEntries
        {
            get => _productReviewHelpfulnessEntries ?? (_productReviewHelpfulnessEntries = new List<ProductReviewHelpfulness>());
            protected set => _productReviewHelpfulnessEntries = value;
        }

        /// <summary>
        /// Gets the entries of product reviews
        /// </summary>
        public virtual ICollection<ProductReviewReviewTypeMapping> ProductReviewReviewTypeMappingEntries
        {
            get { return _productReviewReviewTypeMappingEntries ?? (_productReviewReviewTypeMappingEntries = new List<ProductReviewReviewTypeMapping>()); }
            protected set { _productReviewReviewTypeMappingEntries = value; }
        }
    }
}
