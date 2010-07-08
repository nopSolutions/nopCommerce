<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SettingsControl"
    CodeBehind="Settings.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Settings.Title")%>" />
        <%=GetLocaleResourceString("Admin.Settings.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='SettingAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Settings.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Settings.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvSettings" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvSettings_PageIndexChanging" AllowPaging="true" PageSize="30">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Settings.Name %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Settings.Value %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Value").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Settings.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="SettingDetails.aspx?SettingID=<%#Eval("SettingId")%>" title="<%#GetLocaleResourceString("Admin.Settings.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Settings.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
</asp:GridView>
