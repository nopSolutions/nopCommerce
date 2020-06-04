using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 店铺图片
    /// </summary>
    public partial class SupplierImage : BaseEntity
    {
        /// <summary>
        /// 【SupplierShop.Id】供应商店铺ID
        /// </summary>
        public int SupplierShopId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 工作职称
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// 职称等级
        /// </summary>
        public string GradeLevel { get; set; }
        /// <summary>
        /// 星级
        /// </summary>
        public byte Stars { get; set; }
        /// <summary>
        /// 图片类型：环境图，员工图，产品图
        /// </summary>
        public byte SupplierImageTypeId { get; set; }
        /// <summary>
        /// 图片类型：环境图，员工图，产品图
        /// </summary>
        public SupplierImageType SupplierImageType
        {
            get => (SupplierImageType)SupplierImageTypeId;
            set => SupplierImageTypeId = (byte)value;
        }
        /// <summary>
        /// 【SupplierSelfGroup.Id】供应商自定义分组ID
        /// </summary>
        public int? SupplierSelfGroupId { get; set; }
        /// <summary>
        /// 系统消息，主要用于审核反馈信息用
        /// </summary>
        public string SysMessage { get; set; }
        /// <summary>
        /// 状态（保留字段，是否通过审核等）
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
