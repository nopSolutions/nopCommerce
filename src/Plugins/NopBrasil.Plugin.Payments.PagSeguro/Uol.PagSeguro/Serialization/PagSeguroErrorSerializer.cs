using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class PagSeguroErrorSerializer
    {
        internal const string Error = "error";

        private const string Code = "code";
        private const string Message = "message";

        internal static PagSeguroServiceError Read(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            string code = String.Empty;
            String message = String.Empty;

            if (reader.IsEmptyElement)
            {
                SerializationHelper.SkipNode(reader);
                return new PagSeguroServiceError(code, message);
            }

            reader.ReadStartElement(PagSeguroErrorSerializer.Error);
            reader.MoveToContent();

            while (!reader.EOF)
            {
                if (SerializationHelper.IsEndElement(reader, PagSeguroErrorSerializer.Error))
                {
                    SerializationHelper.SkipNode(reader);
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case PagSeguroErrorSerializer.Code:
                            code = reader.ReadElementContentAsString();
                            break;
                        case PagSeguroErrorSerializer.Message:
                            message = reader.ReadElementContentAsString();
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

            PagSeguroServiceError error = new PagSeguroServiceError(code, message);
            return error;
        }
    }
}
