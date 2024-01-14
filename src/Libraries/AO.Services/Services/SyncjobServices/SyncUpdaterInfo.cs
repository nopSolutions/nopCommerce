using System.Collections.Generic;

namespace AO.Services.Services.SyncjobServices
{
    public class SyncUpdaterInfo
    {
        public string UpdaterName { get; set; }

        public int MinStockCount { get; set; }

        public IList<string> CrawlerEndpoints { get; set; }

        public string NodeDefinitionForFullDescription { get; set; }

        public string NodeDefinitionForImages { get; set; }
    }
}