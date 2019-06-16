using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class PaymentRequestSerializer
    {
        internal const string Checkout = "checkout";
        private const string Currency = "currency";
        private const string RedirectUrl = "redirectURL";
        private const string ExtraAmount = "extraAmount";
        private const string Reference = "reference";
        private const string MaxAge = "maxAge";
        private const string MaxUses = "maxUses";

        internal static void Write(XmlWriter writer, PaymentRequest payment)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (payment == null)
                throw new ArgumentNullException("payment");

            writer.WriteStartElement(PaymentRequestSerializer.Checkout);

            writer.WriteElementString(PaymentRequestSerializer.Currency, payment.Currency);

            if (payment.Sender != null)
            {
                SenderSerializer.Write(writer, payment.Sender);
            }

            if (payment.RedirectUri != null)
            {
                writer.WriteElementString(PaymentRequestSerializer.RedirectUrl, payment.RedirectUri.ToString());
            }

            if (payment.Items.Count > 0)
            {
                ItemListSerializer.Write(writer, payment.Items);
            }

            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.ExtraAmount, payment.ExtraAmount);
            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.Reference, payment.Reference);

            if (payment.Shipping != null)
            {
                ShippingSerializer.Write(writer, payment.Shipping);
            }

            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.MaxAge, payment.MaxAge);
            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.MaxUses, payment.MaxUses);

            writer.WriteEndElement();
        }
    }
}
