using System;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Forums
{
    public partial class PrivateMessage
    {
        public string SenderSubject { get; set; }
        public string SenderBodyText { get; set; }
        public string RecipientBodyText { get; set; }
        public bool IsSystemGenerated { get; set; }
        public int ParentMessageId { get; set; }
    }
}