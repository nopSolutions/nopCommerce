using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.AbcContactUs.Models
{
    public record ContactUsModel : BaseNopModel
    {
        public ContactUsModel()
        {
            ReasonsForContact = new List<SelectListItem>();
            Stores = new List<SelectListItem>();
            ErrorMessages = new List<string>();
        }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Reason { get; set; }
        public IList<SelectListItem> ReasonsForContact { get; set; }
        public string SelectedStore { get; set; }
        public IList<SelectListItem> Stores { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Comments { get; set; }

        public List<string> ErrorMessages { get; set; }

        public bool DisplayCaptcha { get; set; }
        public string GRecaptchaResponse { get; set; }
    }
}