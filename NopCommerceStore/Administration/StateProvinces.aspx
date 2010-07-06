<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_StateProvinces"
    CodeBehind="StateProvinces.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="StateProvinces" Src="Modules/StateProvinces.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:StateProvinces runat="server" ID="ctrlStateProvinces" />
</asp:Content>
