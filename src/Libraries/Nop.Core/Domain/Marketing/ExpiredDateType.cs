namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 过期时间类型
    /// </summary>
    public enum ExpiredDateType : byte
    {
        //1=自购买之日起x时
        HoursFromNow = 1,
        //2=自购买之日起x天
        DaysFromNow = 2,
        //3=自购买之日起x月
        MonthsFromNow = 3,
        //4=固定到期日期
        FixDateTime = 4,
    }
}
