using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Affiliates
{
    /// <summary>
    /// 隶属机构
    /// Represents an affiliate
    /// </summary>
    public partial class Affiliate : BaseEntity
    {
        /// <summary>
        /// 地址编号
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for generated affiliate URL (by default affiliate ID is used)
        /// </summary>
        public string FriendlyUrlName { get; set; }

        /// <summary>
        /// 是否删除
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// 是否有效
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the address
        /// </summary>
        public virtual Address Address { get; set; }
    }
}
