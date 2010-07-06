<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PollDetails"
    CodeBehind="PollDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PollDetails" Src="Modules/PollDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:PollDetails runat="server" ID="ctrlPollDetails" />
</asp:Content>