<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.ReturnItemsPage" CodeBehind="ReturnItems.aspx.cs"
    ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ReturnItems" Src="~/Modules/ReturnItems.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ReturnItems ID="ctrlReturnItems" runat="server" />
</asp:Content>
