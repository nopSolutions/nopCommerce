<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryTemplatesControl"
    CodeBehind="CategoryTemplates.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.CategoryTemplates.Title")%>" />
        <%=GetLocaleResourceString("Admin.CategoryTemplates.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='CategoryTemplateAdd.aspx'" value="<%=GetLocaleResourceString("Admin.CategoryTemplates.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.CategoryTemplates.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvCategoryTemplates" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryTemplates.Name %>"
            ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryTemplates.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CategoryTemplates.Edit %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="CategoryTemplateDetails.aspx?CategoryTemplateID=<%#Eval("CategoryTemplateId")%>"
                    title="<%=GetLocaleResourceString("Admin.CategoryTemplates.Edit.Tooltip")%>">
                    <%=GetLocaleResourceString("Admin.CategoryTemplates.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
