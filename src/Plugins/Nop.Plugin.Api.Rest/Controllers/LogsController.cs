using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Logging;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger _logger;

        public LogsController(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns log records
        /// </summary>
        /// <remarks>
        /// The full message on a failure carries the exception detail, so this endpoint exposes
        /// internal paths and occasionally request data. It stays behind the API key, and the read
        /// setting has to be on for it to be reachable at all.
        /// </remarks>
        // GET api/rest/logs?pageIndex=0&pageSize=20&message=&logLevelId=40&fromUtc=&toUtc=
        [HttpGet]
        public async Task<ActionResult<PagedResult<LogDto>>> GetAll(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
            [FromQuery] string? message = null,
            [FromQuery] int? logLevelId = null,
            [FromQuery] DateTime? fromUtc = null,
            [FromQuery] DateTime? toUtc = null)
        {
            var index = PagingHelper.NormalizePageIndex(pageIndex);
            var size = PagingHelper.NormalizePageSize(pageSize);

            LogLevel? level = null;
            if (logLevelId.HasValue)
            {
                if (!Enum.IsDefined(typeof(LogLevel), logLevelId.Value))
                    return BadRequest(new { error = $"Unknown log level identifier '{logLevelId}'." });

                level = (LogLevel)logLevelId.Value;
            }

            var logs = await _logger.GetAllLogsAsync(fromUtc, toUtc, message ?? string.Empty, level,
                pageIndex: index, pageSize: size);

            return Ok(logs.ToPagedResult().Map(l => l.ToDto()));
        }
    }
}
