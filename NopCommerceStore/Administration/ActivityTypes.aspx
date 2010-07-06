<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ActivityTypes"
    CodeBehind="ActivityTypes.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ActivityTypes" Src="Modules/ActivityTypes.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ActivityTypes runat="server" ID="ctrlActivityTypes" />
</asp:Content>
