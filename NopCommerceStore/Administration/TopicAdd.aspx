<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TopicAdd"
    CodeBehind="TopicAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TopicAdd" Src="Modules/TopicAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TopicAdd runat="server" ID="ctrlTopicAdd" />
</asp:Content>
