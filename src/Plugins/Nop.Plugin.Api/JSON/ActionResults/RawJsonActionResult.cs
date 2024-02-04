using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Nop.Plugin.Api.JSON.ActionResults
{
    // TODO: Move to BaseApiController as method.
    public class RawJsonActionResult : ActionResult
    {
        private readonly string _jsonString;

        public RawJsonActionResult(object value)
        {
            if (value != null)
            {
                _jsonString = value.ToString();
            }
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;

            response.StatusCode = 200;
            response.ContentType = "application/json";

            await response.WriteAsync(_jsonString);
        }
    }
}
