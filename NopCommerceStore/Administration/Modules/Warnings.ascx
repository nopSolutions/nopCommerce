<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.WarningsControl"
    CodeBehind="Warnings.ascx.cs" %>
<div class="warnings">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-warnings.gif" alt="<%=GetLocaleResourceString("Admin.Warnings.Warnings")%>" />
            <%=GetLocaleResourceString("Admin.Warnings.Warnings")%>
        </div>
    </div>
    <div>
        <asp:Label runat="server" ID="lblWarnings" EnableViewState="false"></asp:Label>
    </div>
</div>
