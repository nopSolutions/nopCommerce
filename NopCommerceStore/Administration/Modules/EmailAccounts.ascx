<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.EmailAccountsControl"
    CodeBehind="EmailAccounts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.EmailAccounts.Title")%>" />
        <%=GetLocaleResourceString("Admin.EmailAccounts.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.EmailAccounts.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" />
        <input type="button" onclick="location.href='EmailAccountAdd.aspx'" value="<%=GetLocaleResourceString("Admin.EmailAccounts.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.EmailAccounts.AddNewButton.Tooltip")%>" />
    </div>
</div>

<asp:GridView ID="gvEmailAccounts" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.EmailAccounts.Email %>" ItemStyle-Width="30%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Email").ToString())%>
                <asp:HiddenField runat="server" ID="hfEmailAccountId" Value='<%#Eval("EmailAccountId")%>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.EmailAccounts.DisplayName %>" ItemStyle-Width="30%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("DisplayName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.EmailAccounts.IsDefault %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:GlobalRadioButton runat="server" ID="rdbIsDefault"
                    Checked='<%#Eval("IsDefault")%>' GroupName="DefaultEmailAccount"
                    ToolTip="<% $NopResources:Admin.EmailAccounts.IsDefault.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.EmailAccounts.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="EmailAccountDetails.aspx?EmailAccountID=<%#Eval("EmailAccountId")%>">
                    <%#GetLocaleResourceString("Admin.EmailAccounts.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>