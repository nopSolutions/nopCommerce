using Nop.Core.Configuration;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Security settings
    /// </summary>
    public partial class SecuritySettingsChangedEvent
    {
        public SecuritySettings SecuritySettings { get; set; }
        public string OldEncryptionPrivateKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SecuritySettings"></param>
        /// <param name="oldEncryptionPrivateKey"></param>
        public SecuritySettingsChangedEvent(SecuritySettings SecuritySettings, string oldEncryptionPrivateKey)
        {
            this.SecuritySettings = SecuritySettings;
            this.OldEncryptionPrivateKey = oldEncryptionPrivateKey;
        }
    }
}