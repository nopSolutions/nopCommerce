using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 中国行政区划代码表:http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2019/index.html
    /// </summary>
    public partial class DivisionsCodeChina : BaseEntity
    {
        /// <summary>
        /// 国家代码（保留暂不用）
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 区划代码
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 区划等级：1=省级，2=市级，3=区级，4=街道，5=居委会
        /// </summary>
        public byte AreaLevel { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 是否热门区域
        /// </summary>
        public bool HotArea { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }

    }
}
