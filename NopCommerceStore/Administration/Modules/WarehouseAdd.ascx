<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.WarehouseAddControl"
    CodeBehind="WarehouseAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="WarehouseInfo" Src="WarehouseInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.WarehouseAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.WarehouseAdd.Title")%>
        <a href="Warehouses.aspx" title="<%=GetLocaleResourceString("Admin.WarehouseAdd.BackToWarehouses")%>">
            (<%=GetLocaleResourceString("Admin.WarehouseAdd.BackToWarehouses")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.WarehouseAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.WarehouseAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.WarehouseAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:WarehouseInfo ID="ctrlWarehouseInfo" runat="server" />
