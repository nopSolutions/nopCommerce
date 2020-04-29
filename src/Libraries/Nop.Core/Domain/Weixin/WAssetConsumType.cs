namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a AssetConsumType
    /// </summary>
    public enum WAssetConsumType : byte
    {
        /*积分/消费收支方式*/
        //新用户注册
        NewRegistration = 1,
        //推荐新用户奖励
        Recommend = 2,
        //推荐用户注销扣除
        UserLogout = 3,
        //用户签到
        SignIn = 4,
        //返佣
        Commission = 5,
        //抽奖或奖励
        Bonus = 6,
        //用户充值
        Recharge = 7,
        //购买产品收入或消费
        Buy = 8,
        //系统奖励
        SystemReward = 9,
        //系统扣除
        SystemDeduct = 10,
        //用户提现
        Withdrawal = 11,
        //资产过期
        AssetExpiration = 12,
        //审核无效
        Invalid = 13,
        //赠送(送出或收入)
        Give = 14,
    }
}
