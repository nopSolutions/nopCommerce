using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Web.Framework.Mvc
{
    public class NullJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            //we do it as described here - http://stackoverflow.com/questions/15939944/jquery-post-json-fails-when-returning-null-from-asp-net-mvc

            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : MimeTypes.ApplicationJson;
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            this.Data = null;

            //If you need special handling, you can call another form of SerializeObject below
            var serializedObject = JsonConvert.SerializeObject(Data, Formatting.Indented);
            response.Write(serializedObject);
        }
    }
}
