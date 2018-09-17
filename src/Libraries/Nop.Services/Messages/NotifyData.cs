namespace Nop.Services.Messages
{
    /// <summary>
    /// Message structure
    /// </summary>
    public struct NotifyData
    {
        /// <summary>
        /// Message type (success/warning/error)
        /// </summary>
        public NotifyType Type { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; set; }
    }
}
