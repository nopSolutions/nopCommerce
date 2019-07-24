using System.Collections.Generic;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Event for "Additional tokens added"
    /// </summary>
    public class AdditionalTokensAddedEvent
    {
        public AdditionalTokensAddedEvent()
        {
            AdditionalTokens = new List<string>();
        }

        /// <summary>
        /// Add tokens
        /// </summary>
        /// <param name="additionalTokens">Additional tokens</param>
        public void AddTokens(params string[] additionalTokens)
        {
            foreach (var additionalToken in additionalTokens)
            {
                AdditionalTokens.Add(additionalToken);
            }
        }

        /// <summary>
        /// Additional tokens
        /// </summary>
        public IList<string> AdditionalTokens { get; }
    }
}