<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.WarehousesControl"
    CodeBehind="Warehouses.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Warehouses.Title")%>" />
        <%=GetLocaleResourceString("Admin.Warehouses.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='WarehouseAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Warehouses.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Warehouses.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvWarehouses" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Warehouses.Name %>" ItemStyle-Width="80%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Warehouses.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="WarehouseDetails.aspx?WarehouseID=<%#Eval("WarehouseId")%>" title="<%#GetLocaleResourceString("Admin.Warehouses.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Warehouses.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
