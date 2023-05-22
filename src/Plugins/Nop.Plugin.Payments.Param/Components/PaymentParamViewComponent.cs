using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Payments.Param.Models;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Param.Components
{
    [ViewComponent(Name = "PaymentParam")]
    public class PaymentParamViewComponent : NopViewComponent
    {
        private readonly IParamPaymentSettings _paramPaymentSettings;
        private readonly IShoppingCartModelFactory _shoppingCartService;
        private readonly IWorkContext _workContext;

        public PaymentParamViewComponent(IParamPaymentSettings paramPaymentSettings, 
            IShoppingCartModelFactory shoppingCartService,
            IWorkContext workContext)
        {
            this._paramPaymentSettings = paramPaymentSettings;
            this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
        }

        public async System.Threading.Tasks.Task<IViewComponentResult> InvokeAsync()
        {
            var model = new PaymentInfoModel();

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = Convert.ToString(DateTime.Now.Year + i);
                model.ExpireYears.Add(new SelectListItem { Text = year, Value = year });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                var text = $"{i:00}";
                model.ExpireMonths.Add(new SelectListItem()
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }

            List<MyOzel_Oranlar> oranlistesi = TP_Ozel_Oran_Liste();
            ViewBag.TP_Ozel_Oran_Liste = oranlistesi;
            if (ViewBag.TP_Ozel_Oran_Liste is null || oranlistesi.Count == 0)
            { 
                ViewBag.TP_Ozel_Oran_Sonuc = TP_Ozel_Oran_Liste("sonuc");
            }

            var cartTotal = await _shoppingCartService.PrepareMiniShoppingCartModelAsync();
            ViewBag.SubTotalStr = cartTotal.SubTotal;
            ViewBag.SubTotal = Regex.Replace(cartTotal.SubTotal, @"[^0-9\,]", "");

            if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
            {
                ViewBag.Lang = "tr-TR";
            }
            else
            {
                ViewBag.Lang = "en-US";
            }

            model.Status = true;

            //set postback values (we cannot access "Form" with "GET" requests)
            if (Request.Method == WebRequestMethods.Http.Get)
                return View("~/Plugins/Payments.Param/Views/PaymentInfo.cshtml", model);

            var form = Request.Form;
            model.CardholderName = form["CardholderName"];
            model.CardNumber = form["CardNumber"];
            model.CardCode = form["CardCode"];
            var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;
            
            return View("~/Plugins/Payments.Param/Views/PaymentInfo.cshtml", model);
        }


        public class MyOzel_Oranlar
        {
            public string Ozel_Oran_ID;
            public string GUID;
            public string Tarih_Bas;
            public string Tarih_Bit;
            public string SanalPOS_ID;
            public string Kredi_Karti_Banka;
            public string Kredi_Karti_Banka_Gorsel;
            public string MO_01;
            public string MO_02;
            public string MO_03;
            public string MO_04;
            public string MO_05;
            public string MO_06;
            public string MO_07;
            public string MO_08;
            public string MO_09;
            public string MO_10;
            public string MO_11;
            public string MO_12;
        }
        public dynamic TP_Ozel_Oran_Liste(string type = "")
        {
            //https://localhost:44369/ParamPos/TP_Ozel_Oran_Liste
            //https://localhost:44369/Admin/ParamPos/TP_Ozel_Oran_Liste
            string url = _paramPaymentSettings.UseSandbox ? _paramPaymentSettings.TestUrl : _paramPaymentSettings.ProductUrl;
            string clientCode = _paramPaymentSettings.ClientCode;
            string clientUsername = _paramPaymentSettings.ClientUsername;
            string clientPassword = _paramPaymentSettings.ClientPassword;
            string guidCode = _paramPaymentSettings.Guid;
            bool installment = _paramPaymentSettings.Installment;

            if (!installment)
            {
                return null;
            }

            string data = "" +
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                    "<soap:Body>" +
                        "<TP_Ozel_Oran_Liste xmlns=\"https://turkpos.com.tr/\">" +
                            "<G>" +
                                "<CLIENT_CODE>" + clientCode + "</CLIENT_CODE>" +
                                "<CLIENT_USERNAME>" + clientUsername + "</CLIENT_USERNAME>" +
                                "<CLIENT_PASSWORD>" + clientPassword + "</CLIENT_PASSWORD>" +
                            "</G>" +
                            "<GUID>" + guidCode + "</GUID>" +
                        "</TP_Ozel_Oran_Liste>" +
                    "</soap:Body>" +
                "</soap:Envelope>";

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/xml; charset=\"utf-8\"";
            request.ContentLength = buffer.Length;
            request.Headers.Add("SOAPAction", "https://turkpos.com.tr/TP_Ozel_Oran_Liste");
            Stream post = request.GetRequestStream();

            post.Write(buffer, 0, buffer.Length);
            post.Close();

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseData = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseData);
            string responseResult = responseReader.ReadToEnd();
            responseResult = responseResult.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            responseResult = responseResult.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            responseResult = responseResult.Replace(" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            responseResult = responseResult.Replace("soap:", "").Replace(":soap", "");
            responseResult = responseResult.Replace(" xmlns=\"https://turkpos.com.tr/\"", "");
            responseResult = responseResult.Replace("<Body>", "").Replace("</Body>", "");
            responseResult = responseResult.Replace("<Envelope>", "<root>").Replace("</Envelope>", "</root>");

            XDocument xdoc = XDocument.Parse(responseResult);

            var oranlarListesi = new List<MyOzel_Oranlar>();

            var oranlar = (from p in xdoc.Descendants("DT_Ozel_Oranlar")
                           select new
                           {
                               ozel_Oran_ID = p.Element("Ozel_Oran_ID").Value ?? "",
                               gUID = p.Element("GUID").Value ?? "",
                               tarih_Bas = p.Element("Tarih_Bas").Value ?? "",
                               tarih_Bit = p.Element("Tarih_Bit").Value ?? "",
                               sanalPOS_ID = p.Element("SanalPOS_ID").Value ?? "",
                               kredi_Karti_Banka = p.Element("Kredi_Karti_Banka").Value ?? "",
                               kredi_Karti_Banka_Gorsel = p.Element("Kredi_Karti_Banka_Gorsel").Value ?? "",
                               mO_01 = p.Element("MO_01").Value ?? "",
                               mO_02 = p.Element("MO_02").Value ?? "",
                               mO_03 = p.Element("MO_03").Value ?? "",
                               mO_04 = p.Element("MO_04").Value ?? "",
                               mO_05 = p.Element("MO_05").Value ?? "",
                               mO_06 = p.Element("MO_06").Value ?? "",
                               mO_07 = p.Element("MO_07").Value ?? "",
                               mO_08 = p.Element("MO_08").Value ?? "",
                               mO_09 = p.Element("MO_09").Value ?? "",
                               mO_10 = p.Element("MO_10").Value ?? "",
                               mO_11 = p.Element("MO_11").Value ?? "",
                               mO_12 = p.Element("MO_12").Value ?? ""
                           }).ToArray();


            foreach (var oran in oranlar)
            {

                oranlarListesi.Add(new MyOzel_Oranlar
                {
                    Ozel_Oran_ID = oran.ozel_Oran_ID,
                    GUID = oran.gUID,
                    Tarih_Bas = oran.tarih_Bas,
                    Tarih_Bit = oran.tarih_Bit,
                    SanalPOS_ID = oran.sanalPOS_ID,
                    Kredi_Karti_Banka = oran.kredi_Karti_Banka,
                    Kredi_Karti_Banka_Gorsel = oran.kredi_Karti_Banka_Gorsel,
                    MO_01 = oran.mO_01,
                    MO_02 = oran.mO_02,
                    MO_03 = oran.mO_03,
                    MO_04 = oran.mO_04,
                    MO_05 = oran.mO_05,
                    MO_06 = oran.mO_06,
                    MO_07 = oran.mO_07,
                    MO_08 = oran.mO_08,
                    MO_09 = oran.mO_09,
                    MO_10 = oran.mO_10,
                    MO_11 = oran.mO_11,
                    MO_12 = oran.mO_12
                });

            }

            if(type == "sonuc")
            {
                return xdoc.Descendants("Sonuc_Str").FirstOrDefault().Value;
            }
            return oranlarListesi;
        }


    }
}
