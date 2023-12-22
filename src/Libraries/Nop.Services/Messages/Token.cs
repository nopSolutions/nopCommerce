namespace Nop.Services.Messages;

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