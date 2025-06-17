using System.Collections.Generic;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Models
{
    public class SearchSpringModel
    {
        /// <summary>
        /// Searchspring site ID
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// The current search query
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// List of filters applied (e.g., brand, price, etc.)
        /// </summary>
        public List<string> AppliedFilters { get; set; }

        /// <summary>
        /// Indicates whether Searchspring JavaScript should be injected
        /// </summary>
        public bool EnableSearchspring { get; set; }

        /// <summary>
        /// Additional config parameters for JavaScript integration
        /// </summary>
        public Dictionary<string, string> JsConfigParameters { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int ResultsPerPage { get; set; }
        public int TotalResults { get; set; }

        public SearchSpringModel()
        {
            AppliedFilters = new List<string>();
            JsConfigParameters = new Dictionary<string, string>();
        }
    }
}
