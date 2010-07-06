<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeasureWeightAdd.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_MeasureWeightAdd" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="MeasureWeightAdd" Src="Modules/MeasureWeightAdd.ascx" %>
<asp:Content ID="mainContent" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:MeasureWeightAdd ID="ctrlMeasureWeightAdd" runat="server" />
</asp:Content>
