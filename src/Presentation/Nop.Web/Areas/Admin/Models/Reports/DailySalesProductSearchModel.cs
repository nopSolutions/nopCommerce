using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports
{

    /// <summary>
    /// Represents a low stock product search model
    /// </summary>
    public partial record DailySalesProductSearchModel : BaseSearchModel
    {
        #region Ctor

        public DailySalesProductSearchModel()
        {
            AvailableVendors = new List<SelectListItem>();
            
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Reports.SalesSummary.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? Date { get; set; }

        [NopResourceDisplayName("Admin.Reports.SalesSummary.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Reports.SalesSummary.Product")]
        public int ProductId { get; set; }

      

        public IList<SelectListItem> AvailableVendors { get; set; }


        public bool IsLoggedInAsVendor { get; set; }
        #endregion
    }
}
