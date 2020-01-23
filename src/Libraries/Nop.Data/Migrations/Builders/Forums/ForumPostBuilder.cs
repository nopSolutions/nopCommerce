using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ForumPostBuilder : BaseEntityBuilder<ForumPost>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ForumPost.Text)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ForumPost.IPAddress)).AsString(100).Nullable()
                .WithColumn(nameof(ForumPost.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                    .OnDelete(Rule.None)
                .WithColumn(nameof(ForumPost.TopicId))
                    .AsInt32()
                    .ForeignKey<ForumTopic>();
        }

        #endregion
    }
}