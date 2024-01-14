using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.POS.Kaching.Models;
using Nop.Plugin.POS.Kaching.Models.ReconciliationModels;
using Nop.Plugin.POS.Kaching.Services;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.POS.Kaching.Controller
{
    public class ReconciliationController : BasePluginController
    {
        private readonly ILogger _logger;
        private readonly IModelValidation _modelValidation;
        private readonly ILogService _logService;
        private readonly IReconciliationService _reconciliationService;
        private static string _logFileNameExtension = "ReconciliationController-";

        public ReconciliationController(ILogger logger, IModelValidation modelValidation, ILogService logService, IReconciliationService reconciliationService)
        {
            _logger = logger;
            _modelValidation = modelValidation;
            _logService = logService;
            _reconciliationService = reconciliationService;
        }

        [HttpGet]
        [Route("/api/reconciliation", Name = "Get")]
        public IActionResult Get()
        {
            return Ok("Reconciliation ready");
        }

        [HttpPost]
        [Route("/api/reconciliationwithkey", Name = "CreateReconciliationWithKey")]
        public async Task<IActionResult> PostAsync([Microsoft.AspNetCore.Mvc.FromBody] Dictionary<string, Reconciliation> reconciliation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new StringBuilder("Error in Reconciliation From Kaching");
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

            await _logger.InformationAsync("ReconciliationController called");

            var sb = new StringBuilder();

            try
            {
                await _modelValidation.Validate(reconciliation);

                var accountingFileName = await _logService.LogRawJsonAsync(JsonConvert.SerializeObject(reconciliation), _logFileNameExtension);


                bool result = await _reconciliationService.HandleReconciliationsAsync(reconciliation, accountingFileName);

                if (result)
                {
                    foreach (KeyValuePair<string, Reconciliation> kvp in reconciliation)
                    {
                        sb.Append("Reconciliation with key: " + kvp.Key + " transferred successfully");
                    }
                }
                else
                {
                    sb.Append("Call was not a reconciliation");
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                throw;
            }

            return Ok(sb.ToString());
        }
    }
}