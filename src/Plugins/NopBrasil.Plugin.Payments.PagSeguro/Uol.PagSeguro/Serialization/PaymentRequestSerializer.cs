using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Uol.PagSeguro.Serialization
{
    internal static class PaymentRequestSerializer
    {
        internal const string CHECKOUT = "checkout";
        private const string CURRENCY = "currency";
        private const string REDIRECT_URL = "redirectURL";
        private const string EXTRA_AMOUNT = "extraAmount";
        private const string REFERENCE = "reference";
        private const string MAX_AGE = "maxAge";
        private const string MAX_USES = "maxUses";

        internal static void Write(XmlWriter writer, PaymentRequest payment)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (payment == null)
                throw new ArgumentNullException("payment");

            writer.WriteStartElement(PaymentRequestSerializer.CHECKOUT);

            writer.WriteElementString(PaymentRequestSerializer.CURRENCY, payment.Currency);

            if (payment.Sender != null)
            {
                SenderSerializer.Write(writer, payment.Sender);
            }

            if (payment.RedirectUri != null)
            {
                writer.WriteElementString(PaymentRequestSerializer.REDIRECT_URL, payment.RedirectUri.ToString());
            }

            if (payment.Items.Count > 0)
            {
                ItemListSerializer.Write(writer, payment.Items);
            }

            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.EXTRA_AMOUNT, payment.ExtraAmount);
            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.REFERENCE, payment.Reference);

            if (payment.Shipping != null)
            {
                ShippingSerializer.Write(writer, payment.Shipping);
            }

            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.MAX_AGE, payment.MaxAge);
            SerializationHelper.WriteElementStringNotNull(writer, PaymentRequestSerializer.MAX_USES, payment.MaxUses);

            writer.WriteEndElement();
        }
    }
}
