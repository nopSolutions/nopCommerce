using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PollBuilder : BaseEntityBuilder<Poll>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Poll.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(Poll.LanguageId))
                    .AsInt32()
                    .ForeignKey<Language>();
        }

        #endregion
    }
}