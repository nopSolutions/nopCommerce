<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_OrderDetails"
    CodeBehind="OrderDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="OrderDetails" Src="Modules/OrderDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:OrderDetails runat="server" ID="ctrlOrderDetails" />
</asp:Content>
