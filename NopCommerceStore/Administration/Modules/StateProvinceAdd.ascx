<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.StateProvinceAddControl"
    CodeBehind="StateProvinceAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="StateProvinceInfo" Src="StateProvinceInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.StateProvinceAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.StateProvinceAdd.Title")%>
        <a href="StateProvinces.aspx" title="<%=GetLocaleResourceString("Admin.StateProvinceAdd.BackToStates")%>">
            (<%=GetLocaleResourceString("Admin.StateProvinceAdd.BackToStates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.StateProvinceAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.StateProvinceAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.StateProvinceAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:StateProvinceInfo ID="ctrlStateProvinceInfo" runat="server" />
