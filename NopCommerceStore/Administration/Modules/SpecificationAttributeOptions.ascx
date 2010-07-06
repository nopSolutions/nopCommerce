<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SpecificationAttributeOptionsControl"
    CodeBehind="SpecificationAttributeOptions.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">
    <asp:UpdatePanel ID="upOptions" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
                <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
            </asp:Panel>
            <div>
                <asp:GridView ID="grdSpecificationAttributeOptions" runat="server" AutoGenerateColumns="false"
                    DataKeyNames="SpecificationAttributeOptionId" OnRowDeleting="OnSpecificationAttributeOptionsDeleting"
                    OnRowCommand="OnSpecificationAttributeOptionsCommand" OnRowDataBound="OnSpecificationAttributeOptionsDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.SpecificationAttributeOptions.Name %>">
                            <ItemTemplate>
                                <%if (this.HasLocalizableContent)
                                  { %>
                                <div style="clear: both; padding-bottom: 15px;">
                                    <div style="width: 15%; float: left;">
                                        <%=GetLocaleResourceString("Admin.Localizable.Standard")%>:
                                    </div>
                                    <div style="width: 85%; float: left;">
                                        <%} %><nopCommerce:SimpleTextBox runat="server" ID="txtOptionName" ErrorMessage="<% $NopResources:Admin.SpecificationAttributeOptions.Name.ErrorMessage %>"
                                            Text='<%# Eval("Name") %>' ValidationGroup="SpecificationAttributeOption" CssClass="adminInput"
                                            Width="100%" />
                                        <asp:HiddenField ID="hfSpecificationAttributeOptionId" runat="server" Value='<%# Eval("SpecificationAttributeOptionId") %>' />
                                    </div>
                                </div>
                                <%if (this.HasLocalizableContent)
                                  { %>
                                <asp:Repeater ID="rptrLanguageDivs2" runat="server" OnItemDataBound="rptrLanguageDivs2_ItemDataBound">
                                    <ItemTemplate>
                                        <div style="clear: both; padding-bottom: 15px;">
                                            <div style="width: 15%; float: left;">
                                                <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                                                    AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                                                <%#Server.HtmlEncode(Eval("Name").ToString())%>:
                                            </div>
                                            <div style="width: 85%; float: left;">
                                                <asp:TextBox runat="server" ID="txtLocalizedOptionName" CssClass="adminInput" Width="100%" />
                                                <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <%} %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.SpecificationAttributeOptions.DisplayOrder %>"
                            ItemStyle-Width="10%">
                            <ItemTemplate>
                                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtOptionDisplayOrder"
                                    Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.SpecificationAttributeOptions.DisplayOrder.RequiredErrorMessage %>"
                                    RangeErrorMessage="<% $NopResources:Admin.SpecificationAttributeOptions.DisplayOrder.RangeErrorMessage %>"
                                    ValidationGroup="SpecificationAttributeOption" MinimumValue="-99999" MaximumValue="99999" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.SpecificationAttributeOptions.Update %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.SpecificationAttributeOptions.Update %>"
                                    ValidationGroup="SpecificationAttributeOption" CommandName="UpdateOption" ToolTip="<% $NopResources:Admin.SpecificationAttributeOptions.Update.Tooltip %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.SpecificationAttributeOptions.Delete %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnDelete" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.SpecificationAttributeOptions.Delete %>"
                                    CausesValidation="false" CommandName="Delete" ToolTip="<% $NopResources:Admin.SpecificationAttributeOptions.Delete.Tooltip %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upOptions">
        <ProgressTemplate>
            <div class="progress">
                <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                    AlternateText="update" />
                <%=GetLocaleResourceString("Admin.Common.Wait...")%>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <div>
        <br />
        <strong>
            <%=GetLocaleResourceString("Admin.SpecificationAttributeOptions.AddNew")%>
        </strong>
    </div>
    <%if (this.HasLocalizableContent)
      { %>
    <div id="localizablecontentpanel" class="tabcontainer-usual">
        <ul class="idTabs">
            <li class="idTab"><a href="#idTab_OptionNew1" class="selected">
                <%=GetLocaleResourceString("Admin.Localizable.Standard")%></a></li>
            <asp:Repeater ID="rptrLanguageTabs" runat="server">
                <ItemTemplate>
                    <li class="idTab"><a href="#idTab_OptionNew<%# Container.ItemIndex+2 %>">
                        <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                            AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                        <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <div id="idTab_OptionNew1" class="tab">
            <%} %>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOptionName" Text="<% $NopResources:Admin.SpecificationAttributeOptions.New.Name %>"
                            ToolTip="<% $NopResources:Admin.SpecificationAttributeOptions.New.Name.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" ID="txtNewOptionName" ValidationGroup="NewSpecOption"
                            CssClass="adminInput" />
                    </td>
                </tr>
            </table>
            <%if (this.HasLocalizableContent)
              { %></div>
        <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
            <ItemTemplate>
                <div id="idTab_OptionNew<%# Container.ItemIndex+2 %>" class="tab">
                    <i>
                        <%=GetLocaleResourceString("Admin.Localizable.EmptyFieldNote")%></i>
                    <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                    <table class="adminContent">
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedOptionName" Text="<% $NopResources:Admin.SpecificationAttributeOptions.New.Name %>"
                                    ToolTip="<% $NopResources:Admin.SpecificationAttributeOptions.New.Name.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:TextBox runat="server" ID="txtNewLocalizedOptionName" CssClass="adminInput" />
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
                <nopCommerce:ToolTipLabel runat="server" ID="lblOptionDisplayOrder" Text="<% $NopResources:Admin.SpecificationAttributeOptions.New.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.SpecificationAttributeOptions.New.DisplayOrder.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewOptionDisplayOrder"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.SpecificationAttributeOptions.New.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.SpecificationAttributeOptions.New.DisplayOrder.RangeErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" ValidationGroup="NewSpecOption" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button runat="server" ID="btnAdd" CssClass="adminButton" Text="<% $NopResources:Admin.SpecificationAttributeOptions.New.AddNewButton.Text %>"
                    ValidationGroup="NewSpecOption" OnClick="btnAdd_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <asp:Label runat="server" ID="lblMessage"></asp:Label>
</asp:Panel>
