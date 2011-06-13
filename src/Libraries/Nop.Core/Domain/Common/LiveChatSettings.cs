
using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Common
{
    public class LiveChatSettings : ISettings
    {
        /// <summary>
        /// Gets or sets an system name of active live chat provider
        /// </summary>
        public List<string> ActiveLiveChatProviderSystemName { get; set; }
    }
}