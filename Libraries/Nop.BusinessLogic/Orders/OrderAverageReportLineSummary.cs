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

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents an order average report line summary
    /// </summary>
    public partial class OrderAverageReportLineSummary : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the OrderAverageReportLineSummary class
        /// </summary>
        public OrderAverageReportLineSummary()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public OrderStatusEnum OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the sum today total
        /// </summary>
        public decimal SumTodayOrders { get; set; }

        /// <summary>
        /// Gets or sets the today count
        /// </summary>
        public int CountTodayOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum this week total
        /// </summary>
        public decimal SumThisWeekOrders { get; set; }

        /// <summary>
        /// Gets or sets the this week count
        /// </summary>
        public int CountThisWeekOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum this month total
        /// </summary>
        public decimal SumThisMonthOrders { get; set; }

        /// <summary>
        /// Gets or sets the this month count
        /// </summary>
        public int CountThisMonthOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum this year total
        /// </summary>
        public decimal SumThisYearOrders { get; set; }

        /// <summary>
        /// Gets or sets the this year count
        /// </summary>
        public int CountThisYearOrders { get; set; }

        /// <summary>
        /// Gets or sets the sum all time total
        /// </summary>
        public decimal SumAllTimeOrders { get; set; }

        /// <summary>
        /// Gets or sets the all time count
        /// </summary>
        public int CountAllTimeOrders { get; set; }

        #endregion
    }

}
