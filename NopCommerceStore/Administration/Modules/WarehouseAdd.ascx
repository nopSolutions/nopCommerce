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
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.WarehouseAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.WarehouseAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:WarehouseInfo ID="ctrlWarehouseInfo" runat="server" />
