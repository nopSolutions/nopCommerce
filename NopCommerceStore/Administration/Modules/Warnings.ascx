<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.WarningsControl"
    CodeBehind="Warnings.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-warnings.gif" alt="<%=GetLocaleResourceString("Admin.Warnings.Warnings")%>" />
        <%=GetLocaleResourceString("Admin.Warnings.Warnings")%>
    </div>
</div>
<div class="system-warnings">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer runat="server" ID="RefreshTimer" Interval="500" Enabled="true" OnTick="RefreshTimer_Tick" />
            <h3>
                Store URL</h3>
            <ul>
                <li>
                    <asp:Label CssClass="inprogress" Text="Specified store URL matches this store URL"
                        runat="server" ID="lblStoreUrl" />
                </li>
            </ul>
            <h3>
                Currencies</h3>
            <ul>
                <li>
                    <asp:Label CssClass="inprogress" ID="lblPrimaryExchangeRateCurrency" Text="Primary exchange rate currency is set"
                        runat="server" /></li>
                <li>
                    <asp:Label CssClass="inprogress" ID="lblPrimaryStoreCurrency" Text="Primary store currency is set"
                        runat="server" /></li>
            </ul>
            <h3>
                Measures</h3>
            <ul>
                <li>
                    <asp:Label CssClass="inprogress" ID="lblDefaultWeight" Text="Default weight is set"
                        runat="server" /></li>
                <li>
                    <asp:Label CssClass="inprogress" ID="lblDefaultDimension" Text="Default dimension is set"
                        runat="server" /></li>
            </ul>
            <h3>
                Templates</h3>
            <ul>
                <li>
                    <asp:Label CssClass="inprogress" ID="lblMessageTemplates" Text="All message templates exist"
                        runat="server" /></li>
            </ul>
            <h3>
                Shipping</h3>
            <ul>
                <li>
                    <asp:Label CssClass="inprogress" ID="lblShippingMethods" Text="Shipping methods"
                        runat="server" /></li>
            </ul>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
