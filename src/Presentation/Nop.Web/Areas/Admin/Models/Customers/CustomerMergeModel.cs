using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

public partial record CustomerMergeModel : CustomerSearchModel
{
    public CustomerModel FromCustomer { get; set; }
    public int CurrentCustomerId { get; set; }

    public MergeModel Merge { get; set; } = new();

    public CustomerMergeModel() { }

    public CustomerMergeModel(CustomerModel customer)
    {
        FromCustomer = customer;
        CurrentCustomerId = customer.Id;
        Merge = new()
        {
            FromId = customer.Id,
            DeleteMergedCustomer = true,
            FromIsSource = true
        };
    }

    public record MergeModel
    {
        public int FromId { get; set; } 

        public int ToId { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerMerge.Fields.DeleteMergedCustomer")]
        public bool DeleteMergedCustomer { get; set; }

        public bool FromIsSource { get; set; }
    }
}