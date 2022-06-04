using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.PrivateMessages
{
    public partial record SendPrivateMessageModel
    {
        public int ProductId { get; set; }
        public string CustomerFromSeName { get; set; }
        public string CustomerToSeName { get; set; }
        public string SenderSubject { get; set; }
        public string SenderBodyText { get; set; }
        public string RecipientBodyText { get; set; }
        public bool IsSystemGenerated { get; set; }
        public int ParentMessageId { get; set; }

    }
}
