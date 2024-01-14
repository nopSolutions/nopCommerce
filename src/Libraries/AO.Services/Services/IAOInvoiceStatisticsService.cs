using AO.Services.Models;
using System;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public interface IAOInvoiceStatisticsService
    {
        Task<InvoiceStatisticsModel> GetInvoiceStatisticsModelAsync(DateTime fromDateTime, DateTime toDateTime, int yearsBack);
    }
}