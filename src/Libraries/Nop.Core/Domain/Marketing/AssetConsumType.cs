namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// Represents a AssetConsumType
    /// </summary>
    public enum AssetConsumType : byte
    {
        //所有收支方式：
        //收入：Subscribe订阅，Register账户注册，SignIn签到，Commission佣金，Recommend推荐，Bonus奖金红利，Return退回
        //Recharge充值，Purchase购买，SysReward系统奖励
        //支出：Unsubscribe退订，Cancel账户注销，Consume消费，Withdrawal提现，Expired过期，Invalid无效，Tax税，Settlement结算
        //SysDeduct系统扣除，Receive=获赠，Give赠送出

        //Subscribe订阅
        Subscribe = 1,
        //Register账户注册
        Register = 2,
        //SignIn签到
        SignIn = 3,
        //Commission佣金
        Commission = 4,
        //Recommend推荐
        Recommend = 5,
        //Bonus奖金红利
        Bonus = 6,
        //Return退回
        Return = 7,
        //Recharge充值
        Recharge = 8,
        //Purchase购买
        Purchase = 9,
        //Receive=获赠（他人购买赠送[个人]）
        Receive = 10,
        //PersonalPromotion=他人赠送[个人营销赠送，主要用于合伙人营销使用]
        PersonalPromotion = 11,
        //SupplierPromotion=商家促销活动赠送（商家赠送[商家]，主要用于商家促销使用）
        SupplierPromotion = 12,
        //广告收入或个人文章收入
        Advert = 13,
        //Unsubscribe退订
        Unsubscribe = 51,
        //Cancel账户注销
        Cancel = 52,
        //Settlement结算
        Settlement = 54,
        //Expired过期
        Expired = 55,
        //Invalid无效
        Invalid = 56,
        //Tax税
        Tax = 57,
        //Withdrawal提现
        Withdrawal = 58,
        //Consume消费
        Consume = 59,
        //Give赠送出
        Give = 60,
        //SysReward系统奖励
        SysReward = 90,
        //SysDeduct系统扣除
        SysDeduct = 91,
    }
}
