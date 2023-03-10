namespace Nop.Plugin.Widgets.FacebookPixel.Domain
{
    /// <summary>
    /// Represents tracked event details
    /// </summary>
    public class TrackedEvent
    {
        #region Ctor

        public TrackedEvent()
        {
            EventObjects = new List<string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the event name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this event is a custom event
        /// </summary>
        public bool IsCustomEvent { get; set; }

        /// <summary>
        /// Gets or sets the formatted objects for this event
        /// </summary>
        public IList<string> EventObjects { get; set; }

        #endregion
    }
}