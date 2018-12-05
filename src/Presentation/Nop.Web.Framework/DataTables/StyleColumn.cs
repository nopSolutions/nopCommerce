using System.Runtime.Serialization;

namespace Nop.Web.Framework.DataTables
{
    /// <summary>
    /// Represents style of column
    /// </summary>
    public enum StyleColumn
    {
        /// <summary>
        /// Head and body content will be at center
        /// </summary>
        [EnumMember(Value = "dt-head-center dt-body-center")]
        centerAll = 1,
        /// <summary>
        /// Head content will be at center
        /// </summary>
        [EnumMember(Value = "dt-head-center")]
        centerHead = 2,
        /// <summary>
        /// Body content will be at center
        /// </summary>
        [EnumMember(Value = "dt-body-center")]
        centerBody = 3
    }
}
