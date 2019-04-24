using System.Runtime.Serialization;

namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represents style of column
    /// </summary>
    public enum StyleColumn
    {
        /// <summary>
        /// Head and body content will be at center
        /// </summary>
        [EnumMember(Value = "text-center")]
        CenterAll = 1,

        /// <summary>
        /// Parent-child control element
        /// </summary>
        [EnumMember(Value = "child-control")]
        ChildControl = 4,

        /// <summary>
        /// Column contains button
        /// </summary>
        [EnumMember(Value = "button-column")]
        ButtonStyle = 5
    }
}