using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class PollVotingRecordBuilder : BaseEntityBuilder<PollVotingRecord>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PollVotingRecord.PollAnswerId))
                    .AsInt32()
                    .ForeignKey<PollAnswer>()
                .WithColumn(nameof(PollVotingRecord.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>();
        }

        #endregion
    }
}