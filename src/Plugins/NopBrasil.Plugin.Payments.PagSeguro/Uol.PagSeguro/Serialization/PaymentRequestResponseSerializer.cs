using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class PaymentRequestResponseSerializer
    {
        private const string Code = "code";
        private const string Date = "date";

        internal static void Read(XmlReader reader, PaymentRequestResponse paymentResponse)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (paymentResponse == null)
                throw new ArgumentNullException("paymentResponse");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            string rootElement = reader.Name;
            reader.ReadStartElement();
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, rootElement))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case PaymentRequestResponseSerializer.Date:
                            paymentResponse.RegistrationDate = reader.ReadElementContentAsDateTime();
                            break;
                        case PaymentRequestResponseSerializer.Code:
                            paymentResponse.Code = reader.ReadElementContentAsString();
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
