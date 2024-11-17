using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Web.Models.Customer;

public partial record CustomerInfoModel
{
    public int CustomerProfileTypeId { get; set; }
    public bool IsPaidCustomer { get; set; }
    public string HelpText { get; set; }
    public string LinkedInUrl { get; set; }
    public int Experaince { get; set; }
    public string LastActivityDateUtc { get; set; }
    public string JoinedDate { get; set; }

    public string CustomerAvatarUrl { get; set; }
    public string Country { get; set; }
}
