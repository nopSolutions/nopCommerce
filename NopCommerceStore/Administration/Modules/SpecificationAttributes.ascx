<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SpecificationAttributesControl"
    CodeBehind="SpecificationAttributes.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.SpecificationAttributes.Title")%>" />
        <%=GetLocaleResourceString("Admin.SpecificationAttributes.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='SpecificationAttributeAdd.aspx'" value="<%=GetLocaleResourceString("Admin.SpecificationAttributes.AddButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.SpecificationAttributes.AddButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvSpecificationAttributes" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.SpecificationAttributes.Name %>"
            ItemStyle-Width="70%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.SpecificationAttributes.Edit %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="SpecificationAttributeDetails.aspx?SpecificationAttributeID=<%#Eval("SpecificationAttributeId")%>"
                    title="<%#GetLocaleResourceString("Admin.SpecificationAttributes.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.SpecificationAttributes.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
