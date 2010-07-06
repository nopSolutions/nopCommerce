<%@ Page Language="C#" MasterPageFile="~/Administration/popup.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PictureBrowser"
    CodeBehind="PictureBrowser.aspx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PictureBrowser" Src="Modules/PictureBrowser.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
   
    <nopCommerce:PictureBrowser ID="ctrlPictureBrowser" runat="server" />
</asp:Content>
