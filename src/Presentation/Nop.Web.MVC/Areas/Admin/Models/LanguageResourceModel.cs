using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Nop.Core.Domain.Localization;
using Nop.Web.MVC.Areas.Admin.Validators;

namespace Nop.Web.MVC.Areas.Admin.Models
{
    [Validator(typeof(LanguageResourceValidator))]
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
            if (resource.Language != null)
            {
                LanguageName = resource.Language.Name;
            }
            LanguageId = resource.LanguageId;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string LanguageName { get; set; }
        public int LanguageId { get; set; }
    }
}