using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class TransactionSerializerHelper
    {
        internal const string Transaction = "transaction";
        internal const string Code = "code";
        internal const string Date = "date";
        internal const string Reference = "reference";
        internal const string TransactionType = "type";
        internal const string TransactionStatus = "status";
        internal const string GrossAmount = "grossAmount";
        internal const string DiscountAmount = "discountAmount";
        internal const string FeeAmount = "feeAmount";
        internal const string NetAmount = "netAmount";
        internal const string ExtraAmount = "extraAmount";
        internal const string LastEventDate = "lastEventDate";
        internal const string InstallmentCount = "installmentCount";
    }
}
