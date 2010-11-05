using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.QuickBooks
{
    /// <summary>
    /// Represents implementation of QuickBooks Web Connector Service
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceAttribute(Namespace = "http://developer.intuit.com/")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "QBConnectorSoap", Namespace = "http://developer.intuit.com/")]
    public partial class QBConnector : System.Web.Services.WebService
    {
        #region Methods
        /// <summary>
        /// To enable web service with its version number returned back to QBWC
        /// </summary>
        /// <returns>Server version</returns>
        [WebMethod]
        public string serverVersion()
        {
            return IoCFactory.Resolve<ISettingManager>().CurrentVersion;
        }

        /// <summary>
        /// To enable web service with QBWC version control
        /// </summary>
        /// <param name="strVersion">Version</param>
        /// <returns>Client version</returns>
        [WebMethod]
        public string clientVersion(string strVersion)
        {
            if(String.IsNullOrEmpty(strVersion))
            {
                return "E:Version string is not valid";
            }
            string[] tmp = strVersion.Split('.');
            if(tmp.Length < 2)
            {
                return "E:Version string is not valid";
            }
            double version = 0;
            Double.TryParse(String.Format("{0}.{1}", tmp[0], tmp[1]), out version);
            if(version < 2.0)
            {
                return "E:You need to upgrade your QBWebConnector";
            }
            return String.Empty;
        }


        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="strUserName">Username</param>
        /// <param name="strPassword">Password</param>
        /// <returns>Authenticate result</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/authenticate", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] authenticate(string strUserName, string strPassword)
        {
            string[] rsp = new string[4];

            if (!IoCFactory.Resolve<IQBService>().QBIsEnabled || !strUserName.Equals(IoCFactory.Resolve<IQBService>().QBUsername) || !strPassword.Equals(IoCFactory.Resolve<IQBService>().QBPassword))
            {
                rsp[1] = "nvu";
            }
            else
            {
                rsp[0] = Ticket;
                IsTicketActive = true;
            }

            return rsp;

        }

        /// <summary>
        /// Send request
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <param name="strHCPResponse">HCP Response</param>
        /// <param name="strCompanyFileName">Company filename</param>
        /// <param name="qbXMLCountry">Country</param>
        /// <param name="qbXMLMajorVers">Major version</param>
        /// <param name="qbXMLMinorVers">Minor version</param>
        /// <returns>Result</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/sendRequestXML", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string sendRequestXML(string ticket, string strHCPResponse, string strCompanyFileName, string qbXMLCountry, int qbXMLMajorVers, int qbXMLMinorVers)
        {
            try
            {
                if(!IoCFactory.Resolve<IQBService>().QBIsEnabled || !Ticket.Equals(ticket) || !IsTicketActive)
                {
                    return String.Empty;
                }

                QBEntity entity = IoCFactory.Resolve<IQBService>().GetQBEntityForSynchronization();
                if(entity == null)
                {
                    return String.Empty;
                }

                XmlDocument req = null;
                switch(entity.EntityType)
                {
                    case EntityTypeEnum.Customer:
                        req = String.IsNullOrEmpty(entity.QBEntityId) ? QBXMLHelper.CreateCustomerAddRq(entity) : QBXMLHelper.CreateCustomerModRq(entity);
                        break;
                    case EntityTypeEnum.Invoice:
                        req = String.IsNullOrEmpty(entity.QBEntityId) ? QBXMLHelper.CreateInvoiceAddRq(entity) : QBXMLHelper.CreateInvoiceModRq(entity);
                        break;
                    case EntityTypeEnum.ReceivePayment:
                        req = QBXMLHelper.CreateReceivePaymentAddRq(entity);
                        break;
                    case EntityTypeEnum.TxnVoid:
                        req = QBXMLHelper.CreateTxnVoidRq(entity);
                        break;
                    case EntityTypeEnum.TxnDel:
                        req = QBXMLHelper.CreateTxnDelRq(entity);
                        break;
                    default:
                        return String.Empty;
                }

                if(req == null)
                {
                    return String.Empty;
                }

                QBXMLHelper.SetRequestId(req, entity.EntityId);

                return req.OuterXml;
            }
            catch(Exception ex)
            {
                IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, ex.Message, ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Receive response
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <param name="response">Response</param>
        /// <param name="hresult">Hresult</param>
        /// <param name="message">Message</param>
        /// <returns>Result code</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/receiveResponseXML", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int receiveResponseXML(string ticket, string response, string hresult, string message)
        {
            try
            {
                if (!IoCFactory.Resolve<IQBService>().QBIsEnabled || !Ticket.Equals(ticket) || !IsTicketActive)
                {
                    return -1;
                }

                if(!String.IsNullOrEmpty(hresult))
                {
                    IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, message, hresult);
                    return -1;
                }

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(response);

                int requestId = QBXMLHelper.GetRequestId(xml);
                QBEntity entity = IoCFactory.Resolve<IQBService>().GetQBEntityById(requestId);
                if(entity == null)
                {
                    return -1;
                }

                int statusCode = QBXMLHelper.GetStatusCode(xml);
                switch (statusCode)
                {
                    case 0:
                    case 530:
                    case 531:
                    case 550:
                    case 560:
                    case 570:
                        {
                            string qbId = entity.QBEntityId;
                            string seqNum = entity.SeqNum;
                            switch (QBXMLHelper.GetResponseType(xml))
                            {
                                case "TxnVoidRs":
                                case "TxnDelRs":
                                    qbId = QBXMLHelper.GetTxnID(xml);
                                    break;
                                case "ReceivePaymentAddRs":
                                case "InvoiceAddRs":
                                    qbId = QBXMLHelper.GetTxnID(xml);
                                    seqNum = QBXMLHelper.GetSeqNum(xml);
                                    break;
                                case "CustomerAddRs":
                                    qbId = QBXMLHelper.GetListID(xml);
                                    seqNum = QBXMLHelper.GetSeqNum(xml);
                                    break;
                                case "CustomerModRs":
                                case "InvoiceModRs":
                                    seqNum = QBXMLHelper.GetSeqNum(xml);
                                    break;
                            }

                            entity.QBEntityId = qbId;
                            entity.SynStateId = (int)SynStateEnum.Success;
                            entity.SeqNum = seqNum;
                            entity.UpdatedOn = DateTime.UtcNow;
                            IoCFactory.Resolve<IQBService>().UpdateQBEntity(entity);
                        }
                        break;
                    case 3175:
                        {
                            IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, QBXMLHelper.GetStatusMessage(xml), statusCode.ToString());
                        }
                        break;
                    default:
                        {
                            IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, QBXMLHelper.GetStatusMessage(xml), statusCode.ToString());

                            entity.SynStateId = (int)SynStateEnum.Failed;
                            entity.UpdatedOn = DateTime.UtcNow;
                            IoCFactory.Resolve<IQBService>().UpdateQBEntity(entity);
                        }
                        break;
                }
                return 0;
            }
            catch(Exception ex)
            {
                IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, ex.Message, ex);
                return -1;
            }
        }

        /// <summary>
        /// Connection error notification
        /// </summary>
        /// <param name="ticket">Ticker</param>
        /// <param name="hresult">hresult</param>
        /// <param name="message">Message</param>
        /// <returns>Result</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/connectionError", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string connectionError(string ticket, string hresult, string message)
        {
            IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, message, hresult);
            return String.Empty;
        }

        /// <summary>
        /// Gets the last error
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <returns>Error</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/getLastError", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getLastError(string ticket)
        {
            return String.Empty;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <param name="ticket">Ticket</param>
        /// <returns>Result</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://developer.intuit.com/closeConnection", RequestNamespace = "http://developer.intuit.com/", ResponseNamespace = "http://developer.intuit.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string closeConnection(string ticket)
        {
            IsTicketActive = false;
            return "Closed";
        }
        #endregion

        #region Properties
        private static string Ticket
        {
            get
            {
                Setting setting = IoCFactory.Resolve<ISettingManager>().GetSettingByName("QB.Ticket");
                if (setting == null)
                {
                    setting = IoCFactory.Resolve<ISettingManager>().SetParam("QB.Ticket", Guid.NewGuid().ToString());
                }
                return setting.Value;
            }
        }

        private static bool IsTicketActive
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("QB.Ticket.IsActive", false);
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("QB.Ticket.IsActive", value.ToString());
            }
        }
        #endregion
    }
}