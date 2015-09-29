using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public partial class TopMenuModel : BaseNopModel
    {
        public TopMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
            Topics = new List<TopMenuTopicModel>();
        }

        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<TopMenuTopicModel> Topics { get; set; }

        public bool BlogEnabled { get; set; }
        public bool NewProductsEnabled { get; set; }
        public bool ForumEnabled { get; set; }

        #region Nested classes

        public class TopMenuTopicModel : BaseNopEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }
        }

        #endregion
    }
}