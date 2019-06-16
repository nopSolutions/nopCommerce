using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class SenderSerializer
    {
        internal const string Sender = "sender";

        private const string Name = "name";
        private const string Email = "email";

        internal static void Read(XmlReader reader, Sender sender)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (sender == null)
                throw new ArgumentNullException("sender");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(SenderSerializer.Sender);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, SenderSerializer.Sender))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case SenderSerializer.Name:
                            sender.Name = reader.ReadElementContentAsString();
                            break;
                        case SenderSerializer.Email:
                            sender.Email = reader.ReadElementContentAsString();
                            break;
                        case PhoneSerializer.Phone:
                            Phone phone = new Phone();
                            PhoneSerializer.Read(reader, phone);
                            sender.Phone = phone;
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

        internal static void Write(XmlWriter writer, Sender sender)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (sender == null)
                throw new ArgumentNullException("sender");

            writer.WriteStartElement(SenderSerializer.Sender);

            SerializationHelper.WriteElementStringNotNull(writer, SenderSerializer.Name, sender.Name);
            SerializationHelper.WriteElementStringNotNull(writer, SenderSerializer.Email, sender.Email);

            if (sender.Phone != null)
            {
                PhoneSerializer.Write(writer, sender.Phone);
            }
            writer.WriteEndElement();
        }
    }
}
