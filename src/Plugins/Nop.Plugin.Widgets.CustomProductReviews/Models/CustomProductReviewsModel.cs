using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.CustomCustomProductReviews.Models
{
    
    public partial record CustomProductReviewOverviewModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public int RatingSum { get; set; }

        public int TotalReviews { get; set; }

        public bool AllowCustomerReviews { get; set; }

        public bool CanAddNewReview { get; set; }
    }

    public partial record CustomProductReviewsModel : BaseNopModel
    {
        public CustomProductReviewsModel()
        {
            Items = new List<CustomProductReviewModel>();
            AddCustomProductReview = new AddCustomProductReviewModel();
            ReviewTypeList = new List<ReviewTypeModel>();
            AddAdditionalCustomProductReviewList = new List<AddCustomProductReviewReviewTypeMappingModel>();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public IList<CustomProductReviewModel> Items { get; set; }

        public AddCustomProductReviewModel AddCustomProductReview { get; set; }

        public IList<ReviewTypeModel> ReviewTypeList { get; set; }

        public IList<AddCustomProductReviewReviewTypeMappingModel> AddAdditionalCustomProductReviewList { get; set; }
    }

    public partial record ReviewTypeModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public bool VisibleToAllCustomers { get; set; }

        public double AverageRating { get; set; }
    }

    public partial record CustomProductReviewModel : BaseNopEntityModel
    {
        public CustomProductReviewModel()
        {
            AdditionalCustomProductReviewList = new List<CustomProductReviewReviewTypeMappingModel>();
        }

        public int CustomerId { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CustomerName { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public int Rating { get; set; }

        public string WrittenOnStr { get; set; }

        public CustomProductReviewHelpfulnessModel Helpfulness { get; set; }

        public IList<CustomProductReviewReviewTypeMappingModel> AdditionalCustomProductReviewList { get; set; }
    }

    public partial record CustomProductReviewHelpfulnessModel : BaseNopModel
    {
        public int CustomProductReviewId { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }
    }

    public partial record AddCustomProductReviewModel : BaseNopModel
    {
        [NopResourceDisplayName("Reviews.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Reviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [NopResourceDisplayName("Reviews.Fields.Rating")]
        public int Rating { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool CanCurrentCustomerLeaveReview { get; set; }

        public bool SuccessfullyAdded { get; set; }

        public bool CanAddNewReview { get; set; }

        public string Result { get; set; }
    }

    public partial record AddCustomProductReviewReviewTypeMappingModel : BaseNopEntityModel
    {
        public int CustomProductReviewId { get; set; }

        public int ReviewTypeId { get; set; }

        public int Rating { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }
    }

    public partial record CustomProductReviewReviewTypeMappingModel : BaseNopEntityModel
    {
        public int CustomProductReviewId { get; set; }

        public int ReviewTypeId { get; set; }

        public int Rating { get; set; }

        public string Name { get; set; }

        public bool VisibleToAllCustomers { get; set; }
    }
}
