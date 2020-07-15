using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Weixin;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// WLocation service interface
    /// </summary>
    public partial interface IWLocationService
    {
        void InsertLocation(WLocation wlocation);

        void DeleteLocation(WLocation wlocation);

        void DeleteLocations(IList<WLocation> wlocations);

        void UpdateLocation(WLocation wlocation);

        WLocation GetLocationById(int id);

        WLocation GetLocationByUserId(int userId);

        IPagedList<WLocation> GetLocations(int pageIndex = 0, int pageSize = int.MaxValue);

    }
}