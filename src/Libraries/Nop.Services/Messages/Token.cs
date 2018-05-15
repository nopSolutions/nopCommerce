namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents token
    /// </summary>
    public sealed class Token
    {
        #region Fields

        private readonly string _key;
        private readonly object _value;
        private readonly bool _neverHtmlEncoded;

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public Token(string key, object value) : this(key, value, false)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="neverHtmlEncoded">Indicates whether this token should not be HTML encoded</param>
        public Token(string key, object value, bool neverHtmlEncoded)
        {
            this._key = key;
            this._value = value;
            this._neverHtmlEncoded = neverHtmlEncoded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Token key
        /// </summary>
        public string Key { get { return _key; } }

        /// <summary>
        /// Token value
        /// </summary>
        public object Value { get { return _value; } }
        
        /// <summary>
        /// Indicates whether this token should not be HTML encoded
        /// </summary>
        public bool NeverHtmlEncoded { get { return _neverHtmlEncoded; } }

        #endregion

        #region Methods

        /// <summary>
        /// The string representation of the value of this token
        /// </summary>
        /// <returns>String value</returns>
        public override string ToString()
        {
            return $"{Key}: {Value}";
        }

        #endregion
    }
}
