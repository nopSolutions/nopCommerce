<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Security settings
    /// </summary>
    public partial class SecuritySettings : ISettings
    {
        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a list of admin area allowed IP addresses
        /// </summary>
        public List<string> AdminAreaAllowedIpAddresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether honeypot is enabled on the registration page
        /// </summary>
        public bool HoneypotEnabled { get; set; }

        /// <summary>
        /// Gets or sets a honeypot input name
        /// </summary>
        public string HoneypotInputName { get; set; }        

        /// <summary>
        /// Gets or sets a value indicating whether to allow non-ASCII characters in headers
        /// </summary>
        public bool AllowNonAsciiCharactersInHeaders { get; set; }
    }
=======
﻿using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Security settings
    /// </summary>
    public partial class SecuritySettings : ISettings
    {
        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a list of admin area allowed IP addresses
        /// </summary>
        public List<string> AdminAreaAllowedIpAddresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether honeypot is enabled on the registration page
        /// </summary>
        public bool HoneypotEnabled { get; set; }

        /// <summary>
        /// Gets or sets a honeypot input name
        /// </summary>
        public string HoneypotInputName { get; set; }        

        /// <summary>
        /// Gets or sets a value indicating whether to allow non-ASCII characters in headers
        /// </summary>
        public bool AllowNonAsciiCharactersInHeaders { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}