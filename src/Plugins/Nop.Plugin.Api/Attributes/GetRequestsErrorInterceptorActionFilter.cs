using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Models;

namespace Nop.Plugin.Api.Attributes
{
    public class GetRequestsErrorInterceptorActionFilter : ActionFilterAttribute
    {
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public GetRequestsErrorInterceptorActionFilter()
        {
            _jsonFieldsSerializer = EngineContext.Current.Resolve<IJsonFieldsSerializer>();
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null && !actionExecutedContext.ExceptionHandled)
            {
                var error = new KeyValuePair<string, List<string>>("internal_server_error",
                                                                   new List<string>
                                                                   {
                                                                       "please, contact the store owner"
                                                                   });

                actionExecutedContext.Exception = null;
                actionExecutedContext.ExceptionHandled = true;
                SetError(actionExecutedContext, error);
            }
            else if (actionExecutedContext.HttpContext.Response != null &&
                     (HttpStatusCode) actionExecutedContext.HttpContext.Response.StatusCode != HttpStatusCode.OK)
            {
                string responseBody;

                using (var streamReader = new StreamReader(actionExecutedContext.HttpContext.Response.Body))
                {
                    responseBody = streamReader.ReadToEnd();
                }

                // reset reader position.
                actionExecutedContext.HttpContext.Response.Body.Position = 0;

                var defaultWebApiErrorsModel = JsonConvert.DeserializeObject<DefaultWebApiErrorsModel>(responseBody);

                // If both are null this means that it is not the default web api error format, 
                // which means that it the error is formatted by our standard and we don't need to do anything.
                if (!string.IsNullOrEmpty(defaultWebApiErrorsModel.Message) &&
                    !string.IsNullOrEmpty(defaultWebApiErrorsModel.MessageDetail))
                {
                    var error = new KeyValuePair<string, List<string>>("lookup_error", new List<string>
                                                                                       {
                                                                                           "not found"
                                                                                       });

                    SetError(actionExecutedContext, error);
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }

        private void SetError(ActionExecutedContext actionExecutedContext, KeyValuePair<string, List<string>> error)
        {
            var bindingError = new Dictionary<string, List<string>>
                               {
                                   {
                                       error.Key, error.Value
                                   }
                               };

            var errorsRootObject = new ErrorsRootObject
                                   {
                                       Errors = bindingError
                                   };

            var errorJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);
            var response = new ContentResult();
            response.Content = errorJson;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            actionExecutedContext.Result = response;
            
        }
    }
}
