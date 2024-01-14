using AO.Services.Domain;
using AO.Services.Models;
using Microsoft.Extensions.Azure;
using Nop.Core;
using Nop.Data;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public class AOInvoiceStatisticsService : IAOInvoiceStatisticsService
    {
        private readonly IRepository<AOInvoice> _invoiceRepository;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public AOInvoiceStatisticsService(IRepository<AOInvoice> invoiceRepository, IWorkContext workContext, ILocalizationService localizationService)
        {
            _invoiceRepository = invoiceRepository;
            _workContext = workContext;
            _localizationService = localizationService;
        }

        public async Task<InvoiceStatisticsModel> GetInvoiceStatisticsModelAsync(DateTime fromDateTime, DateTime toDateTime, int yearsBack)
        {
            if (fromDateTime > toDateTime)
            {
                throw new ArgumentException("From datetime cannot be after To datetime");
            }

            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var currentCulture = new CultureInfo(currentLanguage.LanguageCulture);
            var localizedYear = await _localizationService.GetResourceAsync("Common.Year");

            var model = new InvoiceStatisticsModel();
            var items = new List<InvoiceStatisticsModelItem>();
            
            // First add invoiced number today
            decimal totalToday = await GetTotalInvoicedAmount(DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1).AddTicks(-1));

            items.Add(new InvoiceStatisticsModelItem()
            {
                TotalInvoicedAmount = totalToday,
                YearAndMonth = $"I dag",
                Year = toDateTime.Year
            });

            decimal totalYesterday = await GetTotalInvoicedAmount(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date.AddTicks(-1));
            items.Add(new InvoiceStatisticsModelItem()
            {
                TotalInvoicedAmount = totalYesterday,
                YearAndMonth = $"I går",
                Year = toDateTime.Year
            });

            decimal totalThreeDaysAgo = await GetTotalInvoicedAmount(DateTime.UtcNow.Date.AddDays(-2), DateTime.UtcNow.Date.AddDays(-1).AddTicks(-1));
            items.Add(new InvoiceStatisticsModelItem()
            {
                TotalInvoicedAmount = totalThreeDaysAgo,
                YearAndMonth = $"I forgårs",
                Year = toDateTime.Year
            });

            for (int year = 0; year <= yearsBack; year++)
            {
                if (year > 0)
                {
                    // Deduct 1 year after the first run
                    fromDateTime = fromDateTime.AddYears(-1);
                    toDateTime = toDateTime.AddYears(-1);
                }

                int startMonth = (fromDateTime.Year == DateTime.UtcNow.Year) ? DateTime.UtcNow.Month : 12;
                int endMonth = 1;  // Always end at January for each year

                // Loop from the current month back to January (or from December back to January for past years)
                for (int month = startMonth; month >= endMonth; month--)
                {
                    DateTime startOfMonth = new DateTime(fromDateTime.Year, month, 1);
                    DateTime endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

                    decimal totalThisMonth = await GetTotalInvoicedAmount(startOfMonth, endOfMonth);
                    decimal totalYearToDate = await GetTotalInvoicedAmount(new DateTime(fromDateTime.Year, 1, 1), endOfMonth);

                    items.Add(new InvoiceStatisticsModelItem()
                    {
                        TotalInvoicedAmount = totalThisMonth,
                        TotalYearToDate = totalYearToDate,
                        YearAndMonth = $"{localizedYear} {fromDateTime.Year} ({startOfMonth.ToString("MMMM", currentCulture)})",
                        Year = toDateTime.Year
                    });
                }
            }

            model.InvoiceStatisticsItems = items;

            return model;
        }


        private async Task<decimal> GetTotalInvoicedAmount(DateTime fromDateTime, DateTime toDateTime)
        {
            var invoices = await _invoiceRepository.GetAllAsync(query =>
            {
                return from invoice in query
                       where invoice.InvoiceDate.Date >= fromDateTime.Date && invoice.InvoiceDate.Date <= toDateTime
                       select invoice;
            });

            decimal total = 0;
            foreach (var invoice in invoices)
            {
                total += invoice.InvoiceTotal;
            }

            return total;
        }
    }
}