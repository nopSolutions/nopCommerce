<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LocaleStringResourcesControl"
    CodeBehind="LocaleStringResources.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.LocaleStringResources.Title")%>" />
        <%=GetLocaleResourceString("Admin.LocaleStringResources.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" ID="btnAddNew" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LocaleStringResources.AddNewButton.Text %>"
            OnClick="btnAddNew_Click" ToolTip="<% $NopResources:Admin.LocaleStringResources.AddNewButton.Tooltip %>" />
    </div>
</div>
<table>
    <tr>
        <td class="adminTitle">
            <%=GetLocaleResourceString("Admin.LocaleStringResources.Language")%>
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLanguage" runat="server" AutoPostBack="true" CssClass="adminInput"
                OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<br />
<asp:GridView ID="gvLocaleStringResources" runat="server" AutoGenerateColumns="False"
    Width="100%" OnPageIndexChanging="gvLocaleStringResources_PageIndexChanging"
    AllowPaging="true" PageSize="150">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.LocaleStringResources.LanguageColumn %>"
            ItemStyle-Width="20%">
            <ItemTemplate>
                <%#((Language)Eval("Language")).Name%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ResourceName" HeaderText="<% $NopResources:Admin.LocaleStringResources.ResourceNameColumn %>"
            ItemStyle-Width="30%"></asp:BoundField>
        <asp:BoundField DataField="ResourceValue" HeaderText="<% $NopResources:Admin.LocaleStringResources.ResourceValueColumn %>"
            ItemStyle-Width="40%"></asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.LocaleStringResources.EditColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="LocaleStringResourceDetails.aspx?LocaleStringResourceID=<%#Eval("LocaleStringResourceId")%>"
                    title="<%#GetLocaleResourceString("Admin.LocaleStringResources.EditColumn.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.LocaleStringResources.EditColumn")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
