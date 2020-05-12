namespace Nop.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// Represents delivery service
    /// </summary>
    /// <remarks>
    /// Updated at January 7, 2019
    /// </remarks>
    public enum DeliveryService
    {
        /// <summary>
        /// Next Day Air
        /// </summary>
        [UPSCode("01")]
        NextDayAir,

        /// <summary>
        /// 2nd Day Air
        /// </summary>
        [UPSCode("02")]
        _2ndDayAir,

        /// <summary>
        /// Ground
        /// </summary>
        [UPSCode("03")]
        Ground,

        /// <summary>
        /// Worldwide Express
        /// </summary>
        [UPSCode("07")]
        WorldwideExpress,

        /// <summary>
        /// Worldwide Expedited
        /// </summary>
        [UPSCode("08")]
        WorldwideExpedited,

        /// <summary>
        /// Standard
        /// </summary>
        [UPSCode("11")]
        Standard,

        /// <summary>
        /// 3 Day Select
        /// </summary>
        [UPSCode("12")]
        _3DaySelect,

        /// <summary>
        /// Next Day Air Saver
        /// </summary>
        [UPSCode("13")]
        NextDayAirSaver,

        /// <summary>
        /// Next Day Air Early
        /// </summary>
        [UPSCode("14")]
        NextDayAirEarly,

        /// <summary>
        /// Worldwide Express Plus
        /// </summary>
        [UPSCode("54")]
        WorldwideExpressPlus,

        /// <summary>
        /// 2nd Day Air A.M.
        /// </summary>
        [UPSCode("59")]
        _2ndDayAirAm,

        /// <summary>
        /// Worldwide Saver
        /// </summary>
        [UPSCode("65")]
        WorldwideSaver,

        //Currently not available. Actually this is a pickup point service, so you need implement IPickupPointProvider to use this feature.
        ///// <summary>
        ///// Access Point Economy
        ///// </summary>
        //[UPSCode("70")]
        //AccessPointEconomy,

        /// <summary>
        /// Worldwide Express Freight Midday
        /// </summary>
        [UPSCode("71")]
        WorldwideExpressFreightMidday,

        /// <summary>
        /// Express 12:00
        /// </summary>
        [UPSCode("74")]
        Express1200,

        /// <summary>
        /// Heavy Goods
        /// </summary>
        [UPSCode("75")]
        HeavyGoods,

        /// <summary>
        /// Today Standard
        /// </summary>
        [UPSCode("82")]
        TodayStandard,

        /// <summary>
        /// Today Dedicated Courrier
        /// </summary>
        [UPSCode("83")]
        TodayDedicatedCourrier,

        /// <summary>
        /// Today Express
        /// </summary>
        [UPSCode("85")]
        TodayExpress,

        /// <summary>
        /// Today Express Saver
        /// </summary>
        [UPSCode("86")]
        TodayExpressSaver,

        /// <summary>
        /// Worldwide Express Freight
        /// </summary>
        [UPSCode("96")]
        WorldwideExpressFreight
    }
}