using Nop.Web.Framework.Mvc;
using System;


namespace Nop.Web.Models.MyProductReviews
{
    public class MyProductReviewsModel : BaseNopEntityModel
    {
        public string ProductName { get; set; }
        public DateTime DateReviewed { get; set; }
        public string CurrentApprovalStatus { get;set; }
    }
}
