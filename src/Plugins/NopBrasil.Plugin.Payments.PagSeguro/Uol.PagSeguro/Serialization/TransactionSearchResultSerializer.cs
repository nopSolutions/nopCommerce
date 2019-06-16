using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class TransactionSearchResultSerializer
    {
        internal const string TransactionSearchResult = "transactionSearchResult";

        private const string Date = "date";
        private const string CurrentPage = "currentPage";
        private const string TotalPages = "totalPages";

        internal static void Read(XmlReader reader, TransactionSearchResult result)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (result == null)
                throw new ArgumentNullException("result");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(TransactionSearchResultSerializer.TransactionSearchResult);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, TransactionSearchResultSerializer.TransactionSearchResult))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case TransactionSearchResultSerializer.Date:
                            result.Date = reader.ReadElementContentAsDateTime();
                            break;
                        case TransactionSearchResultSerializer.CurrentPage:
                            result.CurrentPage = reader.ReadElementContentAsInt();
                            break;
                        case TransactionSearchResultSerializer.TotalPages:
                            result.TotalPages = reader.ReadElementContentAsInt();
                            break;
                        case TransactionSummaryListSerializer.Transactions:
                            TransactionSummaryListSerializer.Read(reader, result.Items);
                            break;
                        default:
                            SerializationHelper.SkipElement(reader);
                            break;
                    }
                }
                else
                {
                    SerializationHelper.SkipNode(reader);
                }
            }
        }
    }
}
