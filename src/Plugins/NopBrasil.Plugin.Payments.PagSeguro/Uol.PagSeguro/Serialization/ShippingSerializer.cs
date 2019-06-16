using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class ShippingSerializer
    {
        internal const string Shipping = "shipping";

        private const string ShippingType = "type";
        private const string Cost = "cost";

        internal static void Read(XmlReader reader, Shipping shipping)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (shipping == null)
                throw new ArgumentNullException("shipping");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(ShippingSerializer.Shipping);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, ShippingSerializer.Shipping))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case ShippingSerializer.ShippingType:
                            shipping.ShippingType = reader.ReadElementContentAsInt();
                            break;
                        case ShippingSerializer.Cost:
                            shipping.Cost = reader.ReadElementContentAsDecimal();
                            break;
                        case AddressSerializer.Address:
                            Address address = new Address();
                            AddressSerializer.Read(reader, address);
                            shipping.Address = address;
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

        internal static void Write(XmlWriter writer, Shipping shipping)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (shipping == null)
                throw new ArgumentNullException("shipping");

            writer.WriteStartElement(ShippingSerializer.Shipping);

            SerializationHelper.WriteElementStringNotNull(writer, ShippingSerializer.ShippingType, shipping.ShippingType);
            SerializationHelper.WriteElementStringNotNull(writer, ShippingSerializer.Cost, shipping.Cost);

            if (shipping.Address != null)
            {
                AddressSerializer.Write(writer, shipping.Address);
            }

            writer.WriteEndElement();
        }
    }
}
