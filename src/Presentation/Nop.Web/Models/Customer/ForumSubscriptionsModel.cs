using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public class ForumSubscriptionModel : BaseNopEntityModel
    {
        public int ForumId { get; set; }
        public int ForumTopicId { get; set; }
        public bool TopicSubscription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}
