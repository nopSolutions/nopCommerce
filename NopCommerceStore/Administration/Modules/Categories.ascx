<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoriesControl"
    CodeBehind="Categories.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.Categories.ManageCategories")%>" />
        <%=GetLocaleResourceString("Admin.Categories.ManageCategories")%>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Categories.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Categories.SaveButton.ToolTip %>" />
        <input type="button" onclick="location.href='CategoryAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Categories.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Categories.AddNewButton.ToolTip")%>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Categories.ExportXMLButton.Text %>" CssClass="adminButtonBlue" ID="btnExportXML"
            OnClick="btnExportXML_Click" ValidationGroup="ExportXML" ToolTip="<% $NopResources:Admin.Categories.ExportXMLButton.ToolTip %>" />
    </div>
</div>
<table class="adminContent">
    <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvCategories_PageIndexChanging" AllowPaging="true" PageSize="15">
        <Columns>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.Categories.Name %>" ItemStyle-Width="45%">
                <ItemTemplate>
                    <%# GetCategoryFullName((Category)Container.DataItem)%>
                    <asp:HiddenField ID="hfCategoryId" runat="server" Value='<%# Eval("CategoryId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.Categories.Edit %>" HeaderStyle-HorizontalAlign="Center"
                ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <a href="CategoryDetails.aspx?CategoryId=<%#Eval("CategoryId")%>" title="<%#GetLocaleResourceString("Admin.Categories.Edit.Tooltip")%>">
                        <%#GetLocaleResourceString("Admin.Categories.Edit")%>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.Categories.DisplayOrder %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" Width="70px" CssClass="adminInput"
                        RequiredErrorMessage="<% $NopResources:Admin.Categories.DisplayOrder.RequiredErrorMessage %>"
                        RangeErrorMessage="<% $NopResources:Admin.Categories.DisplayOrder.RangeErrorMessage %>"
                        MinimumValue="-99999" MaximumValue="99999" Value='<%# Eval("DisplayOrder") %>'>
                    </nopCommerce:NumericTextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.Categories.Published %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:CheckBox runat="server" Checked='<%# Eval("Published") %>' ID="cbPublished" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</table>