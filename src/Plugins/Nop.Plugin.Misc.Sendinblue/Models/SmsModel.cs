using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents SMS model
    /// </summary>
    public record SmsModel : BaseNopEntityModel
    {
        #region Ctor

        public SmsModel()
        {
            AvailableMessages = new List<SelectListItem>();
            AvailablePhoneTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        public int MessageId { get; set; }

        public IList<SelectListItem> AvailableMessages { get; set; }

        public string DefaultSelectedMessageId { get; set; }

        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Sendinblue.PhoneType")]
        public int PhoneTypeId { get; set; }

        public IList<SelectListItem> AvailablePhoneTypes { get; set; }

        public string DefaultSelectedPhoneTypeId { get; set; }

        public string PhoneType { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Sendinblue.SMSText")]
        public string Text { get; set; }

        #endregion
    }
}