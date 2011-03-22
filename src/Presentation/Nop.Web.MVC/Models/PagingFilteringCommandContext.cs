using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.MVC.Models
{
    public class PagingFilteringCommandAttribute : FilterAttribute, IActionFilter
    {
        #region Constructors (2)

        public PagingFilteringCommandAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public PagingFilteringCommandAttribute()
            : this("command")
        {
        }

        #endregion Constructors

        #region Properties (1)

        public string ParameterName { get; set; }

        #endregion Properties

        #region Methods (1)

        // Private Methods (1) 

        private PagingFilteringCommand BuildCommand(RequestContext requestContext)
        {
            var command = new PagingFilteringCommand();
            return command;
            //decimal? from = null;
            //decimal? to = null;
            //foreach (string key in requestContext.HttpContext.Request.Form.Keys)
            //{
            //    if (!string.IsNullOrEmpty(key))
            //    {
            //        string value = requestContext.HttpContext.Request.Form[key].ToLower();
            //        if (key.StartsWith("attr"))
            //        {
            //            var attrFilter =
            //                SpecificationAttributeManager.GetSpecificationAttributeOptionById(int.Parse(value));
            //            //attrFilter.SpecificationAttributeId = int.Parse(key.Replace("attr", ""));
            //            //attrFilter.SpecificationAttributeOptionId = ;
            //            _filters.Add(attrFilter);
            //        }
            //        else if (key.ToLower().Equals("categoryid"))
            //        {
            //            _categories.Add(int.Parse(value));
            //        }
            //        else if (key.ToLower().Equals("manufacturerid"))
            //        {
            //            _manufacturers.Add(int.Parse(value));
            //        }
            //        else if (key.ToLower().Equals("sortby"))
            //        {
            //            switch (value)
            //            {
            //                case Globals.SORTBY_RELEVANCE:
            //                    _productSortBy = ProductsSortBy.Relevance;
            //                    break;
            //                case Globals.SORTBY_HIGHEST_RATED:
            //                    _productSortBy = ProductsSortBy.HighestRated;
            //                    break;
            //                case Globals.SORTBY_BEST_SELLING:
            //                    _productSortBy = ProductsSortBy.DisplayOrder;
            //                    break;
            //                case Globals.SORTBY_PRICE_LOW_TO_HIGH:
            //                    _productSortBy = ProductsSortBy.PriceAsc;
            //                    break;
            //                case Globals.SORTBY_PRICE_HIGH_TO_LOW:
            //                    _productSortBy = ProductsSortBy.PriceDesc;
            //                    break;
            //                case Globals.SORTBY_BRAND_AZ:
            //                    _productSortBy = ProductsSortBy.BrandAsc;
            //                    break;
            //                case Globals.SORTBY_BRAIN_ZA:
            //                    _productSortBy = ProductsSortBy.BrandDesc;
            //                    break;
            //                default:
            //                    _productSortBy = ProductsSortBy.Relevance;
            //                    break;
            //            }
            //        }
            //        else if (key.ToLower().Equals("from"))
            //        {
            //            decimal i = 0;
            //            if (decimal.TryParse(value, out i))
            //            {
            //                from = i;
            //            }
            //        }
            //        else if (key.ToLower().Equals("to"))
            //        {
            //            decimal i = 0;
            //            if (decimal.TryParse(value, out i))
            //            {
            //                to = i;
            //            }
            //        }
            //        else if (key.ToLower().Equals("searchterms"))
            //        {
            //            keyWords = HttpUtility.UrlDecode(value);
            //        }
            //    }
            //}
            //if (from.HasValue || to.HasValue)
            //{
            //    _priceRange = new PriceRange { From = from, To = to };
            //}
        }

        #endregion Methods



        #region IActionFilter

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.ActionParameters[ParameterName] = BuildCommand(filterContext.RequestContext);
        }

        #endregion
    }

    public class PagingFilteringCommand
    {
        public PagingFilteringCommand()
        {
            Specs = new List<int>();
            ProductSorting = ProductSortingEnum.Position;
        }

        public int CategoryId {get;set;}
        public int ManufacturerId {get;set;}
        public ProductSortingEnum ProductSorting {get;set;}
        public decimal? PriceMin {get;set;}
        public decimal? PriceMax {get;set;}
        public IList<int> Specs {get;set;}
        public int PageIndex {get;set;}
        public int PageSize{get;set;}
    }
}