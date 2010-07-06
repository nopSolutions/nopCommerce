<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_AccessDenied"
    CodeBehind="AccessDenied.aspx.cs" %>

<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-title">
        <img src="Common/ico-dashboard.png" alt="<%=GetLocaleResourceString("Admin.AccessDenied.Title")%>" />
        <%=GetLocaleResourceString("Admin.AccessDenied.Title")%>
    </div>
    <div class="messageBox messageBoxError">
         <%=GetLocaleResourceString("Admin.AccessDenied.Description")%>
    </div>
</asp:Content>
