using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Companies;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Companies
{
    public partial class CompanyCustomerBuilder : NopEntityBuilder<CompanyCustomer>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CompanyCustomer.CompanyId)).AsInt32().ForeignKey<Company>()
                .WithColumn(nameof(CompanyCustomer.CustomerId)).AsInt32().ForeignKey<Customer>();
        }
    }
}