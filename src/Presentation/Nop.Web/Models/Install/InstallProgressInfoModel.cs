using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Install;

public partial record InstallProgressInfoModel : BaseNopModel
{
    #region Properties

    public bool IsActive { get; set; }

    public string ProgressMessage { get; set; }

    #endregion
}
