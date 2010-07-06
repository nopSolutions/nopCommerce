<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MessageQueueDetails"
    CodeBehind="MessageQueueDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="MessageQueueDetails" Src="Modules/MessageQueueDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:MessageQueueDetails runat="server" ID="ctrlMessageQueueDetails" />
</asp:Content>
