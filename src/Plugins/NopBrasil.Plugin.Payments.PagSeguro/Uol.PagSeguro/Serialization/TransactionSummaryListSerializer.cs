using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class TransactionSummaryListSerializer
    {
        internal const string Transactions = "transactions";

        internal static void Read(XmlReader reader, IList<TransactionSummary> transactions)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (transactions == null)
                throw new ArgumentNullException("transactions");

            transactions.Clear();

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
            }

            reader.ReadStartElement(TransactionSummaryListSerializer.Transactions);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, TransactionSummaryListSerializer.Transactions))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case TransactionSerializerHelper.Transaction:
                            TransactionSummary transaction = new TransactionSummary();
                            TransactionSummarySerializer.Read(reader, transaction);
                            transactions.Add(transaction);
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

        internal static void Write(XmlWriter writer, IList<Item> items)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (items == null)
                throw new ArgumentNullException("items");

            writer.WriteStartElement(ItemListSerializer.Items);
            foreach (Item item in items)
            {
                ItemSerializer.Write(writer, item);
            }
            writer.WriteEndElement();
        }
    }
}
