<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_StateProvinceDetails"
    CodeBehind="StateProvinceDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="StateProvinceDetails" Src="Modules/StateProvinceDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:StateProvinceDetails runat="server" ID="ctrlStateProvinceDetails" />
</asp:Content>
