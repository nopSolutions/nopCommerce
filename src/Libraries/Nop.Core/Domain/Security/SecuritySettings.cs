
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    public class SecuritySettings : ISettings
    {
        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        //TODO Add 'Admin area allowed IP' option
    }
}