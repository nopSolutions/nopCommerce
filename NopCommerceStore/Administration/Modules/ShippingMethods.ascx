<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ShippingMethodsControl" CodeBehind="ShippingMethods.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ShippingMethodsFilterControl" Src="ShippingMethodsFilterControl.ascx" %>
    
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingMethods.Title")%>" />
        <%=GetLocaleResourceString("Admin.ShippingMethods.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ShippingMethods.BtnSave.Text %>" CssClass="adminButtonBlue" ID="btnSave" OnClick="BtnSave_OnClick" ToolTip="<% $NopResources:Admin.ShippingMethods.BtnSave.Tooltip %>" />
        <input type="button" onclick="location.href='ShippingMethodAdd.aspx'" value="<%=GetLocaleResourceString("Admin.ShippingMethods.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.ShippingMethods.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvShippingMethods" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ShippingMethods.Name %>" ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ShippingMethods.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ShippingMethods.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="ShippingMethodDetails.aspx?ShippingMethodID=<%#Eval("ShippingMethodId")%>"
                    title="<%#GetLocaleResourceString("Admin.ShippingMethods.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.ShippingMethods.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<nopCommerce:ShippingMethodsFilterControl runat="server" ID="ctrlShippingMethodsFilterControl" />
