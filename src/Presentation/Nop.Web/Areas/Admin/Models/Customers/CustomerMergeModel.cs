using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

public partial record CustomerMergeModel : CustomerSearchModel
{
    public CustomerModel FromCustomer { get; set; }
    public int CurrentCustomerId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerMerge.Fields.DeleteMergedCustomer")]
    public bool DeleteMergedCustomer { get; set; } = true;
}