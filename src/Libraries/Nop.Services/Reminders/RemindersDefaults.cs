using Nop.Core.Domain.Messages;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents default values related to reminders
/// </summary>
public static partial class RemindersDefaults
{
    /// <summary>
    /// Represents defaults related to carts reminders
    /// </summary>
    public static partial class AbandonedCarts
    {
        /// <summary>
        /// Gets the name of the follow up attribute
        /// </summary>
        public static string FollowUpAttributeName => "AbandonedCartFollowUp";

        /// <summary>
        /// Gets the type name of the processing task
        /// </summary>
        public static string ProcessTaskTypeFullName => typeof(ProcessAbandonedCartsTask).FullName;

        /// <summary>
        /// Gets the list of message template system names
        /// </summary>
        public static string[] FollowUpList =>
        [
            MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE,
            MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE,
            MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE
        ];
    }

    /// <summary>
    /// Represents defaults related to pending orders
    /// </summary>
    public static partial class PendingOrders
    {
        /// <summary>
        /// Gets the name of the follow up attribute
        /// </summary>
        public static string FollowUpAttributeName => "PendingOrderFollowUp";

        /// <summary>
        /// Gets the type name of the processing task
        /// </summary>
        public static string ProcessTaskTypeFullName => typeof(ProcessPendingOrdersTask).FullName;

        /// <summary>
        /// Gets the list of message template system names
        /// </summary>
        public static string[] FollowUpList =>
        [
            MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE,
            MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE
        ];
    }

    /// <summary>
    /// Represents defaults related to incomplete registrations
    /// </summary>
    public static partial class IncompleteRegistrations
    {
        /// <summary>
        /// Gets the name of the follow up attribute
        /// </summary>
        public static string FollowUpAttributeName => "IncompleteRegistrationFollowUp";

        /// <summary>
        /// Gets the type name of the processing task
        /// </summary>
        public static string ProcessTaskTypeFullName => typeof(ProcessIncompleteRegistrationsTask).FullName;

        /// <summary>
        /// Gets the list of message template system names
        /// </summary>
        public static string[] FollowUpList =>
        [
            MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE
        ];
    }
}
