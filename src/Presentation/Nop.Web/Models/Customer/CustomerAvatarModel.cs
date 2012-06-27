using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerAvatarModel : BaseNopModel
    {
        public string AvatarUrl { get; set; }
        public CustomerNavigationModel NavigationModel { get; set; }
    }
}