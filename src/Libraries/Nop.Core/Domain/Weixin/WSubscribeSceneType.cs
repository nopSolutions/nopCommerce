namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a WSubscribeSceneType
    /// </summary>
    public enum WSubscribeSceneType : byte
    {
        //返回用户关注的渠道来源
        //ADD_SCENE_OTHERS 其他
        ADD_SCENE_OTHERS = 0,
        //ADD_SCENE_SEARCH 公众号搜索
        ADD_SCENE_SEARCH = 1,
        //ADD_SCENE_ACCOUNT_MIGRATION 公众号迁移
        ADD_SCENE_ACCOUNT_MIGRATION = 2,
        //ADD_SCENE_PROFILE_CARD 名片分享
        ADD_SCENE_PROFILE_CARD = 3,
        //ADD_SCENE_QR_CODE 扫描二维码
        ADD_SCENE_QR_CODE = 4,
        //ADD_SCENE_PROFILE_LINK 图文页内名称点击
        ADD_SCENE_PROFILE_LINK = 5,
        //ADD_SCENE_PROFILE_ITEM 图文页右上角菜单
        ADD_SCENE_PROFILE_ITEM = 6,
        //ADD_SCENE_PAID 支付后关注
        ADD_SCENE_PAID = 7,
        //微信广告
        ADD_SCENE_WECHAT_ADVERTISEMENT = 8,
    }
}
