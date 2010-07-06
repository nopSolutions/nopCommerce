<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Topics"
    CodeBehind="Topics.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Topics" Src="Modules/Topics.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Topics runat="server" ID="ctrlTopics" />
</asp:Content>