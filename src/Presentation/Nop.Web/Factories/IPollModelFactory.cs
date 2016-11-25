using System.Collections.Generic;
using Nop.Core.Domain.Polls;
using Nop.Web.Models.Polls;

namespace Nop.Web.Factories
{
    public partial interface IPollModelFactory
    {
        PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty);

        PollModel PreparePollModelBySystemName(string systemKeyword);

        List<PollModel> PrepareHomePagePollModels();
    }
}
