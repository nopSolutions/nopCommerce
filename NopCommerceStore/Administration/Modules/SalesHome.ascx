<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SalesHomeControl"
    CodeBehind="SalesHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.SalesHome.SalesHome")%>" />
    <%=GetLocaleResourceString("Admin.SalesHome.SalesHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.SalesHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="orders.aspx" title="<%=GetLocaleResourceString("Admin.SalesHome.Orders.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SalesHome.Orders.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SalesHome.Orders.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="recurringpayments.aspx" title="<%=GetLocaleResourceString("Admin.SalesHome.RecurringPayments.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SalesHome.RecurringPayments.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SalesHome.RecurringPayments.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="purchasedgiftcards.aspx" title="<%=GetLocaleResourceString("Admin.SalesHome.PurchasedGiftCards.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SalesHome.PurchasedGiftCards.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SalesHome.PurchasedGiftCards.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="returnrequests.aspx" title="<%=GetLocaleResourceString("Admin.SalesHome.ReturnRequests.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SalesHome.ReturnRequests.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SalesHome.ReturnRequests.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="currentshoppingcarts.aspx" title="<%=GetLocaleResourceString("Admin.SalesHome.CurrentShoppingCarts.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SalesHome.CurrentShoppingCarts.Title")%>
                    </a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SalesHome.CurrentShoppingCarts.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="salesreport.aspx" title="<%=GetLocaleResourceString("Admin.SalesHome.SalesReport.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.SalesHome.SalesReport.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.SalesHome.SalesReport.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
