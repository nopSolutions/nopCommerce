<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BulkEditProducts"
    CodeBehind="BulkEditProducts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="BulkEditProducts" Src="Modules/BulkEditProducts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BulkEditProducts runat="server" ID="ctrlBulkEditProducts" />
</asp:Content>
