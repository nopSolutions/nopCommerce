using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Common;
using Nop.Web.Models.Profile;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public partial class ProfileController : BasePublicController
    {
        private readonly IForumService _forumService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly ForumSettings _forumSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly MediaSettings _mediaSettings;

        public ProfileController(IForumService forumService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICountryService countryService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService,
            ForumSettings forumSettings,
            CustomerSettings customerSettings,
            MediaSettings mediaSettings)
        {
            this._forumService = forumService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._countryService = countryService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._permissionService = permissionService;
            this._forumSettings = forumSettings;
            this._customerSettings = customerSettings;
            this._mediaSettings = mediaSettings;
        }

        public ActionResult Index(int? id, int? page)
        {
            if (!_customerSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("HomePage");
            }

            var customerId = 0;
            if (id.HasValue)
            {
                customerId = id.Value;
            }

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null || customer.IsGuest())
            {
                return RedirectToRoute("HomePage");
            }

            bool pagingPosts = false;
            int postsPage = 0;

            if (page.HasValue)
            {
                postsPage = page.Value;
                pagingPosts = true;
            }

            var name = customer.FormatUserName();
            var title = string.Format(_localizationService.GetResource("Profile.ProfileOf"), name);

            var model = new ProfileIndexModel
            {
                ProfileTitle = title,
                PostsPage = postsPage,
                PagingPosts = pagingPosts,
                CustomerProfileId = customer.Id,
                ForumsEnabled = _forumSettings.ForumsEnabled
            };


            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = "Admin" }));


            return View(model);
        }

        //profile info tab
        [ChildActionOnly]
        public ActionResult Info(int customerProfileId)
        {
            var customer = _customerService.GetCustomerById(customerProfileId);
            if (customer == null)
            {
                return RedirectToRoute("HomePage");
            }

            //avatar
            var avatarUrl = "";
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                avatarUrl =_pictureService.GetPictureUrl(
                 customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                 _mediaSettings.AvatarPictureSize,
                 _customerSettings.DefaultAvatarEnabled,
                 defaultPictureType: PictureType.Avatar);
            }

            //location
            bool locationEnabled = false;
            string location = string.Empty;
            if (_customerSettings.ShowCustomersLocation)
            {
                locationEnabled = true;

                var countryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                var country = _countryService.GetCountryById(countryId);
                if (country != null)
                {
                    location = country.GetLocalized(x => x.Name);
                }
                else
                {
                    locationEnabled = false;
                }
            }

            //private message
            bool pmEnabled = _forumSettings.AllowPrivateMessages && !customer.IsGuest();

            //total forum posts
            bool totalPostsEnabled = false;
            int totalPosts = 0;
            if (_forumSettings.ForumsEnabled && _forumSettings.ShowCustomersPostCount)
            {
                totalPostsEnabled = true;
                totalPosts = customer.GetAttribute<int>(SystemCustomerAttributeNames.ForumPostCount);
            }

            //registration date
            bool joinDateEnabled = false;
            string joinDate = string.Empty;

            if (_customerSettings.ShowCustomersJoinDate)
            {
                joinDateEnabled = true;
                joinDate = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
            }

            //birth date
            bool dateOfBirthEnabled = false;
            string dateOfBirth = string.Empty;
            if (_customerSettings.DateOfBirthEnabled)
            {
                var dob = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
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

            return PartialView(model);
        }

        //latest posts tab
        [ChildActionOnly]
        public ActionResult Posts(int customerProfileId, int page)
        {
            var customer = _customerService.GetCustomerById(customerProfileId);
            if (customer == null)
            {
                return RedirectToRoute("HomePage");
            }

            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.LatestCustomerPostsPageSize;

            var list = _forumService.GetAllPosts(0, customer.Id, string.Empty, false, page, pageSize);

            var latestPosts = new List<PostsModel>();

            foreach (var forumPost in list)
            {
                var posted = string.Empty;
                if (_forumSettings.RelativeDateTimeFormattingEnabled)
                {
                    posted = forumPost.CreatedOnUtc.RelativeFormat(true, "f");
                }
                else
                {
                    posted = _dateTimeHelper.ConvertToUserTime(forumPost.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
                }

                latestPosts.Add(new PostsModel
                {
                    ForumTopicId = forumPost.TopicId,
                    ForumTopicTitle = forumPost.ForumTopic.Subject,
                    ForumTopicSlug = forumPost.ForumTopic.GetSeName(),
                    ForumPostText = forumPost.FormatPostText(),
                    Posted = posted
                });
            }

            var pagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "CustomerProfilePaged",
                UseRouteLinks = true,
                RouteValues = new RouteValues { page = page, id = customerProfileId }
            };

            var model = new ProfilePostsModel
            {
                PagerModel = pagerModel,
                Posts = latestPosts,
            };

            return PartialView(model);
        }
    }
}
