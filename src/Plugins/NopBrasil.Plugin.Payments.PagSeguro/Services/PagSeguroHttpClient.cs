using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Infrastructure;
using Uol.PagSeguro;
using Uol.PagSeguro.Serialization;

namespace NopBrasil.Plugin.Payments.PagSeguro.Services
{
    public partial class PagSeguroHttpClient
    {
        private readonly HttpClient _httpClient;

        public PagSeguroHttpClient(HttpClient client)
        {
            //configure client
            client.Timeout = TimeSpan.FromMilliseconds(5000);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CurrentVersion}");

            _httpClient = client;
        }

        internal async Task<PaymentRequestResponse> CreatePayment(Credentials credentials, PaymentRequest payment, CancellationToken cancellationToken = default)
        {
            var uriBuilder = new UriBuilder(PagSeguroConfiguration.GetPaymentUri(credentials.IsSandbox));
            uriBuilder.Query = ServiceHelper.EncodeCredentialsAsQueryString(credentials);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri))
            {
                var settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.OmitXmlDeclaration = true;

                var sb = PooledStringBuilder.Create();
                using (var writer = XmlWriter.Create(sb, settings))
                {
                    PaymentRequestSerializer.Write(writer, payment);
                    writer.Close();
                }

                requestMessage.Content = new StringContent(sb.ToStringAndReturn(), Encoding.UTF8, ServiceHelper.XML_ENCODED);

                try
                {
                    HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);
                    using (var stream = await responseMessage.Content.ReadAsStreamAsync())
                    {
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            using (var reader = XmlReader.Create(stream))
                            {
                                var paymentResponse = new PaymentRequestResponse(PagSeguroConfiguration.GetPaymentRedirectUri(credentials.IsSandbox));
                                PaymentRequestResponseSerializer.Read(reader, paymentResponse);

                                PagSeguroTrace.Info(string.Format(CultureInfo.InvariantCulture, "PaymentService.Register({0}) - end {1}", payment, paymentResponse.PaymentRedirectUri));

                                return paymentResponse;
                            }
                        }
                        else
                        {
                            var pse = ServiceHelper.CreatePagSeguroServiceException(stream, responseMessage.StatusCode);
                            PagSeguroTrace.Error(string.Format(CultureInfo.InvariantCulture, "PaymentService.Register({0}) - error {1}", payment, pse));
                            throw pse;
                        }
                    }
                }
                //todo - put logger over here
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
