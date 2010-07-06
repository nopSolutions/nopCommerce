<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductTemplatesControl"
    CodeBehind="ProductTemplates.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ProductTemplates.Title")%>" />
        <%=GetLocaleResourceString("Admin.ProductTemplates.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='ProductTemplateAdd.aspx'" value="<%=GetLocaleResourceString("Admin.ProductTemplates.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.ProductTemplates.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvProductTemplates" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductTemplates.Name %>" ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductTemplates.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductTemplates.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="ProductTemplateDetails.aspx?ProductTemplateID=<%#Eval("ProductTemplateId")%>"
                    title="<%=GetLocaleResourceString("Admin.ProductTemplates.Edit.Tooltip")%>">
                    <%=GetLocaleResourceString("Admin.ProductTemplates.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
