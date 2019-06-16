using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uol.PagSeguro.Serialization
{
    internal static class PagSeguroErrorsSerializer
    {
        internal const string Errors = "errors";

        internal static void Read(XmlReader reader, IList<PagSeguroServiceError> errors)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (errors == null)
                throw new ArgumentNullException("errors");

            errors.Clear();

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
            }

            reader.ReadStartElement(PagSeguroErrorsSerializer.Errors);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, PagSeguroErrorsSerializer.Errors))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case PagSeguroErrorSerializer.Error:
                            errors.Add(PagSeguroErrorSerializer.Read(reader));
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
