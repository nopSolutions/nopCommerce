<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CurrencySelectorControl"
    CodeBehind="CurrencySelector.ascx.cs" %>
<asp:DropDownList runat="server" ID="ddlCurrencies" AutoPostBack="true" OnSelectedIndexChanged="ddlCurrencies_OnSelectedIndexChanged"
    CssClass="currencylist" EnableViewState="false">
</asp:DropDownList>
