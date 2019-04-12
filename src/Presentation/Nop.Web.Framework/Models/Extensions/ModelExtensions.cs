using System;
using System.Collections.Generic;
using Nop.Core;

namespace Nop.Web.Framework.Models.Extensions
{
    /// <summary>
    /// Represents model extensions
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Convert list to paged list according to paging request
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">List of objects</param>
        /// <param name="pagingRequestModel">Paging request model</param>
        /// <returns>Paged list</returns>
        public static IPagedList<T> ToPagedList<T>(this IList<T> list, IPagingRequestModel pagingRequestModel)
        {
            return new PagedList<T>(list, pagingRequestModel.Page - 1, pagingRequestModel.PageSize);
        }

        /// <summary>
        /// Prepare passed list model to display in a grid
        /// </summary>
        /// <typeparam name="TListModel">List model type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TObject">Object type</typeparam>
        /// <param name="listModel">List model</param>
        /// <param name="searchModel">Search model</param>
        /// <param name="objectList">Paged list of objects</param>
        /// <param name="dataFillFunction">Function to populate model data</param>
        /// <returns>List model</returns>
        public static TListModel PrepareToGrid<TListModel, TModel, TObject>(this TListModel listModel,
            BaseSearchModel searchModel, IPagedList<TObject> objectList, Func<IEnumerable<TModel>> dataFillFunction) 
            where TListModel : BasePagedListModel<TModel>
            where TModel : BaseNopModel
        {
            if (listModel == null)
                throw new ArgumentNullException(nameof(listModel));

            listModel.Data = dataFillFunction?.Invoke();
            listModel.Draw = searchModel?.Draw;
            listModel.RecordsTotal = objectList?.TotalCount ?? 0;
            listModel.RecordsFiltered = objectList?.TotalCount ?? 0;

            return listModel;
        }
    }
}