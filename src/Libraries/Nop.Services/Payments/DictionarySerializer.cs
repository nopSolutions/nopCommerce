using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Dictionary serializer
    /// </summary>
    public class DictionarySerializer : IXmlSerializable
    {
        public DictionarySerializer()
        {
            Dictionary = new Dictionary<string, object>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dictionary">Dictionary</param>
        public DictionarySerializer(Dictionary<string, object> dictionary)
        {
            Dictionary = dictionary;
        }

        /// <summary>
        /// Write XML
        /// </summary>
        /// <param name="writer">Writer</param>
        public void WriteXml(XmlWriter writer)
        {
            if (!Dictionary.Any())
                return;

            foreach (var key in Dictionary.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteElementString("key", key);
                var value = Dictionary[key];
                //please note that we use ToString() for objects here
                //of course, we can Serialize them
                //but let's keep it simple and leave it for developers to handle it
                //just put required serialization into ToString method of your object(s)
                //because some objects don't implement ISerializable
                //the question is how should we deserialize null values?
                writer.WriteElementString("value", value?.ToString());
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Read XML
        /// </summary>
        /// <param name="reader">Reader</param>
        public void ReadXml(XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                var key = reader.ReadElementString("key");
                var value = reader.ReadElementString("value");
                Dictionary.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Get schema
        /// </summary>
        /// <returns>XML schema</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Dictionary
        /// </summary>
        public Dictionary<string, object> Dictionary { get; }
    }
}