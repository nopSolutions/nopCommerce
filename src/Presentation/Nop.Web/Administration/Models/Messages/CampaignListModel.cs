using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Messages
{
    public class CampaignListModel : BaseNopModel
    {
        public CampaignListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Promotions.Campaigns.List.Stores")]
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}