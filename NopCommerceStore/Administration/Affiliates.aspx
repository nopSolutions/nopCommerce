<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Affiliates"
    CodeBehind="Affiliates.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Affiliates" Src="Modules/Affiliates.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:Affiliates runat="server" ID="ctrlAffiliates" />
</asp:Content>
