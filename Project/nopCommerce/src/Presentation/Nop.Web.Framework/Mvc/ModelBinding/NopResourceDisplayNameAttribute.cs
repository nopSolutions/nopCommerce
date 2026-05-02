namespace Nop.Web.Framework.Mvc.ModelBinding;

/// <summary>
/// Represents model property attribute that specifies the display name by passed key of the locale resource
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NopResourceDisplayNameAttribute : Attribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the attribute
    /// </summary>
    /// <param name="resourceKey">Key of the locale resource</param>
    public NopResourceDisplayNameAttribute(string resourceKey)
    {
        ResourceKey = resourceKey;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets key of the locale resource 
    /// </summary>
    public string ResourceKey { get; set; }

    #endregion
}