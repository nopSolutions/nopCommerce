<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TopicInfoControl"
    CodeBehind="TopicInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
<script type="text/javascript">
    $(document).ready(function () {
        togglePassword();
    });

    function togglePassword() {
        if (getE('<%=cbIsPasswordProtected.ClientID %>').checked) {
            $('#pnlPassword').show();
        }
        else {
            $('#pnlPassword').hide();
        }
    }
</script>
<ajaxToolkit:TabContainer runat="server" ID="TopicTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlInfo" HeaderText="<% $NopResources:Admin.TopicLocalizedDetails.Info %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblSystemName" Text="<% $NopResources:Admin.TopicInfo.SystemName %>"
                            ToolTip="<% $NopResources:Admin.TopicInfo.SystemName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" ID="txtSystemName" CssClass="adminInput"
                            ErrorMessage="<% $NopResources:Admin.TopicInfo.SystemName.ErrorMessage %>"></nopCommerce:SimpleTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblIsPasswordProtected" Text="<% $NopResources:Admin.TopicInfo.IsPasswordProtected %>"
                            ToolTip="<% $NopResources:Admin.TopicInfo.IsPasswordProtected.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox ID="cbIsPasswordProtected" runat="server"></asp:CheckBox>
                    </td>
                </tr>
                <tr id="pnlPassword">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPassword" Text="<% $NopResources:Admin.TopicInfo.Password %>"
                            ToolTip="<% $NopResources:Admin.TopicInfo.Password.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtPassword" CssClass="adminInput" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblIncludeInSitemap" Text="<% $NopResources:Admin.TopicInfo.IncludeInSitemap %>"
                            ToolTip="<% $NopResources:Admin.TopicInfo.IncludeInSitemap.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox ID="cbIncludeInSitemap" runat="server"></asp:CheckBox>
                    </td>
                </tr>
            </table>
            <div id="localizablecontentpanel" class="tabcontainer-usual">
                <ul class="idTabs">
                    <asp:Repeater ID="rptrLanguageTabs" runat="server">
                        <ItemTemplate>
                            <li class="idTab"><a href="#idTab_Info<%# Container.ItemIndex+2 %>">
                                <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                                    AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                                <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
                    <ItemTemplate>
                        <div id="idTab_Info<%# Container.ItemIndex+2 %>" class="tab">
                            <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                            <table class="adminContent">
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedTopicTitle" Text="<% $NopResources:Admin.TopicLocalizedDetails.LocalizedTopicTitle %>"
                                            ToolTip="<% $NopResources:Admin.TopicLocalizedDetails.LocalizedTopicTitle.Tooltip %>"
                                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <asp:TextBox runat="server" CssClass="adminInput" ID="txtTitle">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblBody" Text="<% $NopResources:Admin.TopicLocalizedDetails.Body %>"
                                            ToolTip="<% $NopResources:Admin.TopicLocalizedDetails.Body.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <nopCommerce:NopHTMLEditor ID="txtBody" runat="server" Height="350" />
                                    </td>
                                </tr>
                                <tr runat="server" id="pnlUrl">
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblUrlTooltip" Text="<% $NopResources:Admin.TopicInfo.Url %>"
                                            ToolTip="<% $NopResources:Admin.TopicInfo.Url.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <asp:HyperLink runat="server" ID="hlUrl" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlSEO" HeaderText="<% $NopResources:Admin.TopicLocalizedDetails.SEO %>">
        <ContentTemplate>
            <div id="localizablecontentpanel" class="tabcontainer-usual">
                <ul class="idTabs">
                    <asp:Repeater ID="rptrLanguageTabs_SEO" runat="server">
                        <ItemTemplate>
                            <li class="idTab"><a href="#idTab_SEO<%# Container.ItemIndex+2 %>">
                                <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                                    AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                                <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Repeater ID="rptrLanguageDivs_SEO" runat="server" OnItemDataBound="rptrLanguageDivs_SEO_ItemDataBound">
                    <ItemTemplate>
                        <div id="idTab_SEO<%# Container.ItemIndex+2 %>" class="tab">
                            <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                            <table class="adminContent">
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblTopicMetaKeywords" Text="<% $NopResources:Admin.TopicLocalizedDetails.MetaKeywords %>"
                                            ToolTip="<% $NopResources:Admin.TopicLocalizedDetails.MetaKeywords.ToolTip %>"
                                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <asp:TextBox ID="txtMetaKeywords" CssClass="adminInput" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblTopicMetaDescription" Text="<% $NopResources:Admin.TopicLocalizedDetails.MetaDescription %>"
                                            ToolTip="<% $NopResources:Admin.TopicLocalizedDetails.MetaDescription.ToolTip %>"
                                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <asp:TextBox ID="txtMetaDescription" CssClass="adminInput" runat="server" TextMode="MultiLine"
                                            Height="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblTopicMetaTitle" Text="<% $NopResources:Admin.TopicLocalizedDetails.MetaTitle %>"
                                            ToolTip="<% $NopResources:Admin.TopicLocalizedDetails.MetaTitle.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <asp:TextBox ID="txtMetaTitle" CssClass="adminInput" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>