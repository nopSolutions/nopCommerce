using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Web.Framework.Mvc
{
    /// <summary>
    /// Represents custom JsonResult with using Json converters
    /// </summary>
    public class ConverterJsonResult : JsonResult
    {
        #region Fields

        private readonly JsonConverter[] _converters;

        #endregion

        #region Ctor

        public ConverterJsonResult(params JsonConverter[] converters)
        {
            _converters = converters;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enables processing of the result of an action method
        /// </summary>
        /// <param name="context">The context within which the result is executed</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.HttpContext == null || context.HttpContext.Response == null)
                return;

            context.HttpContext.Response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : MimeTypes.ApplicationJson;
            if (ContentEncoding != null)
                context.HttpContext.Response.ContentEncoding = ContentEncoding;

            //serialize data with any converters
            if (Data != null)
                context.HttpContext.Response.Write(JsonConvert.SerializeObject(Data, _converters));
        }

        #endregion
    }
}
