using System;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    public static class SmsExtentions
    {
        public static bool IsSmsProviderActive(this ISmsProvider smsProvider,
            SmsSettings smsSettings)
        {
            if (smsProvider == null)
                throw new ArgumentNullException("smsProvider");

            if (smsSettings == null)
                throw new ArgumentNullException("smsSettings");

            if (smsSettings.ActiveSmsProviderSystemNames == null)
                return false;
            foreach (string activeMethodSystemName in smsSettings.ActiveSmsProviderSystemNames)
                if (smsProvider.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
