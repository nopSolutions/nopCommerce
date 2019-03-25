using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News
{
    public partial class HomepageNewsItemsModel : BaseNopModel, ICloneable
    {
        public HomepageNewsItemsModel()
        {
            NewsItems = new List<NewsItemModel>();
        }

        public int WorkingLanguageId { get; set; }
        public IList<NewsItemModel> NewsItems { get; set; }

        public object Clone()
        {
            //we use a shallow copy (deep clone is not required here)
            return MemberwiseClone();
        }
    }
}