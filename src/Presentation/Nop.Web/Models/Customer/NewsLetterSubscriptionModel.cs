using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer;

public partial record NewsLetterSubscriptionModel : BaseNopModel
{
    public int TypeId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }

}
