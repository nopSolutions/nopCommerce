<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CheckoutAttributeValuesControl"
    CodeBehind="CheckoutAttributeValues.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">
    <asp:UpdatePanel ID="upValues" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
                <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
            </asp:Panel>
            <div>
                <asp:GridView ID="gvValues" runat="server" AutoGenerateColumns="false" DataKeyNames="CheckoutAttributeValueId"
                    OnRowDeleting="gvValues_RowDeleting" OnRowDataBound="gvValues_RowDataBound" OnRowCommand="gvValues_RowCommand"
                    Width="100%">
                    <Columns>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.Name %>"
                            ItemStyle-Width="30%">
                            <ItemTemplate>
                                <%if (this.HasLocalizableContent)
                                  { %>
                                <div style="clear: both; padding-bottom: 15px;">
                                    <div style="width: 25%; float: left;">
                                        <%=GetLocaleResourceString("Admin.Localizable.Standard")%>:
                                    </div>
                                    <div style="width: 75%; float: left;">
                                        <%} %><nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtName"
                                            ValidationGroup="CheckoutAttributeValue" ErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.Name.ErrorMessage %>"
                                            Text='<%# Eval("Name") %>' Width="100%"></nopCommerce:SimpleTextBox>
                                        <asp:HiddenField ID="hfCheckoutAttributeValueId" runat="server" Value='<%# Eval("CheckoutAttributeValueId") %>' />
                                    </div>
                                </div>
                                <%if (this.HasLocalizableContent)
                                  { %>
                                <asp:Repeater ID="rptrLanguageDivs2" runat="server" OnItemDataBound="rptrLanguageDivs2_ItemDataBound">
                                    <ItemTemplate>
                                        <div style="clear: both; padding-bottom: 15px;">
                                            <div style="width: 25%; float: left;">
                                                <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                                                    AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                                                <%#Server.HtmlEncode(Eval("Name").ToString())%>:
                                            </div>
                                            <div style="width: 75%; float: left;">
                                                <asp:TextBox runat="server" ID="txtLocalizedName" CssClass="adminInput" Width="100%" />
                                                <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <%} %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.PriceAdjustment %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="50px" Value='<%# Eval("PriceAdjustment") %>'
                                    ID="txtPriceAdjustment" RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.PriceAdjustment.RequiredErrorMessage %>"
                                    MinimumValue="0" MaximumValue="100000000" ValidationGroup="CheckoutAttributeValue"
                                    RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.PriceAdjustment.RangeErrorMessage %>">
                                </nopCommerce:DecimalTextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.WeightAdjustment %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="50px" Value='<%# Eval("WeightAdjustment") %>'
                                    ID="txtWeightAdjustment" RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.WeightAdjustment.RequiredErrorMessage %>"
                                    MinimumValue="0" MaximumValue="999999" ValidationGroup="CheckoutAttributeValue"
                                    RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.WeightAdjustment.RangeErrorMessage %>">
                                </nopCommerce:DecimalTextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.IsPreSelected %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" Checked='<%# Eval("IsPreSelected") %>' ID="cbIsPreSelected">
                                </asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                                    Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder.RequiredErrorMessage %>"
                                    RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeInfo.DisplayOrder.RangeErrorMessage %>"
                                    ValidationGroup="CheckoutAttributeValue" MinimumValue="-99999" MaximumValue="99999">
                                </nopCommerce:NumericTextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.Update %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.CheckoutAttributeInfo.Update %>"
                                    ValidationGroup="CheckoutAttributeValue" CommandName="UpdateCheckoutAttributeValue" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CheckoutAttributeInfo.Delete %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Button ID="btnDelete" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.CheckoutAttributeInfo.Delete %>"
                                    CausesValidation="false" CommandName="Delete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upValues">
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
            <%=GetLocaleResourceString("Admin.CheckoutAttributeInfoValues.AddNew")%>
        </strong>
    </div>
    <%if (this.HasLocalizableContent)
      { %>
    <div id="localizablecontentpanel" class="tabcontainer-usual">
        <ul class="idTabs">
            <li class="idTab"><a href="#idTab_ValueNew1" class="selected">
                <%=GetLocaleResourceString("Admin.Localizable.Standard")%></a></li>
            <asp:Repeater ID="rptrLanguageTabs" runat="server">
                <ItemTemplate>
                    <li class="idTab"><a href="#idTab_ValueNew<%# Container.ItemIndex+2 %>">
                        <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                            AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                        <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <div id="idTab_ValueNew1" class="tab">
            <%} %>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeName" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.Name %>"
                            ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.Name.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtNewName" ValidationGroup="NewCheckoutAttributeValue"
                            ErrorMessage="Name is required"></nopCommerce:SimpleTextBox>
                    </td>
                </tr>
            </table>
            <%if (this.HasLocalizableContent)
              { %></div>
        <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
            <ItemTemplate>
                <div id="idTab_ValueNew<%# Container.ItemIndex+2 %>" class="tab">
                    <i>
                        <%=GetLocaleResourceString("Admin.Localizable.EmptyFieldNote")%></i>
                    <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                    <table class="adminContent">
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedAttributeName" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.Name %>"
                                    ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.Name.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:TextBox runat="server" CssClass="adminInput" ID="txtNewLocalizedName" />
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
                <nopCommerce:ToolTipLabel runat="server" ID="lblPriceAdjustment" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.PriceAdjustment %>"
                    ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.PriceAdjustment.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtNewPriceAdjustment"
                    Value="0" RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeValues.New.PriceAdjustment.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeValues.New.PriceAdjustment.RangeErrorMessage %>"
                    ValidationGroup="NewCheckoutAttributeValue"></nopCommerce:DecimalTextBox>
                [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblWeightAdjustment" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.WeightAdjustment %>"
                    ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.WeightAdjustment.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtNewWeightAdjustment"
                    Value="0" RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeValues.New.WeightAdjustment.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeValues.New.WeightAdjustment.RangeErrorMessage %>"
                    ValidationGroup="NewCheckoutAttributeValue"></nopCommerce:DecimalTextBox>
                [<%=MeasureManager.BaseWeightIn.Name%>]
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblPreSelected" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.PreSelected %>"
                    ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.PreSelected.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <asp:CheckBox runat="server" Checked="false" ID="cbNewIsPreSelected"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.DisplayOrder %>"
                    ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.DisplayOrder.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewDisplayOrder"
                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.CheckoutAttributeValues.New.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.CheckoutAttributeValues.New.DisplayOrder.RangeErrorMessage %>"
                    MinimumValue="-99999" MaximumValue="99999" ValidationGroup="NewCheckoutAttributeValue">
                </nopCommerce:NumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button runat="server" ID="btnAdd" CssClass="adminButton" Text="<% $NopResources:Admin.CheckoutAttributeValues.New.AddNewButton.Text %>"
                    ValidationGroup="NewCheckoutAttributeValue" OnClick="btnAdd_Click" ToolTip="<% $NopResources:Admin.CheckoutAttributeValues.New.AddNewButton.Tooltip %>" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <asp:Label runat="server" ID="lblMessage"></asp:Label>
</asp:Panel>
