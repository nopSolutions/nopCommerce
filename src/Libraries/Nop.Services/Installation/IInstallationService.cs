
namespace Nop.Services.Installation
{
    public partial interface IInstallationService
    {
        void InstallData(string defaultUserEmail, string defaultUserPassword, bool installSampleData = true);
    }
}
