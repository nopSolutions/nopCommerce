using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPoint;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class SermepaReturnPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if (!Page.IsPostBack)
            {
                string HostName = Request.UserHostName;
                IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, "TPV SERMEPA: Host " + HostName, "Host: " + HostName);

                //ID de Pedido
                string orderId = Request["Ds_Order"];
                string strDs_Merchant_Order = Request["Ds_Order"];

                string strDs_Merchant_Amount = Request["Ds_Amount"];
                string strDs_Merchant_MerchantCode = Request["Ds_MerchantCode"];
                string strDs_Merchant_Currency = Request["Ds_Currency"];

                //Respuesta del TPV
                string str_Merchant_Response = Request["Ds_Response"];
                int Ds_Response = Convert.ToInt32(Request["Ds_Response"]);

                //Clave
                bool pruebas = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Sermepa.Pruebas");
                string clave = "";
                if (pruebas) { clave = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.ClavePruebas"); }
                else { clave = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Sermepa.ClaveReal"); };

                //Calculo de la firma
                string SHA = string.Format("{0}{1}{2}{3}{4}{5}",
                    strDs_Merchant_Amount,
                    strDs_Merchant_Order,
                    strDs_Merchant_MerchantCode,
                    strDs_Merchant_Currency,
                    str_Merchant_Response,
                    clave);

                byte[] SHAresult;
                SHA1 shaM = new SHA1Managed();
                SHAresult = shaM.ComputeHash(Encoding.Default.GetBytes(SHA));
                string SHAresultStr = BitConverter.ToString(SHAresult).Replace("-", "");

                //Firma enviada
                string signature = Request["Ds_Signature"];

                //Comprobamos la integridad de las comunicaciones con las claves
                //LogManager.InsertLog(LogTypeEnum.OrderError, "TPV SERMEPA: Clave generada", "CLAVE GENERADA: " + SHAresultStr);
                //LogManager.InsertLog(LogTypeEnum.OrderError, "TPV SERMEPA: Clave obtenida", "CLAVE OBTENIDA: " + signature);
                if (!signature.Equals(SHAresultStr))
                {
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, "TPV SERMEPA: Clave incorrecta", "Las claves enviada y generada no coinciden: " + SHAresultStr + " != " + signature);
                    return;
                }

                //Pedido
                Order order = IoC.Resolve<IOrderService>().GetOrderById(Convert.ToInt32(orderId));
                if (order == null)
                    throw new NopException(string.Format("El pedido de ID {0} no existe", orderId));

                //Actualizamos el pedido
                if (Ds_Response > -1 && Ds_Response < 100)
                {
                    //Lo marcamos como pagado
                    if (IoC.Resolve<IOrderService>().CanMarkOrderAsPaid(order))
                    {
                        IoC.Resolve<IOrderService>().MarkOrderAsPaid(order.OrderId);
                    }
                    //IoC.Resolve<IOrderService>().InsertOrderNote(order.OrderId, "Información del pago: " + Request.Form.ToString(), DateTime.UtcNow);
                }
                else
                {
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, "TPV SERMEPA: Pago no autorizado", "Pago no autorizado con ERROR: " + Ds_Response);
                    //IoC.Resolve<IOrderService>().InsertOrderNote(order.OrderId, "!!! PAGO DENEGADO !!! " + Request.Form.ToString(), DateTime.UtcNow);
                }

            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page is tracked by 'Online Customers' module
        /// </summary>
        public override bool TrackedByOnlineCustomersModule
        {
            get
            {
                return false;
            }
        }
    }
}
