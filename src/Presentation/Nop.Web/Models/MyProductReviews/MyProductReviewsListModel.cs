using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Models.MyProductReviews
{
    public partial class MyProductReviewsListModel : BaseNopModel
    {
        public IList<MyProductReviewsModel> ProductReviews { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}
