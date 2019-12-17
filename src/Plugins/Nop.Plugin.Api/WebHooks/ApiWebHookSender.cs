//using System.Collections.Generic;
//using Newtonsoft.Json.Linq;

//namespace Nop.Plugin.Api.WebHooks
//{
//    using Microsoft.AspNet.WebHooks;
//    using Microsoft.AspNet.WebHooks.Diagnostics;

//    public class ApiWebHookSender : DataflowWebHookSender
//    {
//        private const string WebHookIdKey = "WebHookId";

//        public ApiWebHookSender(ILogger logger) : base(logger)
//        {
//        }

//        /// <inheritdoc />
//        protected override JObject CreateWebHookRequestBody(WebHookWorkItem workItem)
//        {
//            JObject data = base.CreateWebHookRequestBody(workItem);

//            Dictionary<string, object> body = data.ToObject<Dictionary<string, object>>();

//            // The web hook id is added to the web hook body.
//            // This is required in order to properly validate the web hook.
//            // When a web hook is created, it is created with a Secred field.
//            // The web hook id and the secret can be stored in the client's database, so that when a web hook is received
//            // it can be validated with the secret in the database.
//            // This ensures that the web hook is send from the proper location and that it's content were not tampered with.
//            body[WebHookIdKey] = workItem.WebHook.Id;

//            return JObject.FromObject(body);
//        }
//    }
//}
