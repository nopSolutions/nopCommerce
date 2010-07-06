<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ACL" CodeBehind="ACL.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ACL" Src="Modules/ACL.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ACL runat="server" ID="ctrlACL" />
</asp:Content>