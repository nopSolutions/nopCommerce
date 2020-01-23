using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ForumSubscriptionBuilder : BaseEntityBuilder<ForumSubscription>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ForumSubscription.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>();
        }

        #endregion
    }
}