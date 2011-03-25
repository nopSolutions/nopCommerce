using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Models.Home;

namespace Nop.Web.Models
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