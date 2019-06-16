using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class TransactionSerializer
    {
        internal static void Read(XmlReader reader, Transaction transaction)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(TransactionSerializerHelper.Transaction);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, TransactionSerializerHelper.Transaction))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case TransactionSerializerHelper.Code:
                            transaction.Code = reader.ReadElementContentAsString();
                            break;
                        case TransactionSerializerHelper.Date:
                            transaction.Date = reader.ReadElementContentAsDateTime();
                            break;
                        case TransactionSerializerHelper.Reference:
                            transaction.Reference = reader.ReadElementContentAsString();
                            break;
                        case TransactionSerializerHelper.TransactionType:
                            transaction.TransactionType = reader.ReadElementContentAsInt();
                            break;
                        case TransactionSerializerHelper.TransactionStatus:
                            transaction.TransactionStatus = reader.ReadElementContentAsInt();
                            break;
                        case TransactionSerializerHelper.GrossAmount:
                            transaction.GrossAmount = reader.ReadElementContentAsDecimal();
                            break;
                        case TransactionSerializerHelper.DiscountAmount:
                            transaction.DiscountAmount = reader.ReadElementContentAsDecimal();
                            break;
                        case TransactionSerializerHelper.FeeAmount:
                            transaction.FeeAmount = reader.ReadElementContentAsDecimal();
                            break;
                        case TransactionSerializerHelper.NetAmount:
                            transaction.NetAmount = reader.ReadElementContentAsDecimal();
                            break;
                        case TransactionSerializerHelper.ExtraAmount:
                            transaction.ExtraAmount = reader.ReadElementContentAsDecimal();
                            break;
                        case TransactionSerializerHelper.LastEventDate:
                            transaction.LastEventDate = reader.ReadElementContentAsDateTime();
                            break;
                        case TransactionSerializerHelper.InstallmentCount:
                            transaction.InstallmentCount = reader.ReadElementContentAsInt();
                            break;
                        case PaymentMethodSerializer.PaymentMethod:
                            PaymentMethod paymentMethod = new PaymentMethod();
                            PaymentMethodSerializer.Read(reader, paymentMethod);
                            transaction.PaymentMethod = paymentMethod;
                            break;
                        case ItemListSerializer.Items:
                            ItemListSerializer.Read(reader, transaction.Items);
                            break;
                        case SenderSerializer.Sender:
                            Sender sender = new Sender();
                            SenderSerializer.Read(reader, sender);
                            transaction.Sender = sender;
                            break;
                        case ShippingSerializer.Shipping:
                            Shipping shipping = new Shipping();
                            ShippingSerializer.Read(reader, shipping);
                            transaction.Shipping = shipping;
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
