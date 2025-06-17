using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Models
{
    public class SearchResultModel
    {
        [JsonPropertyName("results")]
        public List<SearchSpringProductModel> Results { get; set; } = new();

        [JsonPropertyName("page")]
        public int PageNumber { get; set; }

        [JsonPropertyName("resultsPerPage")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling((double)TotalResults / PageSize)
            : 0;

        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("facets")]
        public Dictionary<string, FacetDetail> Facets { get; set; } = new();

        [JsonPropertyName("sortOptions")]
        public List<SortOption> SortOptions { get; set; } = new();

        [JsonPropertyName("redirectResponse")]
        public string RedirectResponse { get; set; }
    }

    public class FacetDetail
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("multiple")]
        public string Multiple { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("collapse")]
        public bool Collapse { get; set; }

        [JsonPropertyName("facet_active")]
        public bool FacetActive { get; set; }

        [JsonPropertyName("hierarchyDelimiter")]
        public string HierarchyDelimiter { get; set; }

        [JsonPropertyName("values")]
        public List<FacetValue> Values { get; set; } = new();

    }

    public class FacetValue
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }

    public class SortOption
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("direction")]
        public string Direction { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}
