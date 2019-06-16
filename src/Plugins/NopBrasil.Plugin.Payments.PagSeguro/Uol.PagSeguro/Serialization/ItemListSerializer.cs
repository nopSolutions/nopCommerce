using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class ItemListSerializer
    {
        internal const string Items = "items";

        internal static void Read(XmlReader reader, IList<Item> items)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (items == null)
                throw new ArgumentNullException("items");

            items.Clear();

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
            }

            reader.ReadStartElement(ItemListSerializer.Items);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, ItemListSerializer.Items))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case ItemSerializer.Item:
                            Item item = new Item();
                            ItemSerializer.Read(reader, item);
                            items.Add(item);
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
