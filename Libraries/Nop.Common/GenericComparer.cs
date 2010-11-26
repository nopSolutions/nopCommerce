//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace NopSolutions.NopCommerce.Common
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
            this.sortColumn = sortColumn;
            this.sortingOrder = sortingOrder;
        }
        #endregion

        #region Fields

        private string sortColumn;
        private SortOrder sortingOrder;

        #endregion

        #region Properties

        /// <summary>
        /// Column Name(public property of the class) to be sorted.
        /// </summary>
        public string SortColumn
        {
            get { return sortColumn; }
        }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public SortOrder SortingOrder
        {
            get { return sortingOrder; }
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
            var propertyInfo = typeof(T).GetProperty(sortColumn);
            var obj1 = (IComparable)propertyInfo.GetValue(x, null);
            var obj2 = (IComparable)propertyInfo.GetValue(y, null);
            if (sortingOrder == SortOrder.Ascending)
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
