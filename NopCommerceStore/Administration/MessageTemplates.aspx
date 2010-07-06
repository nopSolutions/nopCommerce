<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MessageTemplates"
    CodeBehind="MessageTemplates.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="MessageTemplates" Src="Modules/MessageTemplates.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:MessageTemplates runat="server" ID="ctrlMessageTemplates" />
</asp:Content>