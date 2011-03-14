using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Web.MVC.Areas.Admin.Validators;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    [Validator(typeof(LanguageValidator))]
    public class LanguageModel : BaseNopEntityModel
    {
        public LanguageModel()
        {
        }

        public LanguageModel(Language language)
            :this()
        {
            Id = language.Id;
            Name = language.Name;
            LanguageCulture = language.LanguageCulture;
            FlagImageFileName = language.FlagImageFileName;
            Published = language.Published;
            DisplayOrder = language.DisplayOrder;
        }

        [NopResourceDisplayName("Admin.Configuration.Location.Languages.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Languages.Fields.LanguageCulture")]
        public string LanguageCulture { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Languages.Fields.FlagImageFileName")]
        public string FlagImageFileName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Languages.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Location.Languages.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}