using Nop.Core.Infrastructure;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila.Payments
{
    public class SearchPaymentRequest
    {
        public string ExternalReference { get; set; }

        public override string ToString()
        {
            var builder = PooledStringBuilder.Create();
            builder.Append(GetType().Name).Append("(");

            builder.Append("ExternalReference=").Append(ExternalReference).Append(")");

            return builder.ToStringAndReturn();
        }
    }
}
