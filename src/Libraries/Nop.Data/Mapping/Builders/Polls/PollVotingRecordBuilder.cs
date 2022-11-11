using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Polls
{
    /// <summary>
    /// Represents a poll voting record entity builder
    /// </summary>
    public partial class PollVotingRecordBuilder : NopEntityBuilder<PollVotingRecord>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PollVotingRecord.PollAnswerId)).AsInt32().ForeignKey<PollAnswer>()
                .WithColumn(nameof(PollVotingRecord.CustomerId)).AsInt32().ForeignKey<Customer>();
        }

        #endregion
    }
}