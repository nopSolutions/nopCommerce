<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ManufacturerTemplatesControl"
    CodeBehind="ManufacturerTemplates.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ManufacturerTemplates.Title")%>" />
        <%=GetLocaleResourceString("Admin.ManufacturerTemplates.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='ManufacturerTemplateAdd.aspx'" value="<%=GetLocaleResourceString("Admin.ManufacturerTemplates.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.ManufacturerTemplates.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvManufacturerTemplates" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ManufacturerTemplates.Name %>"
            ItemStyle-Width="60%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ManufacturerTemplates.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ManufacturerTemplates.Edit %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="ManufacturerTemplateDetails.aspx?ManufacturerTemplateID=<%#Eval("ManufacturerTemplateId")%>"
                    title="<%=GetLocaleResourceString("Admin.ManufacturerTemplates.Edit.Tooltip")%>">
                    <%=GetLocaleResourceString("Admin.ManufacturerTemplates.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
