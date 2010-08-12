<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ConfigurationHomeControl"
    CodeBehind="ConfigurationHome.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ConfigurationHome.ConfigurationHome")%>" />
    <%=GetLocaleResourceString("Admin.ConfigurationHome.ConfigurationHome")%>
</div>
<div class="homepage">
    <div class="intro">
        <p>
            <%=GetLocaleResourceString("Admin.ConfigurationHome.intro")%>
        </p>
    </div>
    <div class="options">
        <ul>
            <li>
                <div class="title">
                    <a href="globalsettings.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.GlobalSettings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.GlobalSettings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.GlobalSettings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="emailaccounts.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.EmailAccounts.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.EmailAccounts.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.EmailAccounts.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="paymentsettingshome.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.PaymentSettingsHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.PaymentSettingsHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.PaymentSettingsHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="taxsettingshome.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.TaxSettingsHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.TaxSettingsHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.TaxSettingsHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="shippingsettingshome.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.ShippingSettingsHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.ShippingSettingsHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.ShippingSettingsHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="locationsettingshome.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.LocationSettingsHome.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.LocationSettingsHome.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.LocationSettingsHome.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="measures.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.Measures.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.Measures.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.Measures.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="acl.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.ACL.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.ACL.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.ACL.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="settings.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.Settings.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.Settings.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.Settings.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="smsproviders.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.SMSProviders.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.SMSProviders.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.SMSProviders.Description")%>
                    </p>
                </div>
            </li>
            <li>
                <div class="title">
                    <a href="thirdpartyintegration.aspx" title="<%=GetLocaleResourceString("Admin.ConfigurationHome.ThirdPartyIntegration.TitleDescription")%>">
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.ThirdPartyIntegration.Title")%></a>
                </div>
                <div class="description">
                    <p>
                        <%=GetLocaleResourceString("Admin.ConfigurationHome.ThirdPartyIntegration.Description")%>
                    </p>
                </div>
            </li>
        </ul>
    </div>
</div>
