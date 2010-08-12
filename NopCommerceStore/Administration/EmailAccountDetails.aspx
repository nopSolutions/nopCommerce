<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_EmailAccountDetails"
    CodeBehind="EmailAccountDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="EmailAccountDetails" Src="Modules/EmailAccountDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:EmailAccountDetails runat="server" ID="ctrlEmailAccountDetails" />
</asp:Content>
