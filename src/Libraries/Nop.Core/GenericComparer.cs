using System;
using System.Collections.Generic;

namespace Nop.Core
{
    /// <summary>
    /// This class is used to compare any 
    /// type(property) of a class for sorting.
    /// This class automatically fetches the 
    /// type of the property and compares.
    /// </summary>
    public sealed partial class GenericComparer<T> : IComparer<T>
    {
        #region Enums
        /// <summary>
        /// The sort order direction for sorting the collection
        /// </summary>
        public enum SortOrder
        {
            /// <summary>
            /// Ascending
            /// </summary>
            Ascending,
            /// <summary>
            /// Descending
            /// </summary>
            Descending
        };
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the GenericComparer class
        /// </summary>
        /// <param name="sortColumn">The property on which the collection should be sorted</param>
        /// <param name="sortingOrder">The direction of the sort</param>
        public GenericComparer(string sortColumn, SortOrder sortingOrder)
        {
            this._sortColumn = sortColumn;
            this._sortingOrder = sortingOrder;
        }
        #endregion

        #region Fields

        private readonly string _sortColumn;
        private readonly SortOrder _sortingOrder;

        #endregion

        #region Properties

        /// <summary>
        /// Column Name(public property of the class) to be sorted.
        /// </summary>
        public string SortColumn
        {
            get { return _sortColumn; }
        }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public SortOrder SortingOrder
        {
            get { return _sortingOrder; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Compare interface implementation
        /// </summary>
        /// <param name="x">custom Object</param>
        /// <param name="y">custom Object</param>
        /// <returns>int</returns>
        public int Compare(T x, T y)
        {
            var propertyInfo = typeof(T).GetProperty(_sortColumn);
            var obj1 = (IComparable)propertyInfo.GetValue(x, null);
            var obj2 = (IComparable)propertyInfo.GetValue(y, null);
            if (_sortingOrder == SortOrder.Ascending)
            {
                return (obj1.CompareTo(obj2));
            }
            else
            {
                return (obj2.CompareTo(obj1));
            }
        }
        #endregion
    }
}
