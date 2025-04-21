using System.Runtime.Serialization;

namespace Nop.Plugin.Widgets.AccessiBe.Domain;

/// <summary>
/// Represents an enumeration of button shapes 
/// </summary>
public enum TriggerButtonShape
{
    /// <summary>
    /// Circle (square with corner radius 50%)
    /// </summary>
    [EnumMember(Value = "50%")]
    Round,

    /// <summary>
    /// Square without corner radius
    /// </summary>
    [EnumMember(Value = "0")]
    Square,

    /// <summary>
    /// Square with corner radius 10px
    /// </summary>
    [EnumMember(Value = "10px")]
    SquircleBig,

    /// <summary>
    /// Square with corner radius 5px
    /// </summary>
    [EnumMember(Value = "5px")]
    SquircleSmall
}
