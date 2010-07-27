<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_OnlineCustomers"
    CodeBehind="OnlineCustomers.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="OnlineCustomers" Src="Modules/OnlineCustomers.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:OnlineCustomers runat="server" ID="ctrlOnlineCustomers" />
</asp:Content>
