using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.SendinBlue.Models
{
    /// <summary>
    /// Represents SMS model
    /// </summary>
    public class SmsModel : BaseNopEntityModel
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

        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.PhoneType")]
        public int PhoneTypeId { get; set; }

        public IList<SelectListItem> AvailablePhoneTypes { get; set; }

        public string PhoneType { get; set; }

        [NopResourceDisplayName("Plugins.Misc.SendinBlue.SMSText")]
        public string Text { get; set; }

        #endregion
    }
}