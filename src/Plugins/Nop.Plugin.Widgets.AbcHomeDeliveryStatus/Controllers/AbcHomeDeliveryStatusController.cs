using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Models;
using Nop.Web.Framework.Controllers;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Controllers
{
    public class AbcHomeDeliveryStatusController : BasePluginController
    {
        [HttpPost]
        public IActionResult DisplayHomeDeliveryStatus(string invoice, string zipcode)
        {
            HomeDeliveryStatusModel model = new HomeDeliveryStatusModel();
            string xmlRequestString = BuildXmlRequestString(invoice, zipcode);
            model.Invoice = invoice;
            model.Zipcode = zipcode;

            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(xmlRequestString, Encoding.UTF8, "text/xml");
                try
                {
                    var response = client.PostAsync(AbcConstants.StatusAPIUrl, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string xmlResponse = response.Content.ReadAsStringAsync().Result;
                        XmlDocument xml = new XmlDocument();
                        // parse response into status information
                        xml.LoadXml(xmlResponse);
                        var topNodes = xml.SelectNodes("Response");
                        foreach (XmlNode node in topNodes)
                        {
                            xmlResponse = node.InnerXml.ToString();
                        }

                        byte[] byteArray = Encoding.UTF8.GetBytes(xmlResponse);
                        StreamReader reader = new StreamReader(new MemoryStream(byteArray));
                        XmlSerializer serializer = new XmlSerializer(typeof(StatusInfo));
                        StatusInfo statusInfo = (StatusInfo)serializer.Deserialize(reader);
                        model.StatusInfo = statusInfo;
                        if (model.StatusInfo.InvoiceNumber.Trim().ToLower() == "not found!")
                        {
                            model.StatusInfo.ErrorMessage = "Status information not found. Please re-enter information";
                        }
                    }
                }
                catch (Exception exception)
                {
                    string message = "Error occurred, please reload the page and try again.Error: " + exception.Message;
                    if (exception.InnerException != null)
                    {
                        message += " " + exception.InnerException.Message;
                    }
                    // return an error view
                    StatusInfo errorInfo = new StatusInfo
                    {
                        ErrorMessage = message
                    };
                    model.StatusInfo = errorInfo;
                }
            }

            return PartialView("~/Plugins/Widgets.AbcHomeDeliveryStatus/Views/_HomeDeliveryResults.cshtml", model);
        }

        private string BuildXmlRequestString(string invoice, string zipcode)
        {
            XElement xml = new XElement("Request",
                new XElement("Delivery_Lookup",
                    new XElement("INVOICE", invoice),
                    new XElement("ZIPCODE", zipcode)
                )
            );
            return xml.ToString();
        }
    }
}
