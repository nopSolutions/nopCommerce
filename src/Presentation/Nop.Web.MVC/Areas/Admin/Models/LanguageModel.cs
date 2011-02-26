using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Localization;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    public class LanguageModel
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

        public int Id { get; set; }
        public string Name { get; set; }
        public string LanguageCulture { get; set; }
        public string FlagImageFileName { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
    }
}