<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="AttributesHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.AttributesHome" %>

<%@ Register Src="Modules/AttributesHome.ascx" TagName="AttributesHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:AttributesHome ID="ctrlAttributesHome" runat="server" />
</asp:Content>
