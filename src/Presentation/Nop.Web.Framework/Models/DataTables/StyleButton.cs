using System.Runtime.Serialization;

namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents a button style 
    /// https://adminlte.io/themes/AdminLTE/pages/UI/buttons.html
    /// </summary>
    public enum StyleButton
    {
        /// <summary>
        /// Default style
        /// </summary>
        [EnumMember(Value = "btn btn-default")]
        Default,

        /// <summary>
        /// Dark blue style
        /// </summary>
        [EnumMember(Value = "btn btn-primary")]
        Primary,

        /// <summary>
        /// Green style
        /// </summary>
        [EnumMember(Value = "btn btn-success")]
        Success,

        /// <summary>
        /// Blue style
        /// </summary>
        [EnumMember(Value = "btn btn-info")]
        Info,

        /// <summary>
        /// Red style
        /// </summary>
        [EnumMember(Value = "btn btn-danger")]
        Danger,

        /// <summary>
        /// Orange style
        /// </summary>
        [EnumMember(Value = "btn btn-warning")]
        Warning,

        /// <summary>
        /// Olive style
        /// </summary>
        [EnumMember(Value = "btn bg-olive")]
        Olive        

    }
}