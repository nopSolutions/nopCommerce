<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerRolesControl"
    CodeBehind="CustomerRoles.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-customers.png" alt="<%=GetLocaleResourceString("Admin.CustomerRoles.Title")%>" />
        <%=GetLocaleResourceString("Admin.CustomerRoles.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='CustomerRoleAdd.aspx'" value="<%=GetLocaleResourceString("Admin.CustomerRoles.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.CustomerRoles.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvCustomerRoles" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRoles.Name %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <a href="CustomerRoleDetails.aspx?CustomerRoleID=<%#Eval("CustomerRoleId")%>" title="<%#GetLocaleResourceString("Admin.CustomerRoles.Name.Tooltip")%>">
                    <%#Eval("Name")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="FreeShipping" HeaderText="<% $NopResources:Admin.CustomerRoles.FreeShipping %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="TaxExempt" HeaderText="<% $NopResources:Admin.CustomerRoles.TaxExempt %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRoles.Active %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbActive" Checked='<%# Eval("Active") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRoles.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="CustomerRoleDetails.aspx?CustomerRoleID=<%#Eval("CustomerRoleId")%>" title="<%#GetLocaleResourceString("Admin.CustomerRoles.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.CustomerRoles.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
