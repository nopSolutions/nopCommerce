using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Param.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Param.Controllers
{

    public class PaymentParamController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IParamPaymentSettings _paramPaymentSettings;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly IGenericAttributeService _genericAttributeService;

        public PaymentParamController(ISettingService settingService,
            IParamPaymentSettings paramPaymentSettings,
            IWorkContext workContext,
            IOrderService orderService,
            IStoreContext storeContext,
            IOrderProcessingService orderProcessingService,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            IGenericAttributeService genericAttributeService,
            IPermissionService permissionService)
        {
            this._settingService = settingService;
            this._paramPaymentSettings = paramPaymentSettings;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._orderService = orderService;
            this._storeContext = storeContext;
            this._orderProcessingService = orderProcessingService;
            this._webHelper = webHelper;
            this._httpContextAccessor = httpContextAccessor;
            this._genericAttributeService = genericAttributeService;
        }


        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async System.Threading.Tasks.Task<IActionResult> ConfigureAsync()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel()
            {
                UseSandbox = _paramPaymentSettings.UseSandbox,
                ClientCode = _paramPaymentSettings.ClientCode,
                ClientUsername = _paramPaymentSettings.ClientUsername,
                ClientPassword = _paramPaymentSettings.ClientPassword,
                Guid = _paramPaymentSettings.Guid,
                TestUrl = _paramPaymentSettings.TestUrl,
                ProductUrl = _paramPaymentSettings.ProductUrl,
                Installment = _paramPaymentSettings.Installment
            };
            return View("~/Plugins/Payments.Param/Views/Configure.cshtml", model);
        }



        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async System.Threading.Tasks.Task<IActionResult> ConfigureAsync(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await ConfigureAsync();

            //save settings
            _paramPaymentSettings.UseSandbox = model.UseSandbox;
            _paramPaymentSettings.ClientCode = model.ClientCode;
            _paramPaymentSettings.ClientUsername = model.ClientUsername;
            _paramPaymentSettings.ClientPassword = model.ClientPassword;
            _paramPaymentSettings.Guid = model.Guid;
            _paramPaymentSettings.TestUrl = model.TestUrl;
            _paramPaymentSettings.ProductUrl = model.ProductUrl;
            _paramPaymentSettings.Installment = model.Installment;
            await _settingService.SaveSettingAsync(_paramPaymentSettings);

            return await ConfigureAsync();
        }



        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> BIN_SanalPosAsync(string cardCode)
        {
            if (string.IsNullOrEmpty(cardCode))
            {
                return Json("");
            }

            if (string.IsNullOrEmpty(_paramPaymentSettings.ClientCode))
            {
                if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
                {
                    return Json(new { error = "Hata: Ayarlardan 'Müşteri Kodu' girişi yapınız!" });
                }
                else
                {
                    return Json(new { error = "Error: Enter 'Client Code' from settings!" });
                }
            }
            if (string.IsNullOrEmpty(_paramPaymentSettings.ClientUsername))
            {
                if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
                {
                    return Json(new { error = "Hata: Ayarlardan 'Müşteri Adı' girişi yapınız!" });
                }
                else
                {
                    return Json(new { error = "Error: Enter 'Client Username' from settings!" });
                }
            }
            if (string.IsNullOrEmpty(_paramPaymentSettings.ClientPassword))
            {
                if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
                {
                    return Json(new { error = "Hata: Ayarlardan 'Müşteri Parola' girişi yapınız!" });
                }
                else
                {
                    return Json(new { error = "Error: Enter 'Client Password' from settings!" });
                }
            }
            if (string.IsNullOrEmpty(_paramPaymentSettings.ProductUrl))
            {
                if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
                {
                    return Json(new { error = "Hata: Ayarlardan 'Product Url' girişi yapınız!" });
                }
                else
                {
                    return Json(new { error = "Error: Enter 'Product Url' from settings!" });
                }
            }
            if (string.IsNullOrEmpty(_paramPaymentSettings.ProductUrl))
            {
                if ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture == "tr-TR")
                {
                    return Json(new { error = "Hata: Ayarlardan 'Product Url' girişi yapınız!" });
                }
                else
                {
                    return Json(new { error = "Error: Enter 'Product Url' from settings!" });
                }
            }
            string url = _paramPaymentSettings.UseSandbox ? _paramPaymentSettings.TestUrl : _paramPaymentSettings.ProductUrl;
            string clientCode = _paramPaymentSettings.ClientCode;
            string clientUsername = _paramPaymentSettings.ClientUsername;
            string clientPassword = _paramPaymentSettings.ClientPassword;

            string data = "" +
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                    "<soap:Body>" +
                        "<BIN_SanalPos xmlns=\"https://turkpos.com.tr/\">" +
                            "<G>" +
                                "<CLIENT_CODE>" + clientCode + "</CLIENT_CODE>" +
                                "<CLIENT_USERNAME>" + clientUsername + "</CLIENT_USERNAME>" +
                                "<CLIENT_PASSWORD>" + clientPassword + "</CLIENT_PASSWORD>" +
                            "</G>" +
                            "<BIN>" + cardCode + "</BIN>" +
                        "</BIN_SanalPos>" +
                    "</soap:Body>" +
                "</soap:Envelope>";

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/xml; charset=\"utf-8\"";
            request.ContentLength = buffer.Length;
            request.Headers.Add("SOAPAction", "https://turkpos.com.tr/BIN_SanalPos");
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

            string sanalPOS_ID = xdoc.Descendants("SanalPOS_ID").FirstOrDefault().Value;
            string kart_Banka = xdoc.Descendants("Kart_Banka").FirstOrDefault().Value;


            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("https://lookup.binlist.net/" + cardCode);

            string name = new Regex(@"\{""name"":""([^""].*?)"",", RegexOptions.IgnoreCase).Match(json).Groups[1].Value;
            string brand = new Regex(@"""scheme"":""([^""].*?)"",", RegexOptions.IgnoreCase).Match(json).Groups[1].Value;
            string type = new Regex(@"""type"":""([^""].*?)"",", RegexOptions.IgnoreCase).Match(json).Groups[1].Value;
            brand = char.ToUpper(brand[0]) + brand.Substring(1);
            type = char.ToUpper(type[0]) + type.Substring(1);

            return Json(new { SanalPOS_ID = sanalPOS_ID, Kart_Banka = kart_Banka, Kart_Brand = brand, Kart_Tip = type });
        }



        [Route("PaymentParam/OrderRefresh/{orderId?}/")]
        public async System.Threading.Tasks.Task<IActionResult> OrderRefreshAsync(int orderId = 0)
        {
            if (orderId > 0)
            {
                Order lastOrder = (await _orderService.GetOrderByIdAsync(orderId));

                if (lastOrder != null)
                {
                    if (lastOrder.PaymentMethodSystemName.ToLower().Equals("payments.param"))
                    {
                        if (lastOrder.PaymentStatus == PaymentStatus.Pending)
                        {
                            if (_orderProcessingService.CanCancelOrder(lastOrder))
                            {
                                await _orderProcessingService.DeleteOrderAsync(lastOrder);
                                await _orderProcessingService.ReOrderAsync(lastOrder);
                                return RedirectToRoute("Checkout");
                            }
                        }
                    }
                }

                return RedirectToRoute("OrderDetails", new { orderId = orderId });
            }

            return RedirectToRoute("Cart");
        }


        [Route("PaymentParam/OrderComplete/{hash?}/{orderId?}/")]
        public async System.Threading.Tasks.Task<IActionResult> OrderCompleteAsync(string hash = "", int orderId = 0)
        {
            if (hash != "" && orderId > 0)
            {
                Order lastOrder = (await _orderService.GetOrderByIdAsync(orderId));

                if (lastOrder != null)
                {
                    if (lastOrder.PaymentMethodSystemName.ToLower().Equals("payments.param"))
                    {
                        if (lastOrder.OrderGuid.ToString() == hash)
                        {
                            if (lastOrder.PaymentStatus == PaymentStatus.Pending)
                            {
                                if (_orderProcessingService.CanCancelOrder(lastOrder))
                                {
                                    if (_orderProcessingService.CanMarkOrderAsPaid(lastOrder))
                                    {
                                        await _orderProcessingService.MarkOrderAsPaidAsync(lastOrder);
                                        return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
                                    }
                                }
                            }
                        }
                    }
                }

                return RedirectToRoute("OrderDetails", new { orderId = orderId });
            }

            return RedirectToRoute("Cart");
        }



    }
}