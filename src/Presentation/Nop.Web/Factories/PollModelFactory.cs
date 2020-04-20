using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Polls;
using Nop.Services.Caching;
using Nop.Services.Polls;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Polls;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the poll model factory
    /// </summary>
    public partial class PollModelFactory : IPollModelFactory
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IPollService _pollService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PollModelFactory(ICacheKeyService cacheKeyService,
            IPollService pollService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _cacheKeyService = cacheKeyService;
            _pollService = pollService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the poll model
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="setAlreadyVotedProperty">Whether to load a value indicating that customer already voted for this poll</param>
        /// <returns>Poll model</returns>
        public virtual PollModel PreparePollModel(Poll poll, bool setAlreadyVotedProperty)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && _pollService.AlreadyVoted(poll.Id, _workContext.CurrentCustomer.Id),
                Name = poll.Name
            };
            var answers = _pollService.GetPollAnswerByPoll(poll.Id);
            
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

        /// <summary>
        /// Get the poll model by poll system keyword
        /// </summary>
        /// <param name="systemKeyword">Poll system keyword</param>
        /// <returns>Poll model</returns>
        public virtual PollModel PreparePollModelBySystemName(string systemKeyword)
        {
            if (string.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.PollBySystemNameModelKey, 
                systemKeyword, _workContext.WorkingLanguage, _storeContext.CurrentStore);

            var cachedModel = _staticCacheManager.Get(cacheKey, () =>
            {
                var poll = _pollService
                    .GetPolls(_storeContext.CurrentStore.Id, _workContext.WorkingLanguage.Id, systemKeyword: systemKeyword)
                    .FirstOrDefault();

                //we do not cache nulls. that's why let's return an empty record (ID = 0)
                if (poll == null)
                    return new PollModel { Id = 0 };

                return PreparePollModel(poll, false);
            });

            if ((cachedModel?.Id ?? 0) == 0)
                return null;

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (PollModel)cachedModel.Clone();
            model.AlreadyVoted = _pollService.AlreadyVoted(model.Id, _workContext.CurrentCustomer.Id);

            return model;
        }

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        public virtual List<PollModel> PrepareHomepagePollModels()
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepagePollsModelKey, 
                _workContext.WorkingLanguage, _storeContext.CurrentStore);

            var cachedPolls = _staticCacheManager.Get(cacheKey, () =>
                _pollService.GetPolls(_storeContext.CurrentStore.Id, _workContext.WorkingLanguage.Id, loadShownOnHomepageOnly: true)
                    .Select(poll => PreparePollModel(poll, false)).ToList());

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var poll in cachedPolls)
            {
                var pollModel = (PollModel)poll.Clone();
                pollModel.AlreadyVoted = _pollService.AlreadyVoted(pollModel.Id, _workContext.CurrentCustomer.Id);
                model.Add(pollModel);
            }
            
            return model;
        }

        #endregion
    }
}
