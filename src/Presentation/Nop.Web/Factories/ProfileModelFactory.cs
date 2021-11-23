using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Extensions;
using Nop.Web.Models.Common;
using Nop.Web.Models.Profile;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the profile model factory
    /// </summary>
    public partial class ProfileModelFactory : IProfileModelFactory
    {
        #region Fields

        protected CustomerSettings CustomerSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IForumService ForumService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPictureService PictureService { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }

        #endregion

        #region Ctor

        public ProfileModelFactory(CustomerSettings customerSettings,
            ForumSettings forumSettings,
            ICountryService countryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            CustomerSettings = customerSettings;
            ForumSettings = forumSettings;
            CountryService = countryService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            ForumService = forumService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            PictureService = pictureService;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the profile index model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="page">Number of posts page; pass null to disable paging</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the profile index model
        /// </returns>
        public virtual async Task<ProfileIndexModel> PrepareProfileIndexModelAsync(Customer customer, int? page)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var pagingPosts = false;
            var postsPage = 0;

            if (page.HasValue)
            {
                postsPage = page.Value;
                pagingPosts = true;
            }

            var name = await CustomerService.FormatUsernameAsync(customer);
            var title = string.Format(await LocalizationService.GetResourceAsync("Profile.ProfileOf"), name);

            var model = new ProfileIndexModel
            {
                ProfileTitle = title,
                PostsPage = postsPage,
                PagingPosts = pagingPosts,
                CustomerProfileId = customer.Id,
                ForumsEnabled = ForumSettings.ForumsEnabled
            };
            return model;
        }

        /// <summary>
        /// Prepare the profile info model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the profile info model
        /// </returns>
        public virtual async Task<ProfileInfoModel> PrepareProfileInfoModelAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //avatar
            var avatarUrl = "";
            if (CustomerSettings.AllowCustomersToUploadAvatars)
            {
                avatarUrl = await PictureService.GetPictureUrlAsync(
                 await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                 MediaSettings.AvatarPictureSize,
                 CustomerSettings.DefaultAvatarEnabled,
                 defaultPictureType: PictureType.Avatar);
            }

            //location
            var locationEnabled = false;
            var location = string.Empty;
            if (CustomerSettings.ShowCustomersLocation)
            {
                locationEnabled = true;

                var countryId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute);
                var country = await CountryService.GetCountryByIdAsync(countryId);
                if (country != null)
                {
                    location = await LocalizationService.GetLocalizedAsync(country, x => x.Name);
                }
                else
                {
                    locationEnabled = false;
                }
            }

            //private message
            var pmEnabled = ForumSettings.AllowPrivateMessages && !await CustomerService.IsGuestAsync(customer);

            //total forum posts
            var totalPostsEnabled = false;
            var totalPosts = 0;
            if (ForumSettings.ForumsEnabled && ForumSettings.ShowCustomersPostCount)
            {
                totalPostsEnabled = true;
                totalPosts = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.ForumPostCountAttribute);
            }

            //registration date
            var joinDateEnabled = false;
            var joinDate = string.Empty;

            if (CustomerSettings.ShowCustomersJoinDate)
            {
                joinDateEnabled = true;
                joinDate = (await DateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
            }

            //birth date
            var dateOfBirthEnabled = false;
            var dateOfBirth = string.Empty;
            if (CustomerSettings.DateOfBirthEnabled)
            {
                var dob = await GenericAttributeService.GetAttributeAsync<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
                if (dob.HasValue)
                {
                    dateOfBirthEnabled = true;
                    dateOfBirth = dob.Value.ToString("D");
                }
            }

            var model = new ProfileInfoModel
            {
                CustomerProfileId = customer.Id,
                AvatarUrl = avatarUrl,
                LocationEnabled = locationEnabled,
                Location = location,
                PMEnabled = pmEnabled,
                TotalPostsEnabled = totalPostsEnabled,
                TotalPosts = totalPosts.ToString(),
                JoinDateEnabled = joinDateEnabled,
                JoinDate = joinDate,
                DateOfBirthEnabled = dateOfBirthEnabled,
                DateOfBirth = dateOfBirth,
            };

            return model;
        }

        /// <summary>
        /// Prepare the profile posts model
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="page">Number of posts page</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the profile posts model
        /// </returns>
        public virtual async Task<ProfilePostsModel> PrepareProfilePostsModelAsync(Customer customer, int page)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = ForumSettings.LatestCustomerPostsPageSize;

            var list = await ForumService.GetAllPostsAsync(0, customer.Id, string.Empty, false, page, pageSize);

            var latestPosts = new List<PostsModel>();

            foreach (var forumPost in list)
            {
                string posted;
                if (ForumSettings.RelativeDateTimeFormattingEnabled)
                {
                    var languageCode = (await WorkContext.GetWorkingLanguageAsync()).LanguageCulture;
                    var postedAgo = forumPost.CreatedOnUtc.RelativeFormat(languageCode);
                    posted = string.Format(await LocalizationService.GetResourceAsync("Common.RelativeDateTime.Past"), postedAgo);
                }
                else
                {
                    posted = (await DateTimeHelper.ConvertToUserTimeAsync(forumPost.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
                }

                var topic = await ForumService.GetTopicByIdAsync(forumPost.TopicId);

                latestPosts.Add(new PostsModel
                {
                    ForumTopicId = topic.Id,
                    ForumTopicTitle = topic.Subject,
                    ForumTopicSlug = await ForumService.GetTopicSeNameAsync(topic),
                    ForumPostText = ForumService.FormatPostText(forumPost),
                    Posted = posted
                });
            }

            var pagerModel = new PagerModel(LocalizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProfilePaged",
                UseRouteLinks = true,
                RouteValues = new RouteValues { pageNumber = page, id = customer.Id }
            };

            var model = new ProfilePostsModel
            {
                PagerModel = pagerModel,
                Posts = latestPosts,
            };

            return model;
        }

        #endregion
    }
}