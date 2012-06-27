using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Polls;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Polls
{
    [Validator(typeof(PollValidator))]
    public partial class PollModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
        [AllowHtml]
        public string LanguageName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.SystemKeyword")]
        [AllowHtml]
        public string SystemKeyword { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.AllowGuestsToVote")]
        public bool AllowGuestsToVote { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

    }
}