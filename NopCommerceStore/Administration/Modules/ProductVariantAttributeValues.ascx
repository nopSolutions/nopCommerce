<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantAttributeValuesControl"
    CodeBehind="ProductVariantAttributeValues.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="" />
        <asp:Label ID="lblTitle" runat="server" />
        <asp:HyperLink runat="server" ID="hlProductURL" Text="<% $NopResources:Admin.ProductVariantAttributeValues.BackToProductDetails %>"
            ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.BackToProductDetails %>" />
    </div>
</div>
<asp:GridView ID="gvProductVariantAttributeValues" runat="server" AutoGenerateColumns="false"
    DataKeyNames="ProductVariantAttributeValueId" OnRowDeleting="gvProductVariantAttributeValues_RowDeleting"
    OnRowDataBound="gvProductVariantAttributeValues_RowDataBound" OnRowCommand="gvProductVariantAttributeValues_RowCommand"
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.Name %>"
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
                            ValidationGroup="ProductVariantAttributeValue" ErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.Name.ErrorMessage %>"
                            Text='<%# Eval("Name") %>' Width="100%"></nopCommerce:SimpleTextBox>
                        <asp:HiddenField ID="hfProductVariantAttributeValueId" runat="server" Value='<%# Eval("ProductVariantAttributeValueId") %>' />
                    </div>
                </div>
                <%if (this.HasLocalizableContent)
                  { %>
                <asp:Repeater ID="rptrLanguageDivs2" runat="server" OnItemDataBound="rptrLanguageDivs2_ItemDataBound">
                    <ItemTemplate>
                        <div style="clear: both; padding-bottom: 15px;">
                            <div style="width: 25%; float: left;">
                                <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                                    AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' /> <%#Server.HtmlEncode(Eval("Name").ToString())%>:
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
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.PriceAdjustment %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="50px" Value='<%# Eval("PriceAdjustment") %>'
                    ID="txtPriceAdjustment" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.PriceAdjustment.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="100000000" ValidationGroup="ProductVariantAttributeValue"
                    RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.PriceAdjustment.RangeErrorMessage %>">
                </nopCommerce:DecimalTextBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.WeightAdjustment %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="50px" Value='<%# Eval("WeightAdjustment") %>'
                    ID="txtWeightAdjustment" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.WeightAdjustment.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="999999" ValidationGroup="ProductVariantAttributeValue"
                    RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.WeightAdjustment.RangeErrorMessage %>">
                </nopCommerce:DecimalTextBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.IsPreSelected %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:CheckBox runat="server" Checked='<%# Eval("IsPreSelected") %>' ID="cbIsPreSelected"
                    ValidationGroup="ProductVariantAttributeValue"></asp:CheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                    Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.DisplayOrder.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.DisplayOrder.RangeErrorMessage %>"
                    ValidationGroup="ProductVariantAttributeValue" MinimumValue="-99999" MaximumValue="99999">
                </nopCommerce:NumericTextBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.Update %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantAttributeValues.Update %>"
                    ValidationGroup="ProductVariantAttributeValue" CommandName="UpdateProductVariantAttributeValue" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributeValues.Delete %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="btnDeleteProductVariantAttribute" runat="server" CssClass="adminButton"
                    Text="<% $NopResources:Admin.ProductVariantAttributeValues.Delete %>" CausesValidation="false"
                    CommandName="Delete" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<p>
    <strong>
        <%=GetLocaleResourceString("Admin.ProductVariantAttributeValues.New.AddNew")%>
    </strong>
</p>
<%if (this.HasLocalizableContent)
  { %>
<div id="localizablecontentpanel" class="tabcontainer-usual">
    <ul class="idTabs">
        <li class="idTab"><a href="#idTab_Info1" class="selected">
            <%=GetLocaleResourceString("Admin.Localizable.Standard")%></a></li>
        <asp:Repeater ID="rptrLanguageTabs" runat="server">
            <ItemTemplate>
                <li class="idTab"><a href="#idTab_Info<%# Container.ItemIndex+2 %>">
                    <asp:Image runat="server" ID="imgCFlag" Visible='<%# !String.IsNullOrEmpty(Eval("IconURL").ToString()) %>'
                        AlternateText='<%#Eval("Name")%>' ImageUrl='<%#Eval("IconURL").ToString()%>' />
                    <%#Server.HtmlEncode(Eval("Name").ToString())%></a></li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <div id="idTab_Info1" class="tab">
        <%} %>
        <table class="adminContent">
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeName" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.Name %>"
                        ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.Name.Tooltip %>"
                        ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtNewName" ValidationGroup="NewProductVariantAttributeValue"
                        ErrorMessage="Name is required"></nopCommerce:SimpleTextBox>
                </td>
            </tr>
        </table>
        <%if (this.HasLocalizableContent)
          { %></div>
    <asp:Repeater ID="rptrLanguageDivs" runat="server" OnItemDataBound="rptrLanguageDivs_ItemDataBound">
        <ItemTemplate>
            <div id="idTab_Info<%# Container.ItemIndex+2 %>" class="tab">
                <i>
                    <%=GetLocaleResourceString("Admin.Localizable.EmptyFieldNote")%></i>
                <asp:Label ID="lblLanguageId" runat="server" Text='<%#Eval("LanguageId") %>' Visible="false"></asp:Label>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedAttributeName" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.Name %>"
                                ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.Name.Tooltip %>"
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblPriceAdjustment" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.PriceAdjustment %>"
                ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.PriceAdjustment.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtNewPriceAdjustment"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.New.PriceAdjustment.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.New.PriceAdjustment.RangeErrorMessage %>"
                ValidationGroup="NewProductVariantAttributeValue"></nopCommerce:DecimalTextBox> [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWeightAdjustment" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.WeightAdjustment %>"
                ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.WeightAdjustment.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtNewWeightAdjustment"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.New.WeightAdjustment.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.New.WeightAdjustment.RangeErrorMessage %>"
                ValidationGroup="NewProductVariantAttributeValue"></nopCommerce:DecimalTextBox> [<%=MeasureManager.BaseWeightIn.Name%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPreSelected" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.PreSelected %>"
                ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.PreSelected.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" Checked="false" ID="cbNewIsPreSelected" ValidationGroup="NewProductVariantAttributeValue">
            </asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.DisplayOrder.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewDisplayOrder"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.New.DisplayOrder.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributeValues.New.DisplayOrder.RangeErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999" ValidationGroup="NewProductVariantAttributeValue">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button runat="server" ID="btnAdd" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantAttributeValues.New.AddNewButton.Text %>"
                ValidationGroup="NewProductVariantAttributeValue" OnClick="btnAdd_Click" ToolTip="<% $NopResources:Admin.ProductVariantAttributeValues.New.AddNewButton.Tooltip %>" />
        </td>
    </tr>
</table>
