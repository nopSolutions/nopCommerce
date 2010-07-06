<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductAttributesControl"
    CodeBehind="ProductAttributes.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductAttributes.ProductAttributes")%>" />
        <%=GetLocaleResourceString("Admin.ProductAttributes.ProductAttributes")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='ProductAttributeAdd.aspx'" value="<%=GetLocaleResourceString("Admin.ProductAttributes.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.ProductAttributes.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvProductAttributes" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductAttributes.Name %>"
            ItemStyle-Width="70%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductAttributes.Edit %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="ProductAttributeDetails.aspx?ProductAttributeID=<%#Eval("ProductAttributeId")%>"
                    title="<%#GetLocaleResourceString("Admin.ProductAttributes.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.ProductAttributes.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
