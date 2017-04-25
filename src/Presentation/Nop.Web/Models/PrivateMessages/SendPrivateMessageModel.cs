#if NET451
using System.Web.Mvc;
#endif
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Validators.PrivateMessages;

namespace Nop.Web.Models.PrivateMessages
{
    [Validator(typeof(SendPrivateMessageValidator))]
    public partial class SendPrivateMessageModel : BaseNopEntityModel
    {
        public int ToCustomerId { get; set; }
        public string CustomerToName { get; set; }
        public bool AllowViewingToProfile { get; set; }

        public int ReplyToMessageId { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        public string Subject { get; set; }

        	
#if NET451
		[AllowHtml]
#endif
        public string Message { get; set; }
    }
}