//using System;
//using System.Text.RegularExpressions;

using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    public partial class CustomNumberFormatter : ICustomNumberFormatter
    {
        #region Fields

        private OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public CustomNumberFormatter(OrderSettings orderSettings)
        {
            _orderSettings = orderSettings;
        }

        #endregion

        #region Methods

        public virtual string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest)
        {
            var customNumber = string.Empty;

            if (string.IsNullOrEmpty(_orderSettings.ReturnRequestNumberMask))
            {
                customNumber = returnRequest.Id.ToString();
            }
            else
            {
                customNumber = _orderSettings.ReturnRequestNumberMask
                    .Replace("{ID}", returnRequest.Id.ToString())
                    .Replace("{YYYY}", returnRequest.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", returnRequest.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", returnRequest.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", returnRequest.CreatedOnUtc.ToString("dd"));

                ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
                ////mask for Id example {#:00000000}
                //var rgx = new Regex(@"{#:\d+}");
                //var match = rgx.Match(customNumber);
                //var maskForReplase = match.Value;
                //
                //rgx = new Regex(@"\d+");
                //match = rgx.Match(maskForReplase);
                //
                //var formatValue = match.Value;
                //if(!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
                //    customNumber = customNumber.Replace(maskForReplase, returnRequest.Id.ToString(formatValue));
                //else
                //    customNumber = customNumber.Insert(0, string.Format("{0}-", returnRequest.Id));
            }

            return customNumber;
        }

        public virtual string GenerateOrderCustomNumber(Order order)
        {
            if (string.IsNullOrEmpty(_orderSettings.CustomOrderNumberMask))
                return order.Id.ToString();

            var customNumber = _orderSettings.CustomOrderNumberMask
                .Replace("{ID}", order.Id.ToString())
                .Replace("{YYYY}", order.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", order.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", order.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", order.CreatedOnUtc.ToString("dd")).Trim();

            ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
            ////mask for Id example {#:00000000}
            //var rgx = new Regex(@"{#:\d+}");
            //var match = rgx.Match(customNumber);
            //var maskForReplase = match.Value;

            //rgx = new Regex(@"\d+");
            //match = rgx.Match(maskForReplase);

            //var formatValue = match.Value;
            //if (!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
            //    customNumber = customNumber.Replace(maskForReplase, order.Id.ToString(formatValue));
            //else
            //    customNumber = customNumber.Insert(0, string.Format("{0}-", order.Id));


            return customNumber;
        }

        #endregion
    }
}