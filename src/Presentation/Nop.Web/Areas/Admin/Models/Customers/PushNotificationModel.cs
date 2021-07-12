using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial record PushNotificationModel : BaseNopEntityModel
    {
        public PushNotificationModel()
        {
            NotificationTypes = new List<SelectListItem>();
            NotificationTypes.Add(new SelectListItem() { Text = "All", Value = "All" });
            NotificationTypes.Add(new SelectListItem() { Text = "Rate Reminder Notification", Value = "RateReminderNotification" });
            NotificationTypes.Add(new SelectListItem() { Text = "Remind Me Notification", Value = "RemindMeNotification" });
            NotificationTypes.Add(new SelectListItem() { Text = "Order Status Notification", Value = "OrderStatusNotification" });
        }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.MessageTitle")]
        public string MessageTitle { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.MessageBody")]
        public string MessageBody { get; set; }

        public string NotificationType { get; set; }

        public IList<SelectListItem> NotificationTypes { get; set; }
    }
}