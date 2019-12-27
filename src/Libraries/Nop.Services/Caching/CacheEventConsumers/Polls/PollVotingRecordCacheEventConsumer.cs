using Nop.Core.Domain.Polls;

namespace Nop.Services.Caching.CacheEventConsumers.Polls
{
    /// <summary>
    /// Represents a poll voting record
    /// </summary>
    public partial class PollVotingRecordCacheEventConsumer : CacheEventConsumer<PollVotingRecord>
    {
    }
}