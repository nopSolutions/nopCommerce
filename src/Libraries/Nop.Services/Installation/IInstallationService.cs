
using Nop.Core.Domain.Security;

namespace Nop.Services.Installation
{
    public partial interface IInstallationService
    {
        void InstallData(string defaultUserEmail, string defaultUserPassword, HashFormat hashFormat, EncryptionFormat encryptionFormat, bool installSampleData = true);
    }
}
