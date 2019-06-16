using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class ItemSerializer
    {
        internal const string Item = "item";

        private const string Id = "id";
        private const string Description = "description";
        private const string Quantity = "quantity";
        private const string Amount = "amount";
        private const string Weight = "weight";
        private const string ShippingCost = "shippingCost";

        internal static void Read(XmlReader reader, Item item)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (item == null)
                throw new ArgumentNullException("item");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(ItemSerializer.Item);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, ItemSerializer.Item))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case ItemSerializer.Id:
                            item.Id = reader.ReadElementContentAsString();
                            break;
                        case ItemSerializer.Description:
                            item.Description = reader.ReadElementContentAsString();
                            break;
                        case ItemSerializer.Quantity:
                            item.Quantity = reader.ReadElementContentAsInt();
                            break;
                        case ItemSerializer.Amount:
                            item.Amount = reader.ReadElementContentAsDecimal();
                            break;
                        case ItemSerializer.Weight:
                            item.Weight = reader.ReadElementContentAsLong();
                            break;
                        case ItemSerializer.ShippingCost:
                            item.ShippingCost = reader.ReadElementContentAsDecimal();
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

        internal static void Write(XmlWriter writer, Item item)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (item == null)
                throw new ArgumentNullException("item");

            writer.WriteStartElement(ItemSerializer.Item);
            writer.WriteElementString(ItemSerializer.Id, item.Id);
            writer.WriteElementString(ItemSerializer.Description, item.Description);
            SerializationHelper.WriteElementString(writer, ItemSerializer.Quantity, item.Quantity);
            SerializationHelper.WriteElementString(writer, ItemSerializer.Amount, item.Amount);
            SerializationHelper.WriteElementStringNotNull(writer, ItemSerializer.Weight, item.Weight);
            SerializationHelper.WriteElementStringNotNull(writer, ItemSerializer.ShippingCost, item.ShippingCost);
            writer.WriteEndElement();
        }
    }
}
