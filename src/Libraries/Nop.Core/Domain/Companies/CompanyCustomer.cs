namespace Nop.Core.Domain.Companies
{
    public partial class CompanyCustomer : BaseEntity
    {
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
    }
}