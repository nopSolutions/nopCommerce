using Microsoft.AspNetCore.Http;
using System.Net;

namespace Nop.Web.Framework.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string NullIpAddress = "::1";

        public static bool IsLocal(this HttpRequest req)
        {
            //source: https://stackoverflow.com/a/41242493/7860424
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress.IsSet())
            {
                //We have a remote address set up
                return connection.LocalIpAddress.IsSet()
                    //Is local is same as remote, then we are local
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    //else we are remote if the remote IP address is not a loopback address
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return true;
        }

        private static bool IsSet(this IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }
    }
}
