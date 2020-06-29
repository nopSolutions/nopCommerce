namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a SceneType
    /// </summary>
    public enum WSceneType : byte
    {
        //场景类型：主要用于临时二维码的分类和临时二维码不同字符串格式生成

        /// <summary>
        /// 回复消息，如果绑定的消息Id为0，回复success
        /// </summary>
        Message = 0,
        /// <summary>
        /// 广告类型二维码
        /// </summary>
        Adver = 1,
        /// <summary>
        /// 产品二维码
        /// </summary>
        Product = 2,
        /// <summary>
        /// 供应商/或店铺二维码
        /// </summary>
        Supplier = 3,
        /// <summary>
        /// 获取验证码类型二维码
        /// </summary>
        Verify = 4,
        /// <summary>
        /// 通过Login授权方式登记
        /// </summary>
        Command = 5,
        /// <summary>
        /// 扫描对应投票选项的二维码
        /// </summary>
        Vote = 6,
        /// <summary>
        /// 礼品卡：扫码领取赠送的卡券
        /// </summary>
        GiftCard = 7,
        /// <summary>
        /// 名片，身份证类型二维码
        /// </summary>
        IDCard = 8,
        /// <summary>
        /// 无
        /// </summary>
        None = 99,
    }
}
