<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_NewsDetails"
    CodeBehind="NewsDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsDetails" Src="Modules/NewsDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:NewsDetails runat="server" ID="ctrlNewsDetails" />
</asp:Content>