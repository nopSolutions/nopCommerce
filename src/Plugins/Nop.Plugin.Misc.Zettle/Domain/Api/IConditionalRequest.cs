namespace Nop.Plugin.Misc.Zettle.Domain.Api
{
    /// <summary>
    /// Represents request with ETag header.
    /// If the conditional prerequisite is fullfilled, the full resource is returned, otherwise a 304 not modified will be returned with an empty body
    /// </summary>
    public interface IConditionalRequest
    {
        /// <summary>
        /// Gets or sets the ETag header value
        /// </summary>
        public string ETag { get; set; }
    }
}