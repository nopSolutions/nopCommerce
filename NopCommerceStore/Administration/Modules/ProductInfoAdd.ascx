<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductInfoAddControl"
    CodeBehind="ProductInfoAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
<script type="text/javascript">
    $(document).ready(function () {
        toggleGiftCard();
        toggleCustomerEntersPrice();
        toggleDownloadableProduct();
        toggleRecurring();
        toggleShipping();
        toggleManageStock();
    });

    function toggleGiftCard() {
        if (getE('<%=cbIsGiftCard.ClientID %>').checked) {
            $('#pnlGiftCardType').show();
        }
        else {
            $('#pnlGiftCardType').hide();
        }
    }

    function toggleCustomerEntersPrice() {
        if (getE('<%=cbCustomerEntersPrice.ClientID %>').checked) {
            $('#pnlMinimumCustomerEnteredPrice').show();
            $('#pnlMaximumCustomerEnteredPrice').show();
        }
        else {
            $('#pnlMinimumCustomerEnteredPrice').hide();
            $('#pnlMaximumCustomerEnteredPrice').hide();
        }
    }

    function toggleDownloadableProduct() {
        if (getE('<%=cbIsDownload.ClientID %>').checked) {

            $('#pnlUseDownloadURL').show();

            if (getE('<%=cbUseDownloadURL.ClientID %>').checked) {
                $('#pnlDownloadURL').show();
                $('#pnlDownloadFile').hide();
            }
            else {
                $('#pnlDownloadURL').hide();
                $('#pnlDownloadFile').show();
            }

            $('#pnlUnlimitedDownloads').show();
            if (getE('<%=cbUnlimitedDownloads.ClientID %>').checked) {
                $('#pnlMaxNumberOfDownloads').hide();
            }
            else {
                $('#pnlMaxNumberOfDownloads').show();
            }
            $('#pnlDownloadExpirationDays').show();
            $('#pnlDownloadActivationType').show();

            $('#pnlHasUserAgreement').show();
            if (getE('<%=cbHasUserAgreement.ClientID %>').checked) {
                $('#pnlUserAgreementText').show();
            }
            else {
                $('#pnlUserAgreementText').hide();
            }

            $('#pnlHasSampleDownload').show();

            if (getE('<%=cbHasSampleDownload.ClientID %>').checked) {
                $('#pnlUseSampleDownloadURL').show();

                if (getE('<%=cbUseSampleDownloadURL.ClientID %>').checked) {
                    $('#pnlSampleDownloadURL').show();
                    $('#pnlSampleDownloadFile').hide();
                }
                else {
                    $('#pnlSampleDownloadURL').hide();
                    $('#pnlSampleDownloadFile').show();
                }
            }
            else {
                $('#pnlUseSampleDownloadURL').hide();
                $('#pnlSampleDownloadURL').hide();
                $('#pnlSampleDownloadFile').hide();
            }
        }
        else {
            $('#pnlUseDownloadURL').hide();
            $('#pnlDownloadURL').hide();
            $('#pnlDownloadFile').hide();
            $('#pnlUnlimitedDownloads').hide();
            $('#pnlMaxNumberOfDownloads').hide();
            $('#pnlDownloadExpirationDays').hide();
            $('#pnlDownloadActivationType').hide();
            $('#pnlHasUserAgreement').hide();
            $('#pnlUserAgreementText').hide();
            $('#pnlHasSampleDownload').hide();
            $('#pnlUseSampleDownloadURL').hide();
            $('#pnlSampleDownloadURL').hide();
            $('#pnlSampleDownloadFile').hide();
        }
    }

    function toggleShipping() {
        if (getE('<%=cbIsShipEnabled.ClientID %>').checked) {
            $('#pnlFreeShipping').show();
            $('#pnlAdditionalShippingCharge').show();
            $('#pnlWeight').show();
            $('#pnlLength').show();
            $('#pnlWidth').show();
            $('#pnlHeight').show();
        }
        else {
            $('#pnlFreeShipping').hide();
            $('#pnlAdditionalShippingCharge').hide();
            $('#pnlWeight').hide();
            $('#pnlLength').hide();
            $('#pnlWidth').hide();
            $('#pnlHeight').hide();
        }
    }

    function toggleRecurring() {
        if (getE('<%=cbIsRecurring.ClientID %>').checked) {
            $('#pnlCycleLength').show();
            $('#pnlCyclePeriod').show();
            $('#pnlTotalCycles').show();
        }
        else {
            $('#pnlCycleLength').hide();
            $('#pnlCyclePeriod').hide();
            $('#pnlTotalCycles').hide();
        }
    }

    function toggleManageStock() {
        var selectedManageInventoryMethod = document.getElementById('<%=ddlManageStock.ClientID %>');
        var selectedManageInventoryMethodId = selectedManageInventoryMethod.options[selectedManageInventoryMethod.selectedIndex].value;
        if (selectedManageInventoryMethodId == 0) {
            $('#pnlStockQuantity').hide();
            $('#pnlDisplayStockAvailability').hide();
            $('#pnlDisplayStockQuantity').hide();
            $('#pnlMinStockQuantity').hide();
            $('#pnlLowStockActivity').hide();
            $('#pnlNotifyForQuantityBelow').hide();
            $('#pnlBackorders').hide();
        }
        else if (selectedManageInventoryMethodId == 1) {
            $('#pnlStockQuantity').show();
            $('#pnlDisplayStockAvailability').show();

            if (getE('<%=cbDisplayStockAvailability.ClientID %>').checked) {
                $('#pnlDisplayStockQuantity').show();
            }
            else {
                $('#pnlDisplayStockQuantity').hide();
            }

            $('#pnlMinStockQuantity').show();
            $('#pnlLowStockActivity').show();
            $('#pnlNotifyForQuantityBelow').show();
            $('#pnlBackorders').show();
        }
        else {
            $('#pnlStockQuantity').hide();
            $('#pnlDisplayStockAvailability').hide();
            $('#pnlDisplayStockQuantity').hide();
            $('#pnlMinStockQuantity').hide();
            $('#pnlLowStockActivity').hide();
            $('#pnlNotifyForQuantityBelow').hide();
            $('#pnlBackorders').hide();
        }
    }
</script>
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
                    <nopCommerce:ToolTipLabel runat="server" ID="lblProductName" Text="<% $NopResources:Admin.ProductInfo.ProductName %>"
                        ToolTip="<% $NopResources:Admin.ProductInfo.ProductName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtName" ErrorMessage="<% $NopResources:Admin.ProductInfo.ProductName.ErrorMessage %>">
                    </nopCommerce:SimpleTextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblShortDescription" Text="<% $NopResources:Admin.ProductInfo.ShortDescription %>"
                        ToolTip="<% $NopResources:Admin.ProductInfo.ShortDescription.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtShortDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                        Height="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblFullDescription" Text="<% $NopResources:Admin.ProductInfo.FullDescription %>"
                        ToolTip="<% $NopResources: Admin.ProductInfo.FullDescription.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:NopHTMLEditor ID="txtFullDescription" runat="server" Height="350" />
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
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedProductName" Text="<% $NopResources:Admin.ProductInfo.ProductName %>"
                                ToolTip="<% $NopResources:Admin.ProductInfo.ProductName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtLocalizedName">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedShortDescription" Text="<% $NopResources:Admin.ProductInfo.ShortDescription %>"
                                ToolTip="<% $NopResources:Admin.ProductInfo.ShortDescription.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedShortDescription" runat="server" CssClass="adminInput"
                                TextMode="MultiLine" Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedFullDescription" Text="<% $NopResources:Admin.ProductInfo.FullDescription %>"
                                ToolTip="<% $NopResources: Admin.ProductInfo.FullDescription.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NopHTMLEditor ID="txtLocalizedFullDescription" runat="server" Height="350" />
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblAdminComment" Text="<% $NopResources:Admin.ProductInfo.AdminComment %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AdminComment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAdminComment" runat="server" CssClass="adminInput" TextMode="MultiLine"
                Height="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductTemplate" Text="<% $NopResources:Admin.ProductInfo.ProductTemplate %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ProductTemplate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTemplate" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShowOnHomePage" Text="<% $NopResources:Admin.ProductInfo.ShowOnHomePage %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ShowOnHomePage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbShowOnHomePage" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductPublished" Text="<% $NopResources:Admin.ProductInfo.Published %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowCustomerReviews" Text="<% $NopResources:Admin.ProductInfo.AllowCustomerReviews %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AllowCustomerReviews.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowCustomerReviews" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowCustomerRatings" Text="<% $NopResources:Admin.ProductInfo.AllowCustomerRatings %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AllowCustomerRatings.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowCustomerRatings" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductTags" Text="<% $NopResources:Admin.ProductInfo.ProductTags %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ProductTags.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtProductTags" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSKU" Text="<% $NopResources:Admin.ProductInfo.SKU %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.SKU.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSKU" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblManufacturerPartNumber" Text="<% $NopResources:Admin.ProductInfo.ManufacturerPartNumber %>"
                ToolTip="<% $NopResources: Admin.ProductInfo.ManufacturerPartNumber.Tooltip%>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtManufacturerPartNumber" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPrice" Text="<% $NopResources:Admin.ProductInfo.Price %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Price.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtPrice" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.Price.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.Price.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOldPrice" Text="<% $NopResources:Admin.ProductInfo.OldPrice %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.OldPrice.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtOldPrice"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.OldPrice.RequiredErrorMessage%>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.OldPrice.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductCost" Text="<% $NopResources:Admin.ProductInfo.ProductCost %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ProductCost.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtProductCost"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.ProductCost.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.ProductCost.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisableBuyButton" Text="<% $NopResources:Admin.ProductInfo.DisableBuyButton %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.DisableBuyButton.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisableBuyButton" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCallForPrice" Text="<% $NopResources:Admin.ProductInfo.CallForPrice %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.CallForPrice.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCallForPrice" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerEntersPrice" Text="<% $NopResources:Admin.ProductInfo.CustomerEntersPrice %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.CustomerEntersPrice.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCustomerEntersPrice" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMinimumCustomerEnteredPrice">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMinimumCustomerEnteredPrice" Text="<% $NopResources:Admin.ProductInfo.MinimumCustomerEnteredPrice %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.MinimumCustomerEnteredPrice.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMinimumCustomerEnteredPrice"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.MinimumCustomerEnteredPrice.RequiredErrorMessage%>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.MinimumCustomerEnteredPrice.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr id="pnlMaximumCustomerEnteredPrice">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMaximumCustomerEnteredPrice" Text="<% $NopResources:Admin.ProductInfo.MaximumCustomerEnteredPrice %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.MaximumCustomerEnteredPrice.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMaximumCustomerEnteredPrice"
                Value="1000" RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.MaximumCustomerEnteredPrice.RequiredErrorMessage%>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.MaximumCustomerEnteredPrice.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAvailableStartDateTime" Text="<% $NopResources:Admin.ProductInfo.AvailableStartDateTime %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AvailableStartDateTime.ToolTip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlAvailableStartDateTimePicker" TargetControlID="txtAvailableStartDateTime" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAvailableEndDateTime" Text="<% $NopResources:Admin.ProductInfo.AvailableEndDateTime %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AvailableEndDateTime.ToolTip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlAvailableEndDateTimePicker" TargetControlID="txtAvailableEndDateTime" />
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsGiftCard" Text="<% $NopResources:Admin.ProductInfo.IsGiftCard %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.IsGiftCard.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsGiftCard" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlGiftCardType">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGiftCardType" Text="<% $NopResources:Admin.ProductInfo.GiftCardType %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.GiftCardType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlGiftCardType" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsDownload" Text="<% $NopResources:Admin.ProductInfo.IsDownload %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.IsDownload.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsDownload" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlUseDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUseDownloadURL" Text="<% $NopResources: Admin.ProductInfo.UseDownloadURL%>"
                ToolTip="<% $NopResources:Admin.ProductInfo.UseDownloadURL.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseDownloadURL" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadURL" Text="<% $NopResources: Admin.ProductInfo.DownloadURL%>"
                ToolTip="<% $NopResources: Admin.ProductInfo.DownloadURL.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDownloadURL" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlDownloadFile">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadFile" Text="<% $NopResources: Admin.ProductInfo.DownloadFile%>"
                ToolTip="<% $NopResources:Admin.ProductInfo.DownloadFile.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:FileUpload ID="fuProductVariantDownload" CssClass="adminInput" runat="server"
                ToolTip="Choose a file" />
        </td>
    </tr>
    <tr id="pnlUnlimitedDownloads">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUnlimitedDownloads" Text="<% $NopResources:Admin.ProductInfo.UnlimitedDownloads %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.UnlimitedDownloads.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUnlimitedDownloads" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMaxNumberOfDownloads">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMaxNumberOfDownloads" Text="<% $NopResources: Admin.ProductInfo.MaxNumberOfDownloads%>"
                ToolTip="<% $NopResources:Admin.ProductInfo.MaxNumberOfDownloads.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMaxNumberOfDownloads"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.MaxNumberOfDownloads.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.MaxNumberOfDownloads.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlDownloadExpirationDays">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadExpirationDays" Text="<% $NopResources: Admin.ProductInfo.DownloadExpirationDays %>"
                ToolTip="<% $NopResources: Admin.ProductInfo.DownloadExpirationDays.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDownloadExpirationDays" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlDownloadActivationType">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadActivationType" Text="<% $NopResources:Admin.ProductInfo.DownloadActivationType %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.DownloadActivationType.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlDownloadActivationType" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlHasUserAgreement">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel ID="lblHasUserAgreement" runat="server" Text="<% $NopResources:Admin.ProductInfo.CbHasUserAgreement.Info %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.CbHasUserAgreement.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            :
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbHasUserAgreement" Checked="false" />
        </td>
    </tr>
    <tr id="pnlUserAgreementText">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel ID="lblUserAgreementText" runat="server" Text="<% $NopResources:Admin.ProductInfo.TxtUserAgreementText.Info %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.TxtUserAgreementText.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            :
        </td>
        <td class="adminData">
            <nopCommerce:NopHTMLEditor runat="server" ID="txtUserAgreementText" Height="350" />
        </td>
    </tr>
    <tr id="pnlHasSampleDownload">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblHasSampleDownload" Text="<% $NopResources:Admin.ProductInfo.HasSampleDownload %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.HasSampleDownload.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbHasSampleDownload" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlUseSampleDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUseSampleDownloadURL" Text="<% $NopResources: Admin.ProductInfo.UseSampleDownloadURL%>"
                ToolTip="<% $NopResources: Admin.ProductInfo.UseSampleDownloadURL.Tooltip%>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSampleDownloadURL" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlSampleDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSampleDownloadURL" Text="<% $NopResources: Admin.ProductInfo.SampleDownloadURL%>"
                ToolTip="<% $NopResources:Admin.ProductInfo.SampleDownloadURL.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSampleDownloadURL" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlSampleDownloadFile">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSampleDownloadFile" Text="<% $NopResources:Admin.ProductInfo.SampleDownloadFile %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.SampleDownloadFile.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:FileUpload ID="fuProductVariantSampleDownload" CssClass="adminInput" runat="server"
                ToolTip="Choose a file" />
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsRecurring" Text="<% $NopResources:Admin.ProductInfo.IsRecurring %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.IsRecurring.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsRecurring" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlCycleLength">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCycleLength" Text="<% $NopResources: Admin.ProductInfo.CycleLength %>"
                ToolTip="<% $NopResources: Admin.ProductInfo.CycleLength.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtCycleLength"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.CycleLength.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="100" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.CycleLength.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlCyclePeriod">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCyclePeriod" Text="<% $NopResources:Admin.ProductInfo.CyclePeriod %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.CyclePeriod.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCyclePeriod" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlTotalCycles">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTotalCycles" Text="<% $NopResources: Admin.ProductInfo.TotalCycles %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.TotalCycles.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtTotalCycles"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.TotalCycles.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.TotalCycles.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShipEnabled" Text="<% $NopResources: Admin.ProductInfo.ShipEnabled%>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ShipEnabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsShipEnabled" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlFreeShipping">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFreeShipping" Text="<% $NopResources:Admin.ProductInfo.FreeShipping %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.FreeShipping.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsFreeShipping" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlAdditionalShippingCharge">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAdditionalShippingCharge" Text="<% $NopResources: Admin.ProductInfo.AdditionalShippingCharge%>"
                ToolTip="<% $NopResources:Admin.ProductInfo.AdditionalShippingCharge.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtAdditionalShippingCharge"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.AdditionalShippingCharge.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.AdditionalShippingCharge.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr id="pnlWeight">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWeight" Text="<% $NopResources:Admin.ProductInfo.Weight %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Weight.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtWeight" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.Weight.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.Weight.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox> [<%=MeasureManager.BaseWeightIn.Name%>]
        </td>
    </tr>
    <tr id="pnlLength">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLength" Text="<% $NopResources:Admin.ProductInfo.Length %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Length.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtLength" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.Length.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.Length.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=MeasureManager.BaseDimensionIn.Name%>]
        </td>
    </tr>
    <tr id="pnlWidth">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWidth" Text="<% $NopResources:Admin.ProductInfo.Width %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Width.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtWidth" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.Width.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.Width.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=MeasureManager.BaseDimensionIn.Name%>]
        </td>
    </tr>
    <tr id="pnlHeight">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblHeight" Text="<% $NopResources:Admin.ProductInfo.Height %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Height.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtHeight" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.Height.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.Height.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=MeasureManager.BaseDimensionIn.Name%>]
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxExempt" Text="<% $NopResources:Admin.ProductInfo.TaxExempt %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.TaxExempt.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsTaxExempt" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxCategory" Text="<% $NopResources:Admin.ProductInfo.TaxCategory %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.TaxCategory.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTaxCategory" CssClass="adminInput" AutoPostBack="False"
                runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblManageStock" Text="<% $NopResources:Admin.ProductInfo.ManageStock %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.ManageStock.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlManageStock" runat="server">
                <asp:ListItem Text="<% $NopResources:Admin.ManageInventoryMethod.DontManage %>" Value="0"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.ManageInventoryMethod.Manage %>" Value="1"
                    Selected="True"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.ManageInventoryMethod.ManageByAttributes %>"
                    Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlStockQuantity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStockQuantity" Text="<% $NopResources:Admin.ProductInfo.StockQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.StockQuantity.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtStockQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.StockQuantity.RequiredErrorMessage %>"
                MinimumValue="-999999" MaximumValue="999999" Value="10000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.StockQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlDisplayStockAvailability">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayStockAvailability" Text="<% $NopResources:Admin.ProductInfo.DisplayStockAvailability %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.DisplayStockAvailability.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisplayStockAvailability" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlDisplayStockQuantity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayStockQuantity" Text="<% $NopResources:Admin.ProductInfo.DisplayStockQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.DisplayStockQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisplayStockQuantity" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMinStockQuantity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMinStockQuantity" Text="<% $NopResources:Admin.ProductInfo.MinStockQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.MinStockQuantity.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMinStockQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.MinStockQuantity.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="0" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.MinStockQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlLowStockActivity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLowStockActivity" Text="<% $NopResources:Admin.ProductInfo.LowStockActivity %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.LowStockActivity.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLowStockActivity" AutoPostBack="False" CssClass="adminInput"
                runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlNotifyForQuantityBelow">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNotifyForQuantityBelow" Text="<% $NopResources:Admin.ProductInfo.NotifyForQuantityBelow %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.NotifyForQuantityBelow.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNotifyForQuantityBelow"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.NotifyForQuantityBelow.RequiredErrorMessage%>"
                MinimumValue="1" MaximumValue="999999" Value="1" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.NotifyForQuantityBelow.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlBackorders">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBackorders" Text="<% $NopResources:Admin.ProductInfo.Backorders %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.Backorders.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlBackorders" CssClass="adminInput" runat="server">
                <asp:ListItem Text="<% $NopResources:Admin.ProductBackorderMode.NoBackorders %>"
                    Value="0"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.ProductBackorderMode.AllowQtyBelow0 %>"
                    Value="1"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.ProductBackorderMode.AllowQtyBelow0AndNotifyCustomer %>"
                    Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderMinimumQuantity" Text="<% $NopResources:Admin.ProductInfo.OrderMinimumQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.OrderMinimumQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtOrderMinimumQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.OrderMinimumQuantity.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="1" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.OrderMinimumQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderMaximumQuantity" Text="<% $NopResources:Admin.ProductInfo.OrderMaximumQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductInfo.OrderMaximumQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtOrderMaximumQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductInfo.OrderMaximumQuantity.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="10000" RangeErrorMessage="<% $NopResources:Admin.ProductInfo.OrderMaximumQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWarehouse" Text="<% $NopResources:Admin.ProductInfo.Warehouse %>"
                ToolTip="<% $NopResources: Admin.ProductInfo.Warehouse.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlWarehouse" CssClass="adminInput" AutoPostBack="False" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
</table>
