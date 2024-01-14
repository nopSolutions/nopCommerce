using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.POS.Kaching.Services;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching.Controller
{
    public class SalesController : BasePluginController
    {
        private readonly ILogger _logger;
        private readonly ILogService _logService;
        private readonly ISalesService _salesService;
        private static string _logFileNameExtension = "SalesController-";

        public SalesController(ILogger logger, ILogService logService, ISalesService salesService)
        {
            _logger = logger;
            _logService = logService;
            _salesService = salesService;
        }

        [HttpGet]
        [Route("/api/sales", Name = "GetSales")]
        public IActionResult Get()
        {
            return Ok("Sales endpoint ready");
        }

        [HttpPost]
        [Route("/api/saleswithkey", Name = "CreateSalesWithKey")]
        public async Task<IActionResult> PostAsync([Microsoft.AspNetCore.Mvc.FromBody] Dictionary<string, Models.SalesModels.Root> sales)
        {
            try
            {
                if (!ModelState.IsValid)
                {                    
                    var errors = new StringBuilder("Error in Sales From Kaching");
                    foreach (var key in ModelState.Keys)
                    {
                        foreach (var error in ModelState[key].Errors)
                        {
                            errors.AppendLine($"{key}: {error.ErrorMessage}");
                        }
                    }

                    await _logger.ErrorAsync(errors.ToString());
                   
                    return BadRequest(ModelState);
                }
            }
            catch (JsonException ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }

            await _logger.InformationAsync("SalesController with key called, ready");
            var accountingFileName = await _logService.LogRawJsonAsync(JsonConvert.SerializeObject(sales), _logFileNameExtension);

            int stockUpdatedCount;
            try
            {
                stockUpdatedCount = await _salesService.UpdateStockAsync(sales);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                throw;
            }

            return Ok($"Stock quantity has been updated for {stockUpdatedCount} variants");
        }
    }
}