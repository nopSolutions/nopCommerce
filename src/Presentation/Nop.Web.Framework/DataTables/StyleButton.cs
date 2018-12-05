using System.Runtime.Serialization;
namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represenrs a button style 
    /// using https://adminlte.io/themes/AdminLTE/pages/UI/buttons.html
    /// </summary>
    public enum StyleButton
    {
        /// <summary>
        /// Default style
        /// </summary>
        [EnumMember(Value = "btn btn-default")]
        defaultStyle,
        /// <summary>
        /// Dark blue style
        /// </summary>
        [EnumMember(Value = "btn btn-primary")]
        primary,
        /// <summary>
        /// Green style
        /// </summary>
        [EnumMember(Value = "btn btn-success")]
        success,
        /// <summary>
        /// Blue style
        /// </summary>
        [EnumMember(Value = "btn btn-info")]
        info,
        /// <summary>
        /// Red style
        /// </summary>
        [EnumMember(Value = "btn btn-danger")]
        danger,
        /// <summary>
        /// Orange style
        /// </summary>
        [EnumMember(Value = "btn btn-warning")]
        warning        
    }
}
