using Nop.Core.Domain.Forums;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the forum model factory implementation
    /// </summary>
    public partial class ForumModelFactory : IForumModelFactory
    {
        #region Fields

        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly IForumService _forumService;

        #endregion

        #region Ctor

        public ForumModelFactory(IDateTimeHelper dateTimeHelper, IForumService forumService)
        {
            _dateTimeHelper = dateTimeHelper;
            _forumService = forumService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare forum search model
        /// </summary>
        /// <param name="searchModel">Forum search model</param>
        /// <returns>Forum search model</returns>
        protected virtual ForumSearchModel PrepareForumSearchModel(ForumSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare forum group search model
        /// </summary>
        /// <param name="searchModel">Forum group search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group search model
        /// </returns>
        public virtual Task<ForumGroupSearchModel> PrepareForumGroupSearchModelAsync(ForumGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search model
            PrepareForumSearchModel(searchModel.ForumSearch);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged forum group list model
        /// </summary>
        /// <param name="searchModel">Forum group search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group list model
        /// </returns>
        public virtual async Task<ForumGroupListModel> PrepareForumGroupListModelAsync(ForumGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get forum groups
            var forumGroups = (await _forumService.GetAllForumGroupsAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = await new ForumGroupListModel().PrepareToGridAsync(searchModel, forumGroups, () =>
            {
                return forumGroups.SelectAwait(async forumGroup =>
                {
                    //fill in model values from the entity
                    var forumGroupModel = forumGroup.ToModel<ForumGroupModel>();

                    //convert dates to the user time
                    forumGroupModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(forumGroup.CreatedOnUtc, DateTimeKind.Utc);

                    return forumGroupModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare forum group model
        /// </summary>
        /// <param name="model">Forum group model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum group model
        /// </returns>
        public virtual Task<ForumGroupModel> PrepareForumGroupModelAsync(ForumGroupModel model, ForumGroup forumGroup, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (forumGroup != null)
                model ??= forumGroup.ToModel<ForumGroupModel>();

            //set default values for the new model
            if (forumGroup == null)
                model.DisplayOrder = 1;

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare paged forum list model
        /// </summary>
        /// <param name="searchModel">Forum search model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum list model
        /// </returns>
        public virtual async Task<ForumListModel> PrepareForumListModelAsync(ForumSearchModel searchModel, ForumGroup forumGroup)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (forumGroup == null)
                throw new ArgumentNullException(nameof(forumGroup));

            //get forums
            var forums = (await _forumService.GetAllForumsByGroupIdAsync(forumGroup.Id)).ToPagedList(searchModel);

            //prepare list model
            var model = await new ForumListModel().PrepareToGridAsync(searchModel, forums, () =>
            {
                return forums.SelectAwait(async forum =>
                {
                    //fill in model values from the entity
                    var forumModel = forum.ToModel<ForumModel>();

                    //convert dates to the user time
                    forumModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(forum.CreatedOnUtc, DateTimeKind.Utc);

                    return forumModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare forum model
        /// </summary>
        /// <param name="model">Forum model</param>
        /// <param name="forum">Forum</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the forum model
        /// </returns>
        public virtual async Task<ForumModel> PrepareForumModelAsync(ForumModel model, Forum forum, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (forum != null)
                model ??= forum.ToModel<ForumModel>();

            //set default values for the new model
            if (forum == null)
                model.DisplayOrder = 1;

            //prepare available forum groups
            foreach (var forumGroup in await _forumService.GetAllForumGroupsAsync())
            {
                model.ForumGroups.Add(forumGroup.ToModel<ForumGroupModel>());
            }

            return model;
        }

        #endregion
    }
}