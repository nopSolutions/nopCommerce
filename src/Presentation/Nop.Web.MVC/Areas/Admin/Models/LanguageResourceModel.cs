using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Localization;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    public class LanguageResourceModel
    {
        public LanguageResourceModel()
        {
            
        }

        public LanguageResourceModel(LocaleStringResource resource)
        {
            Id = resource.Id;
            Name = resource.ResourceName;
            Value = resource.ResourceValue;
            LanguageId = resource.LanguageId;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int LanguageId { get; set; }
    }
}