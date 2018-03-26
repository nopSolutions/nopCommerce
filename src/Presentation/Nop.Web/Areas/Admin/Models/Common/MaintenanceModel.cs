using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common
{
    public partial class MaintenanceModel : BaseNopModel
    {
        public MaintenanceModel()
        {
            DeleteGuests = new DeleteGuestsModel();
            DeleteAbandonedCarts = new DeleteAbandonedCartsModel();
            DeleteExportedFiles = new DeleteExportedFilesModel();
            BackupFileSearchModel = new BackupFileSearchModel();
        }

        public DeleteGuestsModel DeleteGuests { get; set; }

        public DeleteAbandonedCartsModel DeleteAbandonedCarts { get; set; }

        public DeleteExportedFilesModel DeleteExportedFiles { get; set; }

        public BackupFileSearchModel BackupFileSearchModel { get; set; }

        #region Nested classes

        public partial class DeleteGuestsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.System.Maintenance.DeleteGuests.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [NopResourceDisplayName("Admin.System.Maintenance.DeleteGuests.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            [NopResourceDisplayName("Admin.System.Maintenance.DeleteGuests.OnlyWithoutShoppingCart")]
            public bool OnlyWithoutShoppingCart { get; set; }

            public int? NumberOfDeletedCustomers { get; set; }
        }

        public partial class DeleteAbandonedCartsModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.System.Maintenance.DeleteAbandonedCarts.OlderThan")]
            [UIHint("Date")]
            public DateTime OlderThan { get; set; }

            public int? NumberOfDeletedItems { get; set; }
        }

        public partial class DeleteExportedFilesModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.StartDate")]
            [UIHint("DateNullable")]
            public DateTime? StartDate { get; set; }

            [NopResourceDisplayName("Admin.System.Maintenance.DeleteExportedFiles.EndDate")]
            [UIHint("DateNullable")]
            public DateTime? EndDate { get; set; }

            public int? NumberOfDeletedFiles { get; set; }
        }

        #endregion
    }
}
