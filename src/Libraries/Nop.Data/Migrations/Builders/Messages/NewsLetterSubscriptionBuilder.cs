using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class NewsLetterSubscriptionBuilder : BaseEntityBuilder<NewsLetterSubscription>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(NewsLetterSubscription.Email)).AsString(255).NotNullable();
        }

        #endregion
    }
}