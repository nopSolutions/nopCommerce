using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Models
{
    public class LanguageSelectorModel
    {
        public LanguageSelectorModel()
        {
            AvaibleLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvaibleLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }

        public bool IsAjaxRequest { get; set; }
    }
}