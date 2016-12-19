using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Polls;
using Nop.Services.Polls;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Polls;

namespace Nop.Web.Factories
{
    public partial class PollModelFactory : IPollModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPollService _pollService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public PollModelFactory(IWorkContext workContext,
            IPollService pollService,
            ICacheManager cacheManager)
        {
            this._workContext = workContext;
            this._pollService = pollService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public virtual PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty)
        {
            if (poll == null)
                throw new ArgumentNullException("poll");

            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id),
                Name = poll.Name
            };
            var answers = poll.PollAnswers.OrderBy(x => x.DisplayOrder);
            foreach (var answer in answers)
                model.TotalVotes += answer.NumberOfVotes;
            foreach (var pa in answers)
            {
                model.Answers.Add(new PollAnswerModel
                {
                    Id = pa.Id,
                    Name = pa.Name,
                    NumberOfVotes = pa.NumberOfVotes,
                    PercentOfTotalVotes = model.TotalVotes > 0 ? ((Convert.ToDouble(pa.NumberOfVotes) / Convert.ToDouble(model.TotalVotes)) * Convert.ToDouble(100)) : 0,
                });
            }

            return model;
        }

        public virtual PollModel PreparePollModelBySystemName(string systemKeyword)
        {
            if (String.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var cacheKey = string.Format(ModelCacheEventConsumer.POLL_BY_SYSTEMNAME__MODEL_KEY, systemKeyword, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                Poll poll = _pollService.GetPolls(languageId: _workContext.WorkingLanguage.Id, systemKeyword: systemKeyword).FirstOrDefault();
                if (poll == null)
                    //we do not cache nulls. that's why let's return an empty record (ID = 0)
                    return new PollModel { Id = 0};

                return PreparePollModel(poll, false);
            });
            if (cachedModel == null || cachedModel.Id == 0)
                return null;

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (PollModel)cachedModel.Clone();
            model.AlreadyVoted = _pollService.AlreadyVoted(model.Id, _workContext.CurrentCustomer.Id);

            return model;
        }

        public virtual List<PollModel> PrepareHomePagePollModels()
        {
            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_POLLS_MODEL_KEY, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
                _pollService.GetPolls(_workContext.WorkingLanguage.Id, true)
                .Select(x => PreparePollModel(x, false))
                .ToList());
            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var p in cachedModel)
            {
                var pollModel = (PollModel) p.Clone();
                pollModel.AlreadyVoted = _pollService.AlreadyVoted(pollModel.Id, _workContext.CurrentCustomer.Id);
                model.Add(pollModel);
            }
            
            return model;
        }

        #endregion
    }
}
