using System;
using System.Linq;
using Nop.Core.Domain.Forums;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the forum model factory implementation
    /// </summary>
    public partial class ForumModelFactory : IForumModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IForumService _forumService;

        #endregion

        #region Ctor

        public ForumModelFactory(IDateTimeHelper dateTimeHelper,
            IForumService forumService)
        {
            this._dateTimeHelper = dateTimeHelper;
            this._forumService = forumService;
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
        /// <returns>Forum group search model</returns>
        public virtual ForumGroupSearchModel PrepareForumGroupSearchModel(ForumGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare nested search model
            PrepareForumSearchModel(searchModel.ForumSearch);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged forum group list model
        /// </summary>
        /// <param name="searchModel">Forum group search model</param>
        /// <returns>Forum group list model</returns>
        public virtual ForumGroupListModel PrepareForumGroupListModel(ForumGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get forum groups
            var forumGroups = _forumService.GetAllForumGroups();

            //prepare list model
            var model = new ForumGroupListModel
            {
                Data = forumGroups.PaginationByRequestModel(searchModel).Select(forumGroup =>
                {
                    //fill in model values from the entity
                    var forumGroupModel = forumGroup.ToModel<ForumGroupModel>();

                    //convert dates to the user time
                    forumGroupModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(forumGroup.CreatedOnUtc, DateTimeKind.Utc);

                    return forumGroupModel;
                }),
                Total = forumGroups.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare forum group model
        /// </summary>
        /// <param name="model">Forum group model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Forum group model</returns>
        public virtual ForumGroupModel PrepareForumGroupModel(ForumGroupModel model, ForumGroup forumGroup, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (forumGroup != null)
                model = model ?? forumGroup.ToModel<ForumGroupModel>();

            //set default values for the new model
            if (forumGroup == null)
                model.DisplayOrder = 1;

            return model;
        }

        /// <summary>
        /// Prepare paged forum list model
        /// </summary>
        /// <param name="searchModel">Forum search model</param>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Forum list model</returns>
        public virtual ForumListModel PrepareForumListModel(ForumSearchModel searchModel, ForumGroup forumGroup)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (forumGroup == null)
                throw new ArgumentNullException(nameof(forumGroup));

            //get forums
            var forums = forumGroup.Forums;

            //prepare list model
            var model = new ForumListModel
            {
                Data = forums.PaginationByRequestModel(searchModel).Select(forum =>
                {
                    //fill in model values from the entity
                    var forumModel = forum.ToModel<ForumModel>();

                    //convert dates to the user time
                    forumModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(forum.CreatedOnUtc, DateTimeKind.Utc);

                    return forumModel;
                }),
                Total = forums.Count
            };

            return model;
        }

        /// <summary>
        /// Prepare forum model
        /// </summary>
        /// <param name="model">Forum model</param>
        /// <param name="forum">Forum</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Forum model</returns>
        public virtual ForumModel PrepareForumModel(ForumModel model, Forum forum, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (forum != null)
                model = model ?? forum.ToModel<ForumModel>();

            //set default values for the new model
            if (forum == null)
                model.DisplayOrder = 1;

            //prepare available forum groups
            foreach (var forumGroup in _forumService.GetAllForumGroups())
            {
                model.ForumGroups.Add(forumGroup.ToModel<ForumGroupModel>());
            }

            return model;
        }

        #endregion
    }
}