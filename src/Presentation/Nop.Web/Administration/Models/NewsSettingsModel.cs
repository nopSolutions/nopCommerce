using System.Web.Mvc;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework;

namespace Nop.Admin.Models
{
    public class NewsSettingsModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.News.Settings.Fields.Enabled")]
        public bool Enabled { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.Settings.Fields.AllowNotRegisteredUsersToLeaveComments")]
        public bool AllowNotRegisteredUsersToLeaveComments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.Settings.Fields.NotifyAboutNewNewsComments")]
        public bool NotifyAboutNewNewsComments { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.Settings.Fields.ShowNewsOnMainPage")]
        public bool ShowNewsOnMainPage { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.Settings.Fields.MainPageNewsCount")]
        public int MainPageNewsCount { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.News.Settings.Fields.NewsArchivePageSize")]
        public int NewsArchivePageSize { get; set; }
    }
}