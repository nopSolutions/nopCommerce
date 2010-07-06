<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MessageTemplateDetails"
    ValidateRequest="false" CodeBehind="MessageTemplateDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="MessageTemplateDetails" Src="Modules/MessageTemplateDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:MessageTemplateDetails runat="server" ID="ctrlMessageTemplateDetails" />
</asp:Content>
