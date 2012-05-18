using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Polls;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Web.Framework.Controllers;
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
                AlreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id),
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
                return Json(new
                {
                    error = "No poll answer found with the specified id",
                });

            var poll = pollAnswer.Poll;
            if (!poll.Published)
                return Json(new
                {
                    error = "Poll is not available",
                });

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Json(new
                {
                    error = _localizationService.GetResource("Polls.OnlyRegisteredUsersVote"),
                });

            bool alreadyVoted = _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id);
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
            }

            return Json(new
            {
                html = this.RenderPartialViewToString("_Poll", PreparePollModel(poll)),
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
