using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Newsletter
{
    public partial record SubscriptionActivationModel : BaseNopModel
    {
        public string Result { get; set; }
    }
}