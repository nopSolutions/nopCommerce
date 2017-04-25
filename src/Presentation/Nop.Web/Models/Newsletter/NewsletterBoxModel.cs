using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Newsletter
{
    public partial class NewsletterBoxModel : BaseNopModel
    {
        public string NewsletterEmail { get; set; }
        public bool AllowToUnsubscribe { get; set; }
    }
}