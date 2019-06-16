using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class AddressSerializer
    {
        internal const string Address = "address";

        private const string Country = "country";
        private const string State = "state";
        private const string City = "city";
        private const string District = "district";
        private const string PostalCode = "postalCode";
        private const string Street = "street";
        private const string Number = "number";
        private const string Complement = "complement";

        internal static void Read(XmlReader reader, Address address)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (address == null)
                throw new ArgumentNullException("address");

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return;
            }

            reader.ReadStartElement(AddressSerializer.Address);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, AddressSerializer.Address))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case AddressSerializer.Country:
                            address.Country = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.State:
                            address.State = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.City:
                            address.City = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.District:
                            address.District = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.PostalCode:
                            address.PostalCode = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.Street:
                            address.Street = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.Number:
                            address.Number = reader.ReadElementContentAsString();
                            break;
                        case AddressSerializer.Complement:
                            address.Complement = reader.ReadElementContentAsString();
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

        internal static void Write(XmlWriter writer, Address address)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (address == null)
                throw new ArgumentNullException("address");

            writer.WriteStartElement(AddressSerializer.Address);

            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.Country, address.Country);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.State, address.State);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.City, address.City);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.District, address.District);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.PostalCode, address.PostalCode);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.Street, address.Street);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.Number, address.Number);
            SerializationHelper.WriteElementStringNotNull(writer, AddressSerializer.Complement, address.Complement);

            writer.WriteEndElement();
        }
    }
}
