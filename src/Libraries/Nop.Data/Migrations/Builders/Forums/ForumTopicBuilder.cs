using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ForumTopicBuilder : BaseEntityBuilder<ForumTopic>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ForumTopic.Subject)).AsString(450).NotNullable()
                .WithColumn(nameof(ForumTopic.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(ForumTopic.ForumId))
                    .AsInt32()
                    .ForeignKey<Forum>();
        }

        #endregion
    }
}