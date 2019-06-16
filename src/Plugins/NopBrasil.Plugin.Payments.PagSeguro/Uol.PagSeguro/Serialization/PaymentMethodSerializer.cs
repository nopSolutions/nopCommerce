using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class PaymentMethodSerializer
    {
        internal const string PaymentMethod = "paymentMethod";

        private const string PaymentMethodType = "type";
        private const string PaymentMethodCode = "code";

        internal static void Read(XmlReader reader, PaymentMethod paymentMethod)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(PaymentMethodSerializer.PaymentMethod);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, PaymentMethodSerializer.PaymentMethod))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case PaymentMethodSerializer.PaymentMethodType:
                            paymentMethod.PaymentMethodType = reader.ReadElementContentAsInt();
                            break;
                        case PaymentMethodSerializer.PaymentMethodCode:
                            paymentMethod.PaymentMethodCode = reader.ReadElementContentAsInt();
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
