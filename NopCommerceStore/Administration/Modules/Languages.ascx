<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LanguagesControl"
    CodeBehind="Languages.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Languages.Title")%>" />
        <%=GetLocaleResourceString("Admin.Languages.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='LanguageAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Languages.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Languages.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvLanguages" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Languages.Name %>" ItemStyle-Width="25%">
            <ItemTemplate>
                 <%#Server.HtmlEncode(Eval("Name").ToString())%> <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                        AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="LanguageCulture" HeaderText="<% $NopResources:Admin.Languages.LanguageCulture %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Languages.ViewResources %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="LocaleStringResources.aspx?LanguageID=<%#Eval("LanguageId")%>" title="<%#GetLocaleResourceString("Admin.Languages.ViewResources.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Languages.ViewResources")%></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="DisplayOrder" HeaderText="<% $NopResources:Admin.Languages.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Languages.Published %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbPublished" Checked='<%# Eval("Published") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Languages.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="LanguageDetails.aspx?LanguageID=<%#Eval("LanguageId")%>" title="<%#GetLocaleResourceString("Admin.Languages.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Languages.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
