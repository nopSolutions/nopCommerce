<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PaymentHomeControl"
    CodeBehind="PaymentHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.PaymentHome.PaymentHome")%>" />
    <%=GetLocaleResourceString("Admin.PaymentHome.PaymentHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.PaymentHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="creditcardtypes.aspx" title="<%=GetLocaleResourceString("Admin.PaymentHome.CreditCardTypes.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PaymentHome.CreditCardTypes.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PaymentHome.CreditCardTypes.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="paymentmethods.aspx" title="<%=GetLocaleResourceString("Admin.PaymentHome.PaymentMethods.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.PaymentHome.PaymentMethods.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.PaymentHome.PaymentMethods.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
