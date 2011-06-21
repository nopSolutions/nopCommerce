using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.PrivateMessages;

namespace Nop.Web.Models.PrivateMessages
{
    [Validator(typeof(PrivateMessageValidator))]
    public class PrivateMessageModel : BaseNopEntityModel
    {
        public int ToCustomerId { get; set; }

        public int ReplyToMessageId { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Message { get; set; }

        public string customerToName { get; set; }

        public string customerFromName { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public bool IsRead { get; set; }

        public string PostError { get; set; }
    }
}