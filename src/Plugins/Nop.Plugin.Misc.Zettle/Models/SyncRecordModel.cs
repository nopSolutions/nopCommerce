using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a synchronization record model
    /// </summary>
    public record SyncRecordModel : BaseNopEntityModel
    {
        #region Properties

        public bool Active { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public bool PriceSyncEnabled { get; set; }

        public bool ImageSyncEnabled { get; set; }

        public bool InventoryTrackingEnabled { get; set; }

        public DateTime? UpdatedDate { get; set; }

        #endregion
    }
}