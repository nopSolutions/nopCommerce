using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class PhoneSerializer
    {
        internal const string Phone = "phone";

        private const string AreaCode = "areaCode";
        private const string Number = "number";

        internal static void Read(XmlReader reader, Phone phone)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (phone == null)
                throw new ArgumentNullException("phone");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(PhoneSerializer.Phone);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, PhoneSerializer.Phone))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case PhoneSerializer.AreaCode:
                            phone.AreaCode = reader.ReadElementContentAsString();
                            break;
                        case PhoneSerializer.Number:
                            phone.Number = reader.ReadElementContentAsString();
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

        internal static void Write(XmlWriter writer, Phone phone)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (phone == null)
                throw new ArgumentNullException("phone");

            writer.WriteStartElement(PhoneSerializer.Phone);
            SerializationHelper.WriteElementStringNotNull(writer, PhoneSerializer.AreaCode, phone.AreaCode);
            SerializationHelper.WriteElementStringNotNull(writer, PhoneSerializer.Number, phone.Number);
            writer.WriteEndElement();
        }
    }
}
