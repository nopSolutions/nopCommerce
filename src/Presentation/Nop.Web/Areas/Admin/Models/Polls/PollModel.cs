using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Polls;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Polls
{
    [Validator(typeof(PollValidator))]
    public partial class PollModel : BaseNopEntityModel
    {
        public PollModel()
        {
            this.AvailableLanguages = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
        public int LanguageId { get; set; }
        public IList<SelectListItem> AvailableLanguages { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Language")]
        public string LanguageName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.SystemKeyword")]
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
        [UIHint("DateTimeNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Polls.Fields.EndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDate { get; set; }
    }
}