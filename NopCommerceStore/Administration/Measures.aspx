<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Measures.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Measures" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="Measures" Src="Modules/Measures.ascx" %>
<asp:Content ID="mainContent" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Measures ID="ctrlMeasures" runat="server" />
</asp:Content>
