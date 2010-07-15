<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.TaxDisplayTypeSelectorControl" Codebehind="TaxDisplayTypeSelector.ascx.cs" %>
<asp:DropDownList runat="server" ID="ddlTaxDisplayType" runat="server"
    AutoPostBack="true" OnSelectedIndexChanged="ddlTaxDisplayType_OnSelectedIndexChanged" CssClass="taxdisplaytypelist" EnableViewState="false">
</asp:DropDownList>
