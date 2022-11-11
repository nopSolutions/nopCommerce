using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log model
    /// </summary>
    public partial record ActivityLogModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }

        [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.Customer")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CustomerEmail")]
        [DataType(DataType.EmailAddress)]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.Comment")]
        public string Comment { get; set; }

        [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.Customers.ActivityLog.Fields.IpAddress")]
        public string IpAddress { get; set; }

        #endregion
    }
}