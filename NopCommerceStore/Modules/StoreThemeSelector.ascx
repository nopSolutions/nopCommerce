<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.StoreThemeSelectorControl"
    CodeBehind="StoreThemeSelector.ascx.cs" %>
<%=GetLocaleResourceString("Common.SelectStoreTheme")%> <asp:DropDownList runat="server" ID="ddlTheme" runat="server" AutoPostBack="true"
    OnSelectedIndexChanged="ddlTheme_OnSelectedIndexChanged" CssClass="storethemelist"
    EnableViewState="false">
</asp:DropDownList>
