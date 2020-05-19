using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WQrCodeLimit Service interface
    /// </summary>
    public partial interface IWQrCodeLimitService
    {
        void InsertWQrCodeLimit(WQrCodeLimit wQrCodeLimit);

        void UpdateWQrCodeLimit(WQrCodeLimit wQrCodeLimit);

        void UpdateWQrCodeLimits(IList<WQrCodeLimit> wQrCodeLimits);

        WQrCodeLimit GetWQrCodeLimitById(int id);

        WQrCodeLimit GetWQrCodeLimitByQrCodeId(int qrCodeId);

        List<WQrCodeLimit> GetWUsersByIds(int[] wQrCodeLimitIds);

        IPagedList<WQrCodeLimit> GetWQrCodeLimits(int? wConfigId = null, int? wQrCodeCategoryId = null, int? wQrCodeChannelId = null, bool? fixedUse = null, bool? hasCreated = null, int pageIndex = 0, int pageSize = int.MaxValue);

    }
}