
using System.Collections.Generic;

namespace Nop.Services.Installation
{
    public partial interface IInstallationService
    {
        void InstallData(bool installSampleData = true);
    }
}
