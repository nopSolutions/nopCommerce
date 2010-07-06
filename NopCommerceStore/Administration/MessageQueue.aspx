<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MessageQueue"
    CodeBehind="MessageQueue.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="MessageQueue" Src="Modules/MessageQueue.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:MessageQueue runat="server" ID="ctrlMessageQueue" />
</asp:Content>
