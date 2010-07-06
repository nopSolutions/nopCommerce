<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PollAdd"
    CodeBehind="PollAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PollAdd" Src="Modules/PollAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:PollAdd runat="server" ID="ctrlPollAdd" />
</asp:Content>