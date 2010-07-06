<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.MessageTemplatesControl"
    CodeBehind="MessageTemplates.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-title">
    <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.MessageTemplates.Title")%>" />
    <%=GetLocaleResourceString("Admin.MessageTemplates.Title")%>
</div>
<p>
</p>
<asp:GridView ID="gvMessageTemplates" runat="server" AutoGenerateColumns="False"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageTemplates.Name %>" ItemStyle-Width="70%">
            <ItemTemplate>
                <%#Eval("Name")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.MessageTemplates.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="MessageTemplateDetails.aspx?MessageTemplateID=<%#Eval("MessageTemplateId")%>"
                    title="<%=GetLocaleResourceString("Admin.MessageTemplates.Edit.Tooltip")%>">
                    <%=GetLocaleResourceString("Admin.MessageTemplates.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
