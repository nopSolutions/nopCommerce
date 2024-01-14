using System;
using System.IO;
using System.Reflection;

namespace AO.Services.Logging
{
    public class LoggingUtil
    {
        public static string ExecutingFolder
        {
            get
            {
                var location = Assembly.GetExecutingAssembly().Location;
                UriBuilder uri = new UriBuilder(location);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
