using AO.Services;
using AO.Services.Domain;
using AO.Services.Models;
using AO.Services.Services;
using Microsoft.Extensions.Configuration;
using Nop.Core.Domain.Common;
using Nop.Plugin.Admin.Accounting.Models.EconomicModels;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting.Services
{
    public class BookingSchedule : IScheduleTask
    {
        #region Private variables
        private readonly ILogger _logger;
        private readonly AccountingSettings _accountingSettings;
        private readonly IEconomicGateway _economicGateway;
        private readonly IAOInvoiceService _invoiceService;
        private StringBuilder _errorMessage = new StringBuilder();
        private int _errorCount = 0;
        private int _maxErrors = 100;
        private int _layoutId, _paymentTermsId;
        #endregion

        public BookingSchedule(AccountingSettings accountingSettings, ILogger logger, IAOInvoiceService invoiceService, IEconomicGateway economicGateway, IConfiguration configuration)
        {
            _accountingSettings = accountingSettings;
            _logger = logger;
            _invoiceService = invoiceService;
            _economicGateway = economicGateway;

            _layoutId = Convert.ToInt32(configuration["EconomicApiSettings:LayoutId"]);
            _paymentTermsId = Convert.ToInt32(configuration["EconomicApiSettings:PaymentTermsId"]);
        }

        async System.Threading.Tasks.Task IScheduleTask.ExecuteAsync()
        {
            var runningTime = System.Diagnostics.Stopwatch.StartNew();
            var startTime = DateTime.Now;
            string logText = "";
            int bookedCount = 0;

            try
            {
                var notBookedInvoices = await _invoiceService.GetInvoicesNotBookedAsync();
                if (notBookedInvoices != null && notBookedInvoices.Count > 0)
                {
                    foreach (var invoice in notBookedInvoices)
                    {
                        try
                        {
                            var booked = await BookInvoiceAsync(invoice);
                            if (booked)
                            {
                                bookedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _errorCount++;
                            await _logger.ErrorAsync(ex.Message, ex);
                        }

                        if (_errorCount > _maxErrors)
                        {
                            string message = $"Too many errors occurred, we are stopping the booking of invoices now. ({_errorCount}){Environment.NewLine}There might be more information in other logs.";
                            await _logger.ErrorAsync(message);
                            break;
                        }
                    }

                    logText += $"{bookedCount} invoices has been booked in e-conomic";
                }
                else
                {
                    logText += $"No invoices found to book in e-conomic";
                }

            }
            catch (AggregateException ae)
            {
                var sb = new StringBuilder();
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    if (e is TimeoutException)
                    {
                        sb.AppendLine($"TimeoutException: {e.Message}{Environment.NewLine}Start time: {startTime}");
                    }
                    else
                    {
                        sb.AppendLine(e.Message);
                    }
                }
                await _logger.ErrorAsync($"BookingSchedule.ExecuteAsync(), AggregateException: {sb}");

                throw;
            }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null)
                    inner = inner.InnerException;
                await _logger.ErrorAsync("BookingSchedule.ExecuteAsync(): " + inner.Message, ex);

                throw;
            }
            finally
            {
                runningTime.Stop();
                string timoutSetting = Utilities.GetTimeOutSettingText();
                string timeElapsed = $"Time elapsed: {string.Format("{0:hh\\:mm\\:ss}", runningTime.Elapsed)}";
                if (string.IsNullOrEmpty(timoutSetting) == false)
                {
                    timeElapsed += $"{timoutSetting}";
                }
                var errorMessages = _errorMessage.ToString();
                if (string.IsNullOrEmpty(errorMessages) == false)
                {
                    logText += $"{Environment.NewLine}{errorMessages}{Environment.NewLine}";
                }

                logText += $"{Environment.NewLine}BookingSchedule Sync done{Environment.NewLine}";
                await _logger.InformationAsync(logText + Environment.NewLine + timeElapsed);
            }
        }

        #region Private methods
        private async Task<bool> BookInvoiceAsync(AOInvoice invoice)
        {
            var isAlreadyBooked = await _economicGateway.HasBeenBookedAsync(invoice);
            if (isAlreadyBooked)
            {
                _errorCount++;
                _errorMessage.AppendLine($"Invoice is already booked. {invoice.Id}");
                return false;
            }

            // First set the date for the booking, then we know when we first tried
            invoice.BookedDate = DateTime.UtcNow;
            await _invoiceService.UpdateInvoiceAsync(invoice);

            var invoiceModel = await _invoiceService.GetInvoiceModelByIdAsync(invoice.Id);

            // Check whether customer exists in e-conomic, if not, the create the customer
            await EnsureCustomerExistAsync(invoiceModel);

            // Build the model to book
            var invoiceBookModel = BuildModel(invoiceModel);

            // Do the actual booking in e-conomic
            var draftInvoiceNumber = await _economicGateway.BookInvoiceAsync(invoiceBookModel);
            if (draftInvoiceNumber <= 0)
            {
                _errorCount++;
                return false;
            }
            else
            {
                // Now set the new invoice number to complete the booking
                invoice.EconomicInvoiceNumber = draftInvoiceNumber;
                await _invoiceService.UpdateInvoiceAsync(invoice);
                return true;
            }
        }

        private async Task EnsureCustomerExistAsync(InvoiceModel invoiceModel)
        {
            var exist = await _economicGateway.CustomerExistAsync(invoiceModel.CustomerId);
            if (exist)
            {
                return;
            }

            var vatZone = GetVatZone(invoiceModel.CustomerCurrencyCode);

            var customerInfo = new CustomerInfo()
            {
                Address = GetAddress(invoiceModel.InvoiceAddress) ?? "No address",
                Name = GetName(invoiceModel.InvoiceAddress) ?? "No name",
                Zip = invoiceModel.InvoiceAddress.ZipPostalCode ?? "No zipcode",
                City = invoiceModel.InvoiceAddress.City ?? "No city",
                Country = invoiceModel.InvoiceCountry?.Name ?? "No country",
                CustomerNumber = invoiceModel.CustomerId,
                Email = invoiceModel.InvoiceAddress.Email ?? "No email",
                Currency = invoiceModel.CustomerCurrencyCode,
                CustomerGroup = new CustomerGroup()
                {
                    CustomerGroupNumber = 1
                },
                VatZone = vatZone,
                VatNumber = "",
                Layout = new Layout()
                {
                    LayoutNumber = _layoutId //21 is prod 19 is for test environment
                },
                PaymentTerms = new PaymentTerms()
                {
                    Name = "Til omgående betaling",
                    DaysOfCredit = 0,
                    PaymentTermsNumber = _paymentTermsId,
                    PaymentTermsType = "net"
                },
                PublicEntryNumber = "",
                MobilePhone = invoiceModel.InvoiceAddress.PhoneNumber ?? "",
                TelephoneAndFaxNumber = "",
                CorporateIdentificationNumber = ""
            };

            var createCustomerSucceeded = await _economicGateway.CreateCustomerAsync(customerInfo);
            if (createCustomerSucceeded == false)
            {
                _errorCount++;
                await _logger.ErrorAsync($"E-conomic: No user could be created this id: {customerInfo.CustomerNumber}");
            }
        }

        private string GetAddress(Address invoiceAddress)
        {
            if (string.IsNullOrWhiteSpace(invoiceAddress.Address1) && string.IsNullOrWhiteSpace(invoiceAddress.Address2))
            {
                return "No address";
            }

            return $"{invoiceAddress.Address1} {invoiceAddress.Address2}";
        }

        private InvoiceBookModel BuildModel(InvoiceModel invoiceModel)
        {
            var vatZone = GetVatZone(invoiceModel.CustomerCurrencyCode);
            var productLine = GetProductLine(invoiceModel);

            var invoiceBookModel = new InvoiceBookModel()
            {
                Currency = invoiceModel.CustomerCurrencyCode,
                Date = invoiceModel.InvoiceDate.ToString("yyyy-MM-dd"),
                ExchangeRate = 100,
                GrossAmount = Math.Round(invoiceModel.InvoiceTotal, 2),
                NetAmount = Math.Round(invoiceModel.InvoiceTotal - invoiceModel.InvoiceTax, 2),
                VatAmount = Math.Round(invoiceModel.InvoiceTax, 2),
                Recipient = new Recipient()
                {
                    Name = GetName(invoiceModel.InvoiceAddress),
                    Address = GetAddress(invoiceModel.InvoiceAddress),
                    Zip = invoiceModel.InvoiceAddress.ZipPostalCode ?? "No zipcode",
                    City = invoiceModel.InvoiceAddress.City ?? "No city",
                    VatZone = vatZone
                },
                Delivery = new Delivery()
                {
                    Address = $"{invoiceModel.InvoiceAddress.Address1} {invoiceModel.InvoiceAddress.Address2}",
                    Zip = invoiceModel.InvoiceAddress.ZipPostalCode ?? "No zipcode",
                    City = invoiceModel.InvoiceAddress.City ?? "No city",
                    Country = invoiceModel.InvoiceCountry?.Name ?? "No country",
                    DeliveryDate = DateTime.UtcNow
                },
                Customer = new Customer()
                {
                    CustomerNumber = invoiceModel.CustomerId
                },
                Layout = new Layout()
                {
                    LayoutNumber = _layoutId
                },
                PaymentTerms = new PaymentTerms()
                {
                    Name = "Til omgående betaling",
                    DaysOfCredit = 0,
                    PaymentTermsNumber = _paymentTermsId,
                    PaymentTermsType = "net"
                },
                NetAmountInBaseCurrency = Math.Round(invoiceModel.InvoiceTotal - invoiceModel.InvoiceTax, 2),
                References = new References()
                {
                    Other = $"{invoiceModel.PaymentMethod} ({invoiceModel.OrderId}) - {invoiceModel.CustomerPresentationName}, {invoiceModel.InvoiceCustomer.Email}"
                },
                Lines = new List<Line>
                {
                    productLine
                },
                Notes = new Notes()
                {
                    Heading = BuildHeader(invoiceModel),
                    TextLine1 = $"Webshop invoicenumber: {invoiceModel.InvoiceNumber}",
                    TextLine2 = $"Webshop orderid: {invoiceModel.OrderId}"
                }
            };

            return invoiceBookModel;
        }

        private string BuildHeader(InvoiceModel invoiceModel)
        {
            string header = invoiceModel.InvoiceTotal > 0 ? "Faktura" : "Kreditnota";

            if (invoiceModel.InvoiceItems[0].ProductId == 193359) // Kasselukningsprodukt
            {
                header += " - Kasselukning (butik)";
            }
            else
            {
                switch (invoiceModel.CustomerCurrencyCode)
                {
                    case "DKK":
                        header += " - Friliv.dk"; break;
                    case "SEK":
                        header += " - Andersenoutdoor.se"; break;
                    default:
                        header += " - Ukendt butik"; break;
                }
            }

            if (invoiceModel.OrderId > 0)
            {
                header += $" ({invoiceModel.OrderId})";
            }

            return header;
        }

        private string GetName(Address invoiceAddress)
        {
            if (string.IsNullOrWhiteSpace(invoiceAddress.FirstName) && string.IsNullOrWhiteSpace(invoiceAddress.LastName))
            {
                return "No address";
            }

            return $"{invoiceAddress.FirstName} {invoiceAddress.LastName}";
        }

        private Line GetProductLine(InvoiceModel invoiceModel)
        {
            string productNumber = _accountingSettings.WebshopDK_ProductNumber;

            if (invoiceModel.InvoiceItems[0].ProductId == 193359) // Kasselukningsprodukt
            {
                productNumber = _accountingSettings.PhysicalShop_ProductNumber;
            }
            else
            {
                if (invoiceModel.InvoiceCountry != null)
                {
                    switch (invoiceModel.InvoiceCountry.ThreeLetterIsoCode.ToUpper())
                    {
                        case "DKK":
                        case "DNK":
                            productNumber = _accountingSettings.WebshopDK_ProductNumber; break;
                        case "SEK":
                        case "SWE":
                            productNumber = _accountingSettings.WebshopSE_ProductNumber; break;
                        default:
                            {
                                if (invoiceModel.InvoiceCountry.SubjectToVat)
                                {
                                    productNumber = _accountingSettings.WebshopOther_ProductNumber; break;
                                }
                                else
                                {
                                    productNumber = _accountingSettings.WebshopOtherWithoutTax_ProductNumber; break;
                                }
                            }
                    }
                }
            }

            decimal unitNetPrice = invoiceModel.InvoiceTotal - invoiceModel.InvoiceTax;
            unitNetPrice = Math.Round(unitNetPrice, 2);

            var line = new Line()
            {
                LineNumber = 1,
                Product = new Product()
                {
                    ProductNumber = productNumber
                },
                SortKey = 1,
                Quantity = 1,
                Unit = new Unit()
                {
                    Name = "Combined product line",
                    UnitNumber = 1
                },
                Description = GetProductName(invoiceModel),
                DiscountPercentage = 0,
                UnitNetPrice = unitNetPrice
            };

            return line;
        }

        private string GetProductName(InvoiceModel invoiceModel)
        {
            var sb = new StringBuilder();
            foreach (var item in invoiceModel.InvoiceItems)
            {
                sb.Append($"{item.ProductName}, ");
            }

            string productName = sb.ToString();
            if (productName.Length > 1000)
            {
                productName = productName.Substring(0, 999);
            }

            return productName;
        }

        private VatZone GetVatZone(string customerCurrencyCode)
        {
            VatZone vatZone = null;
            switch (customerCurrencyCode.ToUpper())
            {
                case "DKK":
                    vatZone = new VatZone()
                    {
                        Name = "Domestic",
                        VatZoneNumber = 1
                    };
                    break;
                case "SEK":
                    vatZone = new VatZone()
                    {
                        Name = "Sweden",
                        VatZoneNumber = 31
                    };
                    break;
                default:
                    vatZone = new VatZone()
                    {
                        Name = "Abroad",
                        VatZoneNumber = 3
                    };
                    break;
            }

            vatZone.EnabledForCustomer = true;
            vatZone.EnabledForSupplier = true;

            return vatZone;
        }
        #endregion
    }
}