<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_StateProvinceAdd"
    CodeBehind="StateProvinceAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="StateProvinceAdd" Src="Modules/StateProvinceAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:StateProvinceAdd runat="server" ID="ctrlStateProvinceAdd" />
</asp:Content>
