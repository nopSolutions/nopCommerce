using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog; 
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Filtering and paging model for catalog
    /// </summary>
    public partial record CatalogPagingFilteringModel : BasePageableModel
    {
        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public CatalogPagingFilteringModel()
        {
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();
            PageSizeOptions = new List<SelectListItem>();

            PriceRangeFilter = new PriceRangeFilterModel();
            SpecificationFilter = new SpecificationFilterModel();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Specification filter model
        /// </summary>
        public SpecificationFilterModel SpecificationFilter { get; set; }

        /// <summary>
        /// A value indicating whether product sorting is allowed
        /// </summary>
        public bool AllowProductSorting { get; set; }

        /// <summary>
        /// Available sort options
        /// </summary>
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        /// <summary>
        /// A value indicating whether customers are allowed to change view mode
        /// </summary>
        public bool AllowProductViewModeChanging { get; set; }
        /// <summary>
        /// Available view mode options
        /// </summary>
        public IList<SelectListItem> AvailableViewModes { get; set; }

        /// <summary>
        /// A value indicating whether customers are allowed to select page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }
        /// <summary>
        /// Available page size options
        /// </summary>
        public IList<SelectListItem> PageSizeOptions { get; set; }

        /// <summary>
        /// Order by
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// Product sorting
        /// </summary>
        public string ViewMode { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// Price range filter model
        /// </summary>
        public partial record PriceRangeFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "price";

            #endregion 

            #region Ctor

            /// <summary>
            /// Ctor
            /// </summary>
            public PriceRangeFilterModel()
            {
                Items = new List<PriceRangeFilterItem>();
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Gets parsed price ranges
            /// </summary>
            /// <param name="priceRangesStr">Price ranges in string format</param>
            /// <returns>Price ranges</returns>
            protected virtual IList<PriceRange> GetPriceRangeList(string priceRangesStr)
            {
                var priceRanges = new List<PriceRange>();
                if (string.IsNullOrWhiteSpace(priceRangesStr))
                    return priceRanges;
                var rangeArray = priceRangesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var str1 in rangeArray)
                {
                    var fromTo = str1.Trim().Split(new[] { '-' });

                    decimal? from = null;
                    if (!string.IsNullOrEmpty(fromTo[0]) && !string.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));

                    decimal? to = null;
                    if (!string.IsNullOrEmpty(fromTo[1]) && !string.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                    priceRanges.Add(new PriceRange { From = from, To = to });
                }
                return priceRanges;
            }

            /// <summary>
            /// Exclude query string parameters
            /// </summary>
            /// <param name="url">URL</param>
            /// <param name="webHelper">Web helper</param>
            /// <returns>New URL</returns>
            protected virtual Task<string> ExcludeQueryStringParamsAsync(string url, IWebHelper webHelper)
            {
                //comma separated list of parameters to exclude
                const string excludedQueryStringParams = "pagenumber";
                var excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var exclude in excludedQueryStringParamsSplitted)
                    url = webHelper.RemoveQueryString(url, exclude);
                
                return Task.FromResult(url);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Get selected price range
            /// </summary>
            /// <param name="webHelper">Web helper</param>
            /// <param name="priceRangesStr">Price ranges in string format</param>
            /// <returns>Price ranges</returns>
            public virtual Task<PriceRange> GetSelectedPriceRangeAsync(IWebHelper webHelper, string priceRangesStr)
            {
                var range = webHelper.QueryString<string>(QUERYSTRINGPARAM);

                if (string.IsNullOrEmpty(range))
                    return Task.FromResult<PriceRange>(null);

                var fromTo = range.Trim().Split(new[] { '-' });
                if (fromTo.Length == 2)
                {
                    decimal? from = null;
                    if (!string.IsNullOrEmpty(fromTo[0]) && !string.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));
                    decimal? to = null;
                    if (!string.IsNullOrEmpty(fromTo[1]) && !string.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

                    var priceRangeList = GetPriceRangeList(priceRangesStr);
                    foreach (var pr in priceRangeList)
                    {
                        if (pr.From == from && pr.To == to)
                            return Task.FromResult(pr);
                    }
                }

                return Task.FromResult<PriceRange>(null);
            }

            /// <summary>
            /// Load price range filters
            /// </summary>
            /// <param name="priceRangeStr">Price range in string format</param>
            /// <param name="webHelper">Web helper</param>
            /// <param name="priceFormatter">Price formatter</param>
            public virtual async Task LoadPriceRangeFiltersAsync(string priceRangeStr, IWebHelper webHelper, IPriceFormatter priceFormatter)
            {
                var priceRangeList = GetPriceRangeList(priceRangeStr);
                if (priceRangeList.Any())
                {
                    Enabled = true;

                    var selectedPriceRange = await GetSelectedPriceRangeAsync(webHelper, priceRangeStr);

                    Items = await priceRangeList.SelectAwait(async x =>
                    {
                        //from&to
                        var item = new PriceRangeFilterItem();
                        if (x.From.HasValue)
                            item.From = await priceFormatter.FormatPriceAsync(x.From.Value, true, false);
                        if (x.To.HasValue)
                            item.To = await priceFormatter.FormatPriceAsync(x.To.Value, true, false);
                        var fromQuery = string.Empty;
                        if (x.From.HasValue)
                            fromQuery = x.From.Value.ToString(new CultureInfo("en-US"));
                        var toQuery = string.Empty;
                        if (x.To.HasValue)
                            toQuery = x.To.Value.ToString(new CultureInfo("en-US"));

                        //is selected?
                        if (selectedPriceRange != null
                            && selectedPriceRange.From == x.From
                            && selectedPriceRange.To == x.To)
                            item.Selected = true;

                        //filter URL
                        var url = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM, $"{fromQuery}-{toQuery}");
                        url = await ExcludeQueryStringParamsAsync(url, webHelper);
                        item.FilterUrl = url;

                        return item;
                    }).ToListAsync();

                    if (selectedPriceRange != null)
                    {
                        //remove filter URL
                        var url = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                        url = await ExcludeQueryStringParamsAsync(url, webHelper);
                        RemoveFilterUrl = url;
                    }
                }
                else
                {
                    Enabled = false;
                }
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether filtering is enabled
            /// </summary>
            public bool Enabled { get; set; }
            /// <summary>
            /// Filter items
            /// </summary>
            public IList<PriceRangeFilterItem> Items { get; set; }
            /// <summary>
            /// URL of "remove filters" button
            /// </summary>
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        /// <summary>
        /// Price range filter item
        /// </summary>
        public partial record PriceRangeFilterItem : BaseNopModel
        {
            /// <summary>
            /// From
            /// </summary>
            public string From { get; set; }
            /// <summary>
            /// To
            /// </summary>
            public string To { get; set; }
            /// <summary>
            /// Filter URL
            /// </summary>
            public string FilterUrl { get; set; }
            /// <summary>
            /// Is selected?
            /// </summary>
            public bool Selected { get; set; }
        }

        /// <summary>
        /// Specification filter model
        /// </summary>
        public partial record SpecificationFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "specs";

            #endregion

            #region Ctor

            /// <summary>
            /// Ctor
            /// </summary>
            public SpecificationFilterModel()
            {
                AlreadyFilteredItems = new List<SpecificationFilterItem>();
                NotFilteredItems = new List<SpecificationFilterItem>();
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Exclude query string parameters
            /// </summary>
            /// <param name="url">URL</param>
            /// <param name="webHelper">Web helper</param>
            /// <returns>New URL</returns>
            protected virtual Task<string> ExcludeQueryStringParamsAsync(string url, IWebHelper webHelper)
            {
                //comma separated list of parameters to exclude
                const string excludedQueryStringParams = "pagenumber";
                var excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var exclude in excludedQueryStringParamsSplitted)
                    url = webHelper.RemoveQueryString(url, exclude);
                
                return Task.FromResult(url);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Get IDs of already filtered specification options
            /// </summary>
            /// <param name="webHelper">Web helper</param>
            /// <returns>IDs</returns>
            public virtual Task<List<int>> GetAlreadyFilteredSpecOptionIdsAsync(IWebHelper webHelper)
            {
                var result = new List<int>();

                var alreadyFilteredSpecsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (string.IsNullOrWhiteSpace(alreadyFilteredSpecsStr))
                    return Task.FromResult(result);

                foreach (var spec in alreadyFilteredSpecsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int.TryParse(spec.Trim(), out var specId);
                    if (!result.Contains(specId))
                        result.Add(specId);
                }
                return Task.FromResult(result);
            }

            /// <summary>
            /// Prepare model
            /// </summary>
            /// <param name="alreadyFilteredSpecOptionIds">IDs of already filtered specification options</param>
            /// <param name="filterableSpecificationAttributeOptionIds">IDs of filterable specification options</param>
            /// <param name="specificationAttributeService"></param>
            /// <param name="localizationService">Localization service</param>
            /// <param name="webHelper">Web helper</param>
            /// <param name="workContext">Work context</param>
            /// <param name="staticCacheManager">Cache manager</param>
            public virtual async Task PrepareSpecsFiltersAsync(IList<int> alreadyFilteredSpecOptionIds,
                int[] filterableSpecificationAttributeOptionIds,
                    ISpecificationAttributeService specificationAttributeService, ILocalizationService localizationService,
                IWebHelper webHelper, IWorkContext workContext, IStaticCacheManager staticCacheManager)
            {
                Enabled = false;

                var cacheKey = staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SpecsFilterModelKey, filterableSpecificationAttributeOptionIds, await workContext.GetWorkingLanguageAsync());

                var allOptions = await specificationAttributeService.GetSpecificationAttributeOptionsByIdsAsync(filterableSpecificationAttributeOptionIds);
                var allFilters = await staticCacheManager.GetAsync(cacheKey, async () => await allOptions.SelectAwait(async sao =>
                {
                    var specAttribute = await specificationAttributeService.GetSpecificationAttributeByIdAsync(sao.SpecificationAttributeId);

                    return new SpecificationAttributeOptionFilter
                    {
                        SpecificationAttributeId = specAttribute.Id,
                        SpecificationAttributeName = await localizationService.GetLocalizedAsync(specAttribute, x => x.Name, (await workContext.GetWorkingLanguageAsync()).Id),
                        SpecificationAttributeDisplayOrder = specAttribute.DisplayOrder,
                        SpecificationAttributeOptionId = sao.Id,
                        SpecificationAttributeOptionName = await localizationService.GetLocalizedAsync(sao, x => x.Name, (await workContext.GetWorkingLanguageAsync()).Id),
                        SpecificationAttributeOptionColorRgb = sao.ColorSquaresRgb,
                        SpecificationAttributeOptionDisplayOrder = sao.DisplayOrder
                    };
                }).ToListAsync());

                if (!allFilters.Any())
                    return;

                //sort loaded options
                allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeName)
                    .ThenBy(saof => saof.SpecificationAttributeOptionDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();

                //prepare the model properties
                Enabled = true;
                var removeFilterUrl = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                RemoveFilterUrl = await ExcludeQueryStringParamsAsync(removeFilterUrl, webHelper);

                //get already filtered specification options
                var alreadyFilteredOptions = allFilters.Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeOptionId));
                AlreadyFilteredItems = alreadyFilteredOptions.Select(x =>
                    new SpecificationFilterItem
                    {
                        SpecificationAttributeName = x.SpecificationAttributeName,
                        SpecificationAttributeOptionName = x.SpecificationAttributeOptionName,
                        SpecificationAttributeOptionColorRgb = x.SpecificationAttributeOptionColorRgb
                    }).ToList();

                //get not filtered specification options
                NotFilteredItems = await allFilters.Except(alreadyFilteredOptions).SelectAwait(async x =>
                {
                    //filter URL
                    var alreadyFiltered = alreadyFilteredSpecOptionIds.Concat(new List<int> { x.SpecificationAttributeOptionId });
                    var filterUrl = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM,
                        alreadyFiltered.OrderBy(id => id).Select(id => id.ToString()).ToArray());

                    return new SpecificationFilterItem
                    {
                        SpecificationAttributeName = x.SpecificationAttributeName,
                        SpecificationAttributeOptionName = x.SpecificationAttributeOptionName,
                        SpecificationAttributeOptionColorRgb = x.SpecificationAttributeOptionColorRgb,
                        FilterUrl = await ExcludeQueryStringParamsAsync(filterUrl, webHelper)
                    };
                }).ToListAsync();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether filtering is enabled
            /// </summary>
            public bool Enabled { get; set; }
            /// <summary>
            /// Already filtered items
            /// </summary>
            public IList<SpecificationFilterItem> AlreadyFilteredItems { get; set; }
            /// <summary>
            /// Not filtered yet items
            /// </summary>
            public IList<SpecificationFilterItem> NotFilteredItems { get; set; }
            /// <summary>
            /// URL of "remove filters" button
            /// </summary>
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        /// <summary>
        /// Specification filter item
        /// </summary>
        public partial record SpecificationFilterItem : BaseNopModel
        {
            /// <summary>
            /// Specification attribute name
            /// </summary>
            public string SpecificationAttributeName { get; set; }
            /// <summary>
            /// Specification attribute option name
            /// </summary>
            public string SpecificationAttributeOptionName { get; set; }
            /// <summary>
            /// Specification attribute option color (RGB)
            /// </summary>
            public string SpecificationAttributeOptionColorRgb { get; set; }
            /// <summary>
            /// Filter URL
            /// </summary>
            public string FilterUrl { get; set; }
        }

        #endregion
    }
}