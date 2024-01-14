namespace AO.Services.Models
{
    public class InvoiceStatisticsModelItem
    {
        public decimal TotalInvoicedAmount { get; set; }

        public decimal TotalYearToDate { get; set; }

        public string YearAndMonth { get; set; }

        public int Year { get; set; }
    }
}
