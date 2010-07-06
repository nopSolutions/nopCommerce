<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ManufacturerSEOControl"
    CodeBehind="ManufacturerSEO.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%if (this.HasLocalizableContent)
  { %>
<div id="localizablecontentpanel" class="tabcontainer-usual">
    <ul class="idTabs">
        <li class="idTab"><a href="#idTab_SEO1" class="selected">
            <%=GetLocaleResourceString("Admin.Localizable.Standard")%></a></li>
        <asp:Repeater ID="rptrLanguageTabs" runat="server">
            <ItemTemplate>
                <li class="idTab"><a href="#idTab_SEO<%# Container.ItemIndex+2 %>">
                    <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                        AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                    <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <div id="idTab_SEO1" class="tab">
        <%} %>
        <table class="adminContent">
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblMetaKeywords" Text="<% $NopResources:Admin.ManufacturerSEO.MetaKeywords %>"
                        ToolTip="<% $NopResources:Admin.ManufacturerSEO.MetaKeywords.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtMetaKeywords" CssClass="adminInput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblMetaDescription" Text="<% $NopResources:Admin.ManufacturerSEO.MetaDescription %>"
                        ToolTip="<% $NopResources:Admin.ManufacturerSEO.MetaDescription.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtMetaDescription" CssClass="adminInput" runat="server" TextMode="MultiLine"
                        Height="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblMetaTitle" Text="<% $NopResources:Admin.ManufacturerSEO.MetaTitle %>"
                        ToolTip="<% $NopResources:Admin.ManufacturerSEO.MetaTitle.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtMetaTitle" runat="server" CssClass="adminInput"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblSEName" Text="<% $NopResources:Admin.ManufacturerSEO.SEName %>"
                        ToolTip="<% $NopResources:Admin.ManufacturerSEO.SEName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtSEName" runat="server" CssClass="adminInput"></asp:TextBox>
                </td>
            </tr>
        </table>
        <%if (this.HasLocalizableContent)
          { %></div>
    <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
        <ItemTemplate>
            <div id="idTab_SEO<%# Container.ItemIndex+2 %>" class="tab">
                <i>
                    <%=GetLocaleResourceString("Admin.Localizable.EmptyFieldNote")%></i>
                <table class="adminContent">
                    <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedMetaKeywords" Text="<% $NopResources:Admin.ManufacturerSEO.MetaKeywords %>"
                                ToolTip="<% $NopResources:Admin.ManufacturerSEO.MetaKeywords.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedMetaKeywords" CssClass="adminInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedMetaDescription" Text="<% $NopResources:Admin.ManufacturerSEO.MetaDescription %>"
                                ToolTip="<% $NopResources:Admin.ManufacturerSEO.MetaDescription.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedMetaDescription" CssClass="adminInput" runat="server"
                                TextMode="MultiLine" Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedMetaTitle" Text="<% $NopResources:Admin.ManufacturerSEO.MetaTitle %>"
                                ToolTip="<% $NopResources:Admin.ManufacturerSEO.MetaTitle.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedMetaTitle" runat="server" CssClass="adminInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedSEName" Text="<% $NopResources:Admin.ManufacturerSEO.SEName %>"
                                ToolTip="<% $NopResources:Admin.ManufacturerSEO.SEName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedSEName" runat="server" CssClass="adminInput"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<%} %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPageSize" Text="<% $NopResources:Admin.ManufacturerSEO.PageSize %>"
                ToolTip="<% $NopResources:Admin.ManufacturerSEO.PageSize.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtPageSize"
                RequiredErrorMessage="<% $NopResources:Admin.ManufacturerSEO.PageSize.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ManufacturerSEO.PageSize.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
