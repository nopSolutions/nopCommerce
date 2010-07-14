<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true" ValidateRequest="false"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SMSProviders"
    CodeBehind="SMSProviders.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="SMSProviders" Src="Modules/SMSProviders.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:SMSProviders runat="server" ID="ctrlSMSProviders" />
</asp:Content>
