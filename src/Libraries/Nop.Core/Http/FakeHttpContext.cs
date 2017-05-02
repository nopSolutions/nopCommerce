using Microsoft.AspNetCore.Http;

namespace Nop.Core.Http
{
    /// <summary>
    /// Represents the fake HTTP context
    /// </summary>
    public class FakeHttpContext : DefaultHttpContext
    {
        public FakeHttpContext() : base()
        {
        }
    }
}
