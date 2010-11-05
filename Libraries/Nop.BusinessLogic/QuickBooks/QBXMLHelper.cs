using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using System.Text.RegularExpressions;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Utils.Html;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.QuickBooks
{
    /// <summary>
    /// Represents QuickBooks helper class
    /// </summary>
    internal class QBXMLHelper
    {
        #region Methods
        public static void SetRequestId(XmlDocument xml, int requestId)
        {
            XmlAttribute attr = xml.CreateAttribute("requestID");
            attr.Value = requestId.ToString();
            xml["QBXML"]["QBXMLMsgsRq"].FirstChild.Attributes.Append(attr);
        }

        public static int GetRequestId(XmlDocument xml)
        {
            return Int32.Parse(xml["QBXML"]["QBXMLMsgsRs"].FirstChild.Attributes["requestID"].Value);
        }

        public static int GetStatusCode(XmlDocument xml)
        {
            return Int32.Parse(xml["QBXML"]["QBXMLMsgsRs"].FirstChild.Attributes["statusCode"].Value);
        }

        public static string GetStatusSeverity(XmlDocument xml)
        {
            return xml["QBXML"]["QBXMLMsgsRs"].FirstChild.Attributes["statusSeverity"].Value;
        }

        public static string GetStatusMessage(XmlDocument xml)
        {
            return xml["QBXML"]["QBXMLMsgsRs"].FirstChild.Attributes["statusMessage"].Value;
        }

        public static string GetResponseType(XmlDocument xml)
        {
            return xml["QBXML"]["QBXMLMsgsRs"].FirstChild.Name;
        }

        public static string GetTxnID(XmlDocument xml)
        {
            return xml["QBXML"]["QBXMLMsgsRs"].FirstChild.FirstChild["TxnID"].InnerText;
        }

        public static string GetListID(XmlDocument xml)
        {
            return xml["QBXML"]["QBXMLMsgsRs"].FirstChild.FirstChild["ListID"].InnerText;
        }

        public static string GetSeqNum(XmlDocument xml)
        {
            return xml["QBXML"]["QBXMLMsgsRs"].FirstChild.FirstChild["EditSequence"].InnerText;
        }

        public static XmlDocument CreateInvoiceModRq(QBEntity entity)
        {
            Order order = entity.NopEnity as Order;

            if(order == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elInvoiceModRq = xml.CreateElement("InvoiceModRq");
            elRoot.AppendChild(elInvoiceModRq);

            XmlElement elInvoiceMod = xml.CreateElement("InvoiceMod");
            elInvoiceModRq.AppendChild(elInvoiceMod);

            elInvoiceMod.AppendChild(CreateIDTypeNode(xml, "TxnID", entity));
            elInvoiceMod.AppendChild(CreateStrTypeNode(xml, "EditSequence", entity.SeqNum));
            elInvoiceMod.AppendChild(CreateCustomerRefNode(xml, order.Customer));
            elInvoiceMod.AppendChild(CreateDateTypeNode(xml, "TxnDate", order.CreatedOn));
            elInvoiceMod.AppendChild(CreateStrTypeNode(xml, "RefNumber", order.OrderId.ToString()));
            elInvoiceMod.AppendChild(CreateAddressNode(xml, "BillAddress", order.BillingAddress1, order.BillingAddress2, order.BillingCity, order.BillingStateProvince, order.BillingZipPostalCode, order.BillingCountry));
            if(order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                elInvoiceMod.AppendChild(CreateAddressNode(xml, "ShipAddress", order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity, order.ShippingStateProvince, order.ShippingZipPostalCode, order.ShippingCountry));
            }
            elInvoiceMod.AppendChild(CreateBoolTypeNode(xml, "IsPending", order.OrderStatus == OrderStatusEnum.Pending));
            elInvoiceMod.AppendChild(CreateStrTypeNode(xml, "PONumber", order.PurchaseOrderNumber));
            if(order.ShippedDate.HasValue)
            {
                elInvoiceMod.AppendChild(CreateDateTypeNode(xml, "ShipDate", order.ShippedDate.Value));
            }
            elInvoiceMod.AppendChild(CreateBoolTypeNode(xml, "IsTaxIncluded", true));

            foreach(OrderProductVariant opv in order.OrderProductVariants)
            {
                elInvoiceMod.AppendChild(CreateInvoiceLineModNode(xml, opv));
            }

            return xml;
        }

        public static XmlDocument CreateInvoiceAddRq(QBEntity entity)
        {
            Order order = entity.NopEnity as Order;

            if(order == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elInvoiceAddRq = xml.CreateElement("InvoiceAddRq");
            elRoot.AppendChild(elInvoiceAddRq);

            XmlElement elInvoiceAdd = xml.CreateElement("InvoiceAdd");
            elInvoiceAddRq.AppendChild(elInvoiceAdd);

            elInvoiceAdd.AppendChild(CreateCustomerRefNode(xml, order.Customer));

            elInvoiceAdd.AppendChild(CreateDateTypeNode(xml, "TxnDate", order.CreatedOn));
            elInvoiceAdd.AppendChild(CreateStrTypeNode(xml, "RefNumber", order.OrderId.ToString()));

            elInvoiceAdd.AppendChild(CreateAddressNode(xml, "BillAddress", order.BillingAddress1, order.BillingAddress2, order.BillingCity, order.BillingStateProvince, order.BillingZipPostalCode, order.BillingCountry));

            if(order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                elInvoiceAdd.AppendChild(CreateAddressNode(xml, "ShipAddress", order.ShippingAddress1, order.ShippingAddress2, order.ShippingCity, order.ShippingStateProvince, order.ShippingZipPostalCode, order.ShippingCountry));
            }

            elInvoiceAdd.AppendChild(CreateBoolTypeNode(xml, "IsPending", order.OrderStatus == OrderStatusEnum.Pending));
            elInvoiceAdd.AppendChild(CreateBoolTypeNode(xml, "IsFinanceCharge", order.PaymentStatus == PaymentStatusEnum.Paid));
            elInvoiceAdd.AppendChild(CreateStrTypeNode(xml, "PONumber", order.PurchaseOrderNumber));
            if(order.ShippedDate.HasValue)
            {
                elInvoiceAdd.AppendChild(CreateDateTypeNode(xml, "ShipDate", order.ShippedDate.Value));
            }
            elInvoiceAdd.AppendChild(CreateBoolTypeNode(xml, "IsTaxIncluded", true));

            foreach(OrderProductVariant opv in order.OrderProductVariants)
            {
                elInvoiceAdd.AppendChild(CreateInvoiceLineAddNode(xml, opv));
            }

            decimal discTotal = order.OrderDiscount;
            if (order.RedeemedRewardPoints != null)
            {
                discTotal += order.RedeemedRewardPoints.UsedAmount;
            }
            foreach(var gc in IoCFactory.Resolve<IOrderService>().GetAllGiftCardUsageHistoryEntries(null, null, order.OrderId))
            {
                discTotal += gc.UsedValue;
            }
            if (discTotal != Decimal.Zero)
            {
                elInvoiceAdd.AppendChild(CreateDiscountLineAddNode(xml, discTotal));
            }


            if (order.PaymentMethodAdditionalFeeInclTax != Decimal.Zero)
            {
                elInvoiceAdd.AppendChild(CreateSalesTaxLineAddNode(xml, order.PaymentMethodAdditionalFeeInclTax));
            }

            if(order.OrderShippingInclTax != Decimal.Zero)
            {
                elInvoiceAdd.AppendChild(CreateShippingLineAddNode(xml, order.OrderShippingInclTax));
            }

            return xml;
        }

        public static XmlDocument CreateCustomerAddRq(QBEntity entity)
        {
            Customer customer = entity.NopEnity as Customer;

            if(customer == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elCustomerAddRq = xml.CreateElement("CustomerAddRq");
            elRoot.AppendChild(elCustomerAddRq);

            XmlElement elCustomerAdd = xml.CreateElement("CustomerAdd");
            elCustomerAddRq.AppendChild(elCustomerAdd);

            elCustomerAdd.AppendChild(CreateStrTypeNode(xml, "Name", customer.Email));
            elCustomerAdd.AppendChild(CreateBoolTypeNode(xml, "IsActive", customer.Active));
            elCustomerAdd.AppendChild(CreateStrTypeNode(xml, "CompanyName", customer.Company));
            elCustomerAdd.AppendChild(CreateStrTypeNode(xml, "FirstName", customer.FirstName));
            elCustomerAdd.AppendChild(CreateStrTypeNode(xml, "LastName", customer.LastName));

            Address billAddr = customer.BillingAddress;
            if (billAddr != null)
            {
                string stateprovince = string.Empty;
                if (billAddr.StateProvince != null)
                    stateprovince = billAddr.StateProvince.Name;
                elCustomerAdd.AppendChild(CreateAddressNode(xml, "BillAddress", billAddr.Address1, billAddr.Address2, billAddr.City, stateprovince, billAddr.ZipPostalCode, billAddr.Country.Name));
            }

            Address shipAddr = customer.ShippingAddress;
            if (shipAddr != null)
            {
                string stateprovince = string.Empty;
                if (shipAddr.StateProvince != null)
                    stateprovince = shipAddr.StateProvince.Name;
                elCustomerAdd.AppendChild(CreateAddressNode(xml, "ShipAddress", shipAddr.Address1, shipAddr.Address2, shipAddr.City, stateprovince, shipAddr.ZipPostalCode, shipAddr.Country.Name));
            }

            elCustomerAdd.AppendChild(CreateStrTypeNode(xml, "Phone", customer.PhoneNumber));
            elCustomerAdd.AppendChild(CreateStrTypeNode(xml, "Email", customer.Email));

            return xml;
        }

        public static XmlDocument CreateCustomerModRq(QBEntity entity)
        {
            Customer customer = entity.NopEnity as Customer;
            if(customer == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elCustomerModRq = xml.CreateElement("CustomerModRq");
            elRoot.AppendChild(elCustomerModRq);

            XmlElement elCustomerMod = xml.CreateElement("CustomerMod");
            elCustomerModRq.AppendChild(elCustomerMod);

            elCustomerMod.AppendChild(CreateIDTypeNode(xml, "ListID", entity));
            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "EditSequence", entity.SeqNum));
            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "Name", customer.Email));
            elCustomerMod.AppendChild(CreateBoolTypeNode(xml, "IsActive", customer.Active));
            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "CompanyName", customer.Company));
            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "FirstName", customer.FirstName));
            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "LastName", customer.LastName));

            Address billAddr = customer.BillingAddress;
            if (billAddr != null)
            {
                elCustomerMod.AppendChild(CreateAddressNode(xml, "BillAddress", billAddr.Address1, billAddr.Address2, billAddr.City, billAddr.StateProvince.Name, billAddr.ZipPostalCode, billAddr.Country.Name));
            }

            Address shipAddr = customer.ShippingAddress;
            if (shipAddr != null)
            {
                elCustomerMod.AppendChild(CreateAddressNode(xml, "ShipAddress", shipAddr.Address1, shipAddr.Address2, shipAddr.City, shipAddr.StateProvince.Name, shipAddr.ZipPostalCode, shipAddr.Country.Name));
            }

            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "Phone", customer.PhoneNumber));
            elCustomerMod.AppendChild(CreateStrTypeNode(xml, "Email", customer.Email));

            return xml;
        }

        public static XmlDocument CreateReceivePaymentAddRq(QBEntity entity)
        {
            Order order = entity.NopEnity as Order;

            if(order == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elReceivePaymentAddRq = xml.CreateElement("ReceivePaymentAddRq");
            elRoot.AppendChild(elReceivePaymentAddRq);

            XmlElement elReceivePaymentAdd = xml.CreateElement("ReceivePaymentAdd");
            elReceivePaymentAddRq.AppendChild(elReceivePaymentAdd);

            elReceivePaymentAdd.AppendChild(CreateCustomerRefNode(xml, order.Customer));
            elReceivePaymentAdd.AppendChild(CreateDateTypeNode(xml, "TxnDate", order.CreatedOn));
            elReceivePaymentAdd.AppendChild(CreateStrTypeNode(xml, "RefNumber", order.OrderId.ToString()));
            elReceivePaymentAdd.AppendChild(CreateAmtTypeNode(xml, "TotalAmount", order.OrderTotal));

            XmlElement elAppliedToTxnAdd = xml.CreateElement("AppliedToTxnAdd");
            elReceivePaymentAdd.AppendChild(elAppliedToTxnAdd);

            elAppliedToTxnAdd.AppendChild(CreateIDTypeNode(xml, "TxnID", IoCFactory.Resolve<IQBService>().GetQBEntityByNopId(EntityTypeEnum.Invoice, order.OrderId)));
            elAppliedToTxnAdd.AppendChild(CreateAmtTypeNode(xml, "PaymentAmount", order.OrderTotal));

            decimal discTotal = order.OrderDiscount;
            if (order.RedeemedRewardPoints != null)
            {
                discTotal += order.RedeemedRewardPoints.UsedAmount;
            }
            foreach (var gc in IoCFactory.Resolve<IOrderService>().GetAllGiftCardUsageHistoryEntries(null, null, order.OrderId))
            {
                discTotal += gc.UsedValue;
            }
            if (discTotal != Decimal.Zero)
            {
                elAppliedToTxnAdd.AppendChild(CreateAmtTypeNode(xml, "DiscountAmount", discTotal));
                elAppliedToTxnAdd.AppendChild(CreateRefNode(xml, "DiscountAccountRef", IoCFactory.Resolve<IQBService>().QBDiscountAccountRef));
            }

            return xml;
        }

        public static XmlDocument CreateTxnVoidRq(QBEntity entity)
        {
            Order order = entity.NopEnity as Order;

            if (order == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elTxnVoidRq = xml.CreateElement("TxnVoidRq");
            elRoot.AppendChild(elTxnVoidRq);
            
            elTxnVoidRq.AppendChild(CreateStrTypeNode(xml, "TxnVoidType", "Invoice"));
            elTxnVoidRq.AppendChild(CreateIDTypeNode(xml, "TxnID", IoCFactory.Resolve<IQBService>().GetQBEntityByNopId(EntityTypeEnum.Invoice, order.OrderId)));

            return xml;
        }

        public static XmlDocument CreateTxnDelRq(QBEntity entity)
        {
            Order order = entity.NopEnity as Order;

            if (order == null)
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();

            XmlElement elRoot = InitializeDocument(xml);

            XmlElement elTxnVoidRq = xml.CreateElement("TxnDelRq");
            elRoot.AppendChild(elTxnVoidRq);

            elTxnVoidRq.AppendChild(CreateStrTypeNode(xml, "TxnDelType", "Invoice"));
            elTxnVoidRq.AppendChild(CreateIDTypeNode(xml, "TxnID", IoCFactory.Resolve<IQBService>().GetQBEntityByNopId(EntityTypeEnum.Invoice, order.OrderId)));

            return xml;
        }
        #endregion

        #region Utilities
        private static XmlElement InitializeDocument(XmlDocument xml)
        {
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            xml.AppendChild(xml.CreateProcessingInstruction("qbxml", "version=\"8.0\""));
        
            XmlElement elQBXML = xml.CreateElement("QBXML");
            xml.AppendChild(elQBXML);

            XmlElement elQBXMLMsgsRq = xml.CreateElement("QBXMLMsgsRq");
            elQBXML.AppendChild(elQBXMLMsgsRq);
            elQBXMLMsgsRq.SetAttribute("onError", "stopOnError");

            return elQBXMLMsgsRq;
        }

        private static XmlElement CreateCustomerRefNode(XmlDocument xml, Customer customer)
        {
            XmlElement elCustomerRef = xml.CreateElement("CustomerRef");
            elCustomerRef.AppendChild(CreateStrTypeNode(xml, "FullName", customer.Email));
            return elCustomerRef;
        }

        private static XmlElement CreateRefNode(XmlDocument xml, string name, string refstr)
        {
            XmlElement el = xml.CreateElement(name);
            el.AppendChild(CreateStrTypeNode(xml, "FullName", refstr));
            return el;
        }

        private static XmlElement CreateDiscountLineAddNode(XmlDocument xml, decimal amount)
        {
            XmlElement el = xml.CreateElement("DiscountLineAdd");
            el.AppendChild(CreateAmtTypeNode(xml, "Amount", amount));
            el.AppendChild(CreateRefNode(xml, "AccountRef", IoCFactory.Resolve<IQBService>().QBDiscountAccountRef));
            return el;
        }

        private static XmlElement CreateShippingLineAddNode(XmlDocument xml, decimal amount)
        {
            XmlElement el = xml.CreateElement("ShippingLineAdd");
            el.AppendChild(CreateAmtTypeNode(xml, "Amount", amount));
            el.AppendChild(CreateRefNode(xml, "AccountRef", IoCFactory.Resolve<IQBService>().QBShippingAccountRef));
            return el;
        }

        private static XmlElement CreateSalesTaxLineAddNode(XmlDocument xml, decimal amount)
        {
            XmlElement el = xml.CreateElement("SalesTaxLineAdd");
            el.AppendChild(CreateAmtTypeNode(xml, "Amount", amount));
            el.AppendChild(CreateRefNode(xml, "AccountRef", IoCFactory.Resolve<IQBService>().QBSalesTaxAccountRef));
            return el;
        }

        private static XmlElement CreatePriceTypeNode(XmlDocument xml, string name, decimal value)
        {
            XmlElement el = xml.CreateElement(name);
            el.InnerText = value.ToString("F", CultureInfo.CreateSpecificCulture(IoCFactory.Resolve<IQBService>().QBCultureName));
            return el;
        }

        private static XmlElement CreateAmtTypeNode(XmlDocument xml, string name, decimal value)
        {
            XmlElement el = xml.CreateElement(name);
            el.InnerText = value.ToString("F", CultureInfo.CreateSpecificCulture(IoCFactory.Resolve<IQBService>().QBCultureName));
            return el;
        }

        private static XmlElement CreateQuanTypeNode(XmlDocument xml, string name, decimal value)
        {
            XmlElement el = xml.CreateElement(name);
            el.InnerText = value.ToString("F", CultureInfo.CreateSpecificCulture(IoCFactory.Resolve<IQBService>().QBCultureName));
            return el;
        }

        private static XmlElement CreateBoolTypeNode(XmlDocument xml, string name, bool value)
        {
            XmlElement el = xml.CreateElement(name);
            el.InnerText = value.ToString().ToLowerInvariant();
            return el;
        }

        private static XmlElement CreateDateTypeNode(XmlDocument xml, string name, DateTime value)
        {
            XmlElement el = xml.CreateElement(name);
            el.InnerText = value.ToString("yyyy-MM-dd");
            return el;
        }

        private static XmlElement CreateStrTypeNode(XmlDocument xml, string name, string value)
        {
            Encoding enc = Encoding.GetEncoding("us-ascii");
            value = enc.GetString(enc.GetBytes(value));

            XmlElement el = xml.CreateElement(name);
            el.InnerText = value;
            return el;
        }

        private static XmlElement CreateIDTypeNode(XmlDocument xml, string name, QBEntity entity)
        {
            XmlElement el = xml.CreateElement(name);
            el.InnerText = entity.QBEntityId;
            return el;
        }
        
        private static XmlElement CreateInvoiceLineAddNode(XmlDocument xml, OrderProductVariant opv)
        {
            XmlElement el = xml.CreateElement("InvoiceLineAdd");
            ProductVariant pv = opv.ProductVariant;

            el.AppendChild(CreateRefNode(xml, "ItemRef", IoCFactory.Resolve<IQBService>().QBItemRef));
            el.AppendChild(CreateStrTypeNode(xml, "Desc", pv != null ? pv.FullProductName : "Product variant is not available"));
            el.AppendChild(CreateQuanTypeNode(xml, "Quantity", opv.Quantity));
            el.AppendChild(CreatePriceTypeNode(xml, "Rate", opv.UnitPriceInclTax));

            return el;
        }

        private static XmlElement CreateInvoiceLineModNode(XmlDocument xml, OrderProductVariant opv)
        {
            XmlElement el = xml.CreateElement("InvoiceLineMod");
            ProductVariant pv = opv.ProductVariant;

            el.AppendChild(CreateStrTypeNode(xml, "TxnLineID", "-1"));
            el.AppendChild(CreateRefNode(xml, "ItemRef", IoCFactory.Resolve<IQBService>().QBItemRef));
            el.AppendChild(CreateStrTypeNode(xml, "Desc", pv != null ? pv.FullProductName : "Product variant is not available"));
            el.AppendChild(CreateQuanTypeNode(xml, "Quantity", opv.Quantity));
            el.AppendChild(CreatePriceTypeNode(xml, "Rate", opv.UnitPriceInclTax));

            return el;
        }

        private static XmlElement CreateAddressNode(XmlDocument xml, string addrName, string addr1, string addr2, string city, string state, string postalCode, string country)
        {
            XmlElement elAddr = xml.CreateElement(addrName);
            elAddr.AppendChild(CreateStrTypeNode(xml, "Addr1", addr1));
            if(!String.IsNullOrEmpty(addr2))
            {
                elAddr.AppendChild(CreateStrTypeNode(xml, "Addr2", addr2));
            }
            elAddr.AppendChild(CreateStrTypeNode(xml, "City", city));
            elAddr.AppendChild(CreateStrTypeNode(xml, "State", state));
            elAddr.AppendChild(CreateStrTypeNode(xml, "PostalCode", postalCode));
            elAddr.AppendChild(CreateStrTypeNode(xml, "Country", country));

            return elAddr;
        }
        #endregion
    }
}
