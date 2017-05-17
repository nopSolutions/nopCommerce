using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Framework.Mvc
{
    public class NullJsonResult : JsonResult
    {
        public NullJsonResult() : base(null)
        {
            //TODO test
        }
    }
}
