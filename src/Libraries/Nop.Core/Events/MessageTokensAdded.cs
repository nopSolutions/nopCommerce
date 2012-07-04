using System.Collections.Generic;
using System;
using Nop.Core.Domain.Messages;
namespace Nop.Core.Events
{
    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageTokensAdded<U>
    {
        private readonly MessageTemplate _message;
        private readonly IList<U> _tokens;

        public MessageTokensAdded(MessageTemplate message, IList<U> tokens)
        {
            _message = message;
            _tokens = tokens;
        }

        public MessageTemplate Message { get { return _message; } }
        public IList<U> Tokens { get { return _tokens; } }
    }
}
