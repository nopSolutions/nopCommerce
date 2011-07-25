using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Polls;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Web.Models.Polls;

namespace Nop.Web.Controllers
{
    public class PollController : BaseNopController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;
        private readonly IWebHelper _webHelper;

        public PollController(ILocalizationService localizationService,
            IWorkContext workContext, IPollService pollService,
            IWebHelper webHelper)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._pollService = pollService;
            this._webHelper = webHelper;
        }


        protected PollModel PreparePollModel(Poll poll)
        {
            var model = new PollModel()
            {
                Id = poll.Id,
                AlreadyVoted = poll.AlreadyVoted(_workContext.CurrentCustomer),
                Name = poll.Name
            };
            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder);
            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;
            foreach (var pa in answers)
            {
                model.Answers.Add(new PollAnswerModel()
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0,
                });
            }

            return model;
        }

        [ChildActionOnly]
        public ActionResult PollBlock(string systemKeyword)
        {
            Poll poll = _pollService.GetPollBySystemKeyword(systemKeyword);
            if (poll == null ||
                !poll.Published ||
                (poll.StartDateUtc.HasValue && poll.StartDateUtc.Value > DateTime.UtcNow) ||
                (poll.EndDateUtc.HasValue && poll.EndDateUtc.Value < DateTime.UtcNow))
                return Content("");

            var model = PreparePollModel(poll);
            return PartialView(model);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Vote(int pollAnswerId)
        {
            var pollAnswer = _pollService.GetPollAnswerById(pollAnswerId);
            if (pollAnswer == null)
                throw new ArgumentException("No poll answer found with the specified id");

            string result = "Thank you!";
            bool success = false;


            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                result = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote");
            }
            else
            {
                var poll = pollAnswer.Poll;

                bool alreadyVoted = poll.AlreadyVoted(_workContext.CurrentCustomer);
                if (!alreadyVoted)
                {
                    //vote
                    pollAnswer.PollVotingRecords.Add(new PollVotingRecord()
                    {
                        PollAnswerId = pollAnswer.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        IsApproved = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    });
                    //update totals
                    pollAnswer.NumberOfVotes = pollAnswer.PollVotingRecords.Count;
                    _pollService.UpdatePoll(poll);

                    result = _localizationService.GetResource("Polls.Voted");
                    success = true;
                }
            }

            return Json(new
            {
                Success = success,
                Result = result,
            });
        }


        [ChildActionOnly]
        public ActionResult HomePagePolls()
        {
            var polls = _pollService.GetPolls(_workContext.WorkingLanguage.Id, true, 0, int.MaxValue);
            var model = polls.Select(x => PreparePollModel(x)).ToList();
            return PartialView(model);
        }

    }
}
