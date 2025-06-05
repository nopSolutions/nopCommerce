namespace Nop.Web.Areas.Admin.Models.Customers;

public partial record CustomerMergeModel : CustomerSearchModel
{
    public CustomerModel FromCustomer { get; set; }
    public int CurrentCustomerId { get; set; }
}