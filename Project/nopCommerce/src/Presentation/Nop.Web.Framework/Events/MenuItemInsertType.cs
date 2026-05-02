namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents a menu item insertion type
/// </summary>
public enum MenuItemInsertType
{
    /// <summary>
    /// Insert item after specified one
    /// </summary>
    After,
    /// <summary>
    /// Insert item before specified one
    /// </summary>
    Before,
    /// <summary>
    /// Try to insert the item after the specified one, if it fails, paste it before the specified element
    /// </summary>
    TryAfterThanBefore,
    /// <summary>
    /// Try to insert the item before the specified one, if it fails, paste it after the specified element
    /// </summary>
    TryBeforeThanAfter
}