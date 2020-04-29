namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WUserAddress
    /// </summary>
    public partial class WUserAddress : BaseEntity
    {
        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// 收货人手机号
        /// </summary>
        public string TelNumber { get; set; }
        /// <summary>
        /// 收货地址国家码
        /// </summary>
        public string NationalCode { get; set; }
        /// <summary>
        /// 国标收货地址第一级地址-省
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 国标收货地址第二级地址-市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 国标收货地址第三级地址-国家
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// 详细收货地址信息
        /// </summary>
        public string DetailInfo { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }

    }
}
