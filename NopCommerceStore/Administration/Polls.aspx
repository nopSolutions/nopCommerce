<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Polls" CodeBehind="Polls.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Polls" Src="Modules/Polls.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Polls runat="server" ID="ctrlPolls" />
</asp:Content>