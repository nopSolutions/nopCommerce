using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public virtual async Task<PollModel> PreparePollModel(Poll poll, bool setAlreadyVotedProperty)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            var model = new PollModel
            {
                Id = poll.Id,
                AlreadyVoted = setAlreadyVotedProperty && await _pollService.AlreadyVoted(poll.Id, (await _workContext.GetCurrentCustomer()).Id),
                Name = poll.Name
            };
            var answers = await _pollService.GetPollAnswerByPoll(poll.Id);
            
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
        public virtual async Task<PollModel> PreparePollModelBySystemName(string systemKeyword)
        {
            if (string.IsNullOrWhiteSpace(systemKeyword))
                return null;

            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.PollBySystemNameModelKey, 
                systemKeyword, await _workContext.GetWorkingLanguage(), await _storeContext.GetCurrentStore());

            var cachedModel = await _staticCacheManager.Get(cacheKey, async () =>
            {
                var poll = (await _pollService
                    .GetPolls((await _storeContext.GetCurrentStore()).Id, (await _workContext.GetWorkingLanguage()).Id, systemKeyword: systemKeyword))
                    .FirstOrDefault();

                //we do not cache nulls. that's why let's return an empty record (ID = 0)
                if (poll == null)
                    return new PollModel { Id = 0 };

                return await PreparePollModel(poll, false);
            });

            if ((cachedModel?.Id ?? 0) == 0)
                return null;

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (PollModel)cachedModel.Clone();
            model.AlreadyVoted = await _pollService.AlreadyVoted(model.Id, (await _workContext.GetCurrentCustomer()).Id);

            return model;
        }

        /// <summary>
        /// Prepare the home page poll models
        /// </summary>
        /// <returns>List of the poll model</returns>
        public virtual async Task<List<PollModel>> PrepareHomepagePollModels()
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepagePollsModelKey, 
                await _workContext.GetWorkingLanguage(), await _storeContext.GetCurrentStore());

            var cachedPolls = await _staticCacheManager.Get(cacheKey, async () =>
                (await _pollService.GetPolls((await _storeContext.GetCurrentStore()).Id, (await _workContext.GetWorkingLanguage()).Id, loadShownOnHomepageOnly: true))
                    .Select(poll => PreparePollModel(poll, false).Result).ToList());

            //"AlreadyVoted" property of "PollModel" object depends on the current customer. Let's update it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = new List<PollModel>();
            foreach (var poll in cachedPolls)
            {
                var pollModel = (PollModel)poll.Clone();
                pollModel.AlreadyVoted = await _pollService.AlreadyVoted(pollModel.Id, (await _workContext.GetCurrentCustomer()).Id);
                model.Add(pollModel);
            }
            
            return model;
        }

        #endregion
    }
}
