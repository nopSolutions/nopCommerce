<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TaxCategoriesControl"
    CodeBehind="TaxCategories.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.TaxCategories.Title")%>" />
        <%=GetLocaleResourceString("Admin.TaxCategories.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='TaxCategoryAdd.aspx'" value="<%=GetLocaleResourceString("Admin.TaxCategories.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.TaxCategories.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvTaxCategories" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.TaxCategories.Name %>" ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources: Admin.TaxCategories.DisplayOrder%>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.TaxCategories.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="TaxCategoryDetails.aspx?TaxCategoryID=<%#Eval("TaxCategoryId")%>" title="<%#GetLocaleResourceString("Admin.TaxCategories.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.TaxCategories.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
