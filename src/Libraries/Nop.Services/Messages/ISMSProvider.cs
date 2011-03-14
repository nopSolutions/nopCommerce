using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Provides an interface for SMS providers
    /// </summary>
    public partial interface ISMSProvider : IPlugin
    {
        /// <summary>
        /// Gets the friendly name
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets the system name
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        bool SendSMS(string text);
    }
}
