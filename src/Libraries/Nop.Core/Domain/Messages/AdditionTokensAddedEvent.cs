using System.Collections.Generic;

namespace Nop.Core.Domain.Messages
{
    /// <summary>
    /// Event for "Additional tokens added"
    /// </summary>
    public class AdditionTokensAddedEvent
    {
        public AdditionTokensAddedEvent()
        {
            this.AdditionTokens = new List<string>();
        }

        /// <summary>
        /// Add tokens
        /// </summary>
        /// <param name="additionTokens">Additional tokens</param>
        public void AddTokens(params string[] additionTokens)
        {
            foreach (var additionToken in additionTokens)
            {
                AdditionTokens.Add(additionToken);
            }
        }

        /// <summary>
        /// Additional tokens
        /// </summary>
        public IList<string> AdditionTokens { get; }
    }
}