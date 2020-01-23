using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PollAnswerBuilder : BaseEntityBuilder<PollAnswer>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PollAnswer.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(PollAnswer.PollId))
                    .AsInt32()
                    .ForeignKey<Poll>();
        }

        #endregion
    }
}