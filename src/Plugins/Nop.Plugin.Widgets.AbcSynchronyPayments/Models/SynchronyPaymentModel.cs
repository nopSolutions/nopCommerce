using System;

namespace Nop.Plugin.Widgets.AbcSynchronyPayments.Models
{
    public class SynchronyPaymentModel
    {
        public int MonthlyPayment { get; set; }
        public int EqualPayment { get; set; }
        public int MonthCount { get; set; }
        public int ProductId { get; set; }
        public string ApplyUrl { get; set; }
        public bool IsMonthlyPaymentStyle { get; set; }
        public string ModalHexColor { get; set; }
        public string StoreName { get; set; }
        public string ImageUrl { get; set; }
        public string OfferValidFrom { get; set; }
        public string OfferValidTo { get; set; }
        public decimal FullPrice { get; internal set; }
        public decimal FinalPayment { get; internal set; }
        public bool IsHidden { get; internal set; }
    }
}
