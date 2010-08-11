<%@ Page Language="C#" MasterPageFile="~/Administration/popup.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CrossSellProductAdd"
    CodeBehind="CrossSellProductAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CrossSellProductAdd" Src="Modules/CrossSellProductAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CrossSellProductAdd runat="server" ID="ctrlCrossSellProductAdd" />
</asp:Content>
