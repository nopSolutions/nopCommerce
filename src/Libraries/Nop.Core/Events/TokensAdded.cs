using System.Collections.Generic;
namespace Nop.Core.Events
{
    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TokensAdded<T, U> where T : BaseEntity
    {
        private readonly T _entity;
        private readonly IList<U> _tokens;

        public TokensAdded(T entity, IList<U> tokens)
        {
            _entity = entity;
            _tokens = tokens;
        }

        public T Entity { get { return _entity; } }
        public IList<U> Tokens { get { return _tokens; } }
    }
}
