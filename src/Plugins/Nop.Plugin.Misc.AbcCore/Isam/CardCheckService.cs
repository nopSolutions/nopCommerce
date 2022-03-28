using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;
using Nop.Services.Payments;
using Nop.Core.Domain.Common;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class CardCheckService : ICardCheckService
    {
        private readonly CoreSettings _settings;

        private readonly ILogger _logger;

        public CardCheckService(
            CoreSettings settings,
            ILogger logger
        )
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<(string AuthNo, string RefNo, string StatusCode, string ResponseMessage)> CheckCardAsync(
            ProcessPaymentRequest paymentRequest,
            Address billingAddress,
            string domain,
            string ip
        )
        {
            if (_settings.AreExternalCallsSkipped)
            {
                await _logger.WarningAsync("External calls are turned off, card check skipped.");
                return (null, null, null, null);
            }

            string nl = Environment.NewLine;
            string xml = "";

            xml = $"<Request>{nl}";
            xml += $"<Card_Check>{nl}";
            xml += $"<Card_Number>{paymentRequest.CreditCardNumber}</Card_Number>{nl}";
            xml += $"<Exp_Month>{paymentRequest.CreditCardExpireMonth}</Exp_Month>{nl}";
            xml += $"<Exp_Year>{paymentRequest.CreditCardExpireYear}</Exp_Year>{nl}";
            xml += $"<IP>{ip}</IP>{nl}";
            xml += $"<Cvv2>{paymentRequest.CreditCardCvv2}</Cvv2>{nl}";
            xml += $"<Bill_First_Name>{billingAddress.FirstName}</Bill_First_Name>{nl}";
            xml += $"<Bill_Last_Name>{billingAddress.LastName}</Bill_Last_Name>{nl}";
            xml += $"<Bill_Address>{billingAddress.Address1}</Bill_Address>{nl}";
            xml += $"<Bill_Zip>{billingAddress.ZipPostalCode}</Bill_Zip>{nl}";
            xml += $"<Company>{domain}</Company>{nl}";
            xml += $"<email>{billingAddress.Email}</email>{nl}";
            xml += $"</Card_Check>{nl}";
            xml += $"</Request>";

            if (_settings.IsDebugMode)
            {
                await _logger.InsertLogAsync(
                    LogLevel.Information,
                    "Card Check Request",
                    xml
                );
            }

            var webRequest = HttpWebRequest.CreateHttp(AbcConstants.StatusAPIUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml; charset=utf-8";

            byte[] byteArray = Encoding.UTF8.GetBytes(xml);
            webRequest.ContentLength = byteArray.Length;
            using (System.IO.Stream requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }
            WebResponse response = null;
            try
            {
                response = webRequest.GetResponse();
            }
            catch (WebException e)
            {
                throw new IsamException($"Error when connecting to Card Check API: {e.Message}");
            }
            Stream r_stream = response.GetResponseStream();

            using (StreamReader reader = new StreamReader(r_stream))
            {
                string strResponse = reader.ReadToEnd();

                if (_settings.IsDebugMode)
                {
                    await _logger.InsertLogAsync(
                        LogLevel.Information,
                        "Card Check Response",
                        strResponse
                    );
                }

                if (string.IsNullOrEmpty(strResponse))
                {
                    throw new IsamException(
                        "Empty response received from ISAM backend during Card Check."
                    );
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResponse);

                var authNo = xmlDoc.SelectSingleNode("Response/Card_Check/Auth_No");
                var refNo = xmlDoc.SelectSingleNode("Response/Card_Check/Ref_No");
                var statusCode = xmlDoc.SelectSingleNode("Response/Card_Check/Resp_Code");
                var responseMessage = xmlDoc.SelectSingleNode("Response/Card_Check/Resp_Mesg");

                return (
                    authNo?.InnerText,
                    refNo?.InnerText,
                    statusCode?.InnerText,
                    responseMessage?.InnerText
                );
            }
        }
    }
}