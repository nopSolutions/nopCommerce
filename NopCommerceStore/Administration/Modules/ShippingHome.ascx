<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ShippingHomeControl"
    CodeBehind="ShippingHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingHome.ShippingHome")%>" />
    <%=GetLocaleResourceString("Admin.ShippingHome.ShippingHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.ShippingHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="shippingsettings.aspx" title="<%=GetLocaleResourceString("Admin.ShippingHome.ShippingSettings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ShippingHome.ShippingSettings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ShippingHome.ShippingSettings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="shippingmethods.aspx" title="<%=GetLocaleResourceString("Admin.ShippingHome.ShippingMethods.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ShippingHome.ShippingMethods.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ShippingHome.ShippingMethods.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="shippingratecomputationmethods.aspx" title="<%=GetLocaleResourceString("Admin.ShippingHome.ShippingRateComputationMethods.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ShippingHome.ShippingRateComputationMethods.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ShippingHome.ShippingRateComputationMethods.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
