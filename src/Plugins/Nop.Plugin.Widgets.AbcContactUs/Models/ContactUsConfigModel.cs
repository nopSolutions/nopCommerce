using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.AbcContactUs.Models
{
    public class ContactUsConfigModel
    {
        [Required]
        [NopResourceDisplayName(ContactUsLocaleKeys.ContactUsEmail)]
        public string ContactUsEmail { get; set; }

        [NopResourceDisplayName(ContactUsLocaleKeys.AdditionalEmails)]
        public string AdditionalEmails { get; set; }

        [NopResourceDisplayName(ContactUsLocaleKeys.IsEmailSubmissionSkipped)]
        public bool IsEmailSubmissionSkipped { get; set; }
    }
}
