namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a WMenuButtonType
    /// </summary>
    public enum WMenuButtonType : byte
    {
        /// <summary>
        /// 点击推事件
        /// </summary>
        Click = 1,
        /// <summary>
        /// 跳转URL事件
        /// </summary>
        View = 2,
        /// <summary>
        /// 图片
        /// </summary>
        Img = 3,
        /// <summary>
        /// 
        /// </summary>
        Photo = 4,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 5,
        /// <summary>
        /// 音频
        /// </summary>
        Voice = 6,
        /// <summary>
        /// 图文
        /// </summary>
        News = 7,
        /// <summary>
        /// 小程序
        /// </summary>
        Miniprogram = 8,
        /// <summary>
        /// 扫码推事件用户点击按钮后，微信客户端将调起扫一扫工具
        /// </summary>
        Scancode_push = 9,
        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框
        /// </summary>
        Scancode_waitmsg = 10,
        /// <summary>
        /// 弹出系统拍照发图
        /// </summary>
        Pic_sysphoto = 11,
        /// <summary>
        /// 弹出拍照或者相册发图
        /// </summary>
        Pic_photo_or_album = 12,
        /// <summary>
        /// 弹出微信相册发图器
        /// </summary>
        Pic_weixin = 13,
        /// <summary>
        /// 弹出地理位置选择器
        /// </summary>
        Location_select = 14,
        /// <summary>
        /// 下发消息（除文本消息）
        /// </summary>
        Media_id = 15,
        /// <summary>
        /// 跳转图文消息URL
        /// </summary>
        View_limited = 16,
    }
}
