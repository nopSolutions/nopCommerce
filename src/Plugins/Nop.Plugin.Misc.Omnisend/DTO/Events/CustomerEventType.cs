namespace Nop.Plugin.Misc.Omnisend.DTO.Events;

public static class CustomerEventType
{
    public static string AddedProduct => "added product to cart";

    public static string StartedCheckout => "started checkout";

    public static string PlacedOrder => "placed order";

    public static string OrderPaid => "paid for order";

    public static string OrderCanceled => "order canceled";

    public static string OrderFulfilled => "order fulfilled";

    public static string OrderRefunded => "order refunded";
}