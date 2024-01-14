using System.Collections.Generic;

namespace AO.Services.Services.SyncjobServices
{
    public class CrawlResult
    {
        public IList<string> ValidEndpoints { get; set; }
        public string CrawlErrors { get; set; }
    }
}
