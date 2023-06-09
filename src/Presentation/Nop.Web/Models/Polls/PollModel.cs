using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Polls
{
    public partial record PollModel : BaseNopEntityModel
    {
        public PollModel()
        {
            Answers = new List<PollAnswerModel>();
        }

        public string Name { get; set; }

        public bool AlreadyVoted { get; set; }

        public int TotalVotes { get; set; }

        public IList<PollAnswerModel> Answers { get; set; }
    }

    public partial record PollAnswerModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public int NumberOfVotes { get; set; }

        public double PercentOfTotalVotes { get; set; }
    }
}