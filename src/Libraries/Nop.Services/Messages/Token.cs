<<<<<<< HEAD
<<<<<<< HEAD
﻿namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents token
    /// </summary>
    public sealed partial class Token
    {
        #region Ctor

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
            Key = key;
            Value = value;
            NeverHtmlEncoded = neverHtmlEncoded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Token key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Token value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Indicates whether this token should not be HTML encoded
        /// </summary>
        public bool NeverHtmlEncoded { get; }

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
=======
=======
=======
<<<<<<< HEAD
﻿namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents token
    /// </summary>
    public sealed partial class Token
    {
        #region Ctor

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
            Key = key;
            Value = value;
            NeverHtmlEncoded = neverHtmlEncoded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Token key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Token value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Indicates whether this token should not be HTML encoded
        /// </summary>
        public bool NeverHtmlEncoded { get; }

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
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents token
    /// </summary>
    public sealed partial class Token
    {
        #region Ctor

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
            Key = key;
            Value = value;
            NeverHtmlEncoded = neverHtmlEncoded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Token key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Token value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Indicates whether this token should not be HTML encoded
        /// </summary>
        public bool NeverHtmlEncoded { get; }

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
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
