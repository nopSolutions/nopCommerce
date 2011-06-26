using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Polls;

namespace Nop.Services.Polls
{
    public static class PollExtentions
    {
        /// <summary>
        /// Gets a value indicating whether customer already vited for this poll
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="customer">Customer</param>
        /// <returns>Result</returns>
        public static bool AlreadyVoted(this Poll poll,
            Customer customer)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            if (customer == null)
                return false;

            var result = (from pa in poll.PollAnswers
                         from pvr in pa.PollVotingRecords
                         where pvr.CustomerId == customer.Id
                         select pvr).FirstOrDefault() != null;
            return result;
        }
    }
}
