<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantInfoControl"
    CodeBehind="ProductVariantInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
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
                    <nopCommerce:ToolTipLabel runat="server" ID="lblProductVariantName" Text="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName %>"
                        ToolTip="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName.Tooltip %>"
                        ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtName" runat="server" CssClass="adminInput"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.ProductVariantInfo.Description %>"
                        ToolTip="<% $NopResources:Admin.ProductVariantInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:NopHTMLEditor ID="txtDescription" runat="server" Height="350" />
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
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedProductVariantName" Text="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName %>"
                                ToolTip="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedName" runat="server" CssClass="adminInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblLocalizedDescription" Text="<% $NopResources:Admin.ProductVariantInfo.Description %>"
                                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NopHTMLEditor ID="txtLocalizedDescription" runat="server" Height="350" />
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<%} %>
<table class="adminContent">
    <tr runat="server" id="pnlProductVariantId">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.ProductVariantInfo.ID %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.ID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblProductVariantId" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSKU" Text="<% $NopResources:Admin.ProductVariantInfo.SKU %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.SKU.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSKU" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblImage" Text="<% $NopResources:Admin.ProductVariantInfo.Image %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Image.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Image ID="iProductVariantPicture" runat="server" />
            <br />
            <asp:Button ID="btnRemoveProductVariantImage" CssClass="adminButton" CausesValidation="false"
                runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.RemoveImage %>"
                OnClick="btnRemoveProductVariantImage_Click" Visible="false" />
            <br />
            <asp:FileUpload ID="fuProductVariantPicture" CssClass="adminInput" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAdminComment" Text="<% $NopResources:Admin.ProductVariantInfo.AdminComment %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.AdminComment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAdminComment" runat="server" CssClass="adminInput" TextMode="MultiLine"
                Height="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblManufacturerPartNumber" Text="<% $NopResources:Admin.ProductVariantInfo.ManufacturerPartNumber %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.ManufacturerPartNumber.Tooltip %>"
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblPrice" Text="<% $NopResources:Admin.ProductVariantInfo.Price %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Price.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtPrice" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Price.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Price.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOldPrice" Text="<% $NopResources:Admin.ProductVariantInfo.OldPrice %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.OldPrice.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtOldPrice"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.OldPrice.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.OldPrice.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductCost" Text="<% $NopResources:Admin.ProductVariantInfo.ProductCost %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.ProductCost.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtProductCost"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.ProductCost.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.ProductCost.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisableBuyButton" Text="<% $NopResources:Admin.ProductVariantInfo.DisableBuyButton %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DisableBuyButton.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisableBuyButton" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerEntersPrice" Text="<% $NopResources:Admin.ProductVariantInfo.CustomerEntersPrice %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.CustomerEntersPrice.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCustomerEntersPrice" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMinimumCustomerEnteredPrice">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMinimumCustomerEnteredPrice" Text="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMinimumCustomerEnteredPrice"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.RequiredErrorMessage%>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlMaximumCustomerEnteredPrice">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMaximumCustomerEnteredPrice" Text="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMaximumCustomerEnteredPrice"
                Value="1000" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.RequiredErrorMessage%>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAvailableStartDateTime" Text="<% $NopResources:Admin.ProductVariantInfo.AvailableStartDateTime %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.AvailableStartDateTime.ToolTip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlAvailableStartDateTimePicker" TargetControlID="txtAvailableStartDateTime" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAvailableEndDateTime" Text="<% $NopResources:Admin.ProductVariantInfo.AvailableEndDateTime %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.AvailableEndDateTime.ToolTip %>"
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsGiftCard" Text="<% $NopResources:Admin.ProductVariantInfo.IsGiftCard %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.IsGiftCard.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsGiftCard" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlGiftCardType">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGiftCardType" Text="<% $NopResources:Admin.ProductVariantInfo.GiftCardType %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.GiftCardType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsDownload" Text="<% $NopResources:Admin.ProductVariantInfo.IsDownload %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.IsDownload.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsDownload" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlUseDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUseDownloadURL" Text="<% $NopResources:Admin.ProductVariantInfo.UseDownloadURL %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.UseDownloadURL.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseDownloadURL" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadURL" Text="<% $NopResources:Admin.ProductVariantInfo.DownloadURL %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DownloadURL.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDownloadURL" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlDownloadFile">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadFile" Text="<% $NopResources:Admin.ProductVariantInfo.DownloadFile %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DownloadFile.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:HyperLink ID="hlProductVariantDownload" runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.DownloadFile.Download %>" />
            <br />
            <asp:Button ID="btnRemoveProductVariantDownload" CssClass="adminButton" CausesValidation="false"
                runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.DownloadFile.Remove %>"
                OnClick="btnRemoveProductVariantDownload_Click" Visible="false" />
            <br />
            <asp:FileUpload ID="fuProductVariantDownload" CssClass="adminInput" runat="server" />
        </td>
    </tr>
    <tr id="pnlUnlimitedDownloads">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUnlimitedDownloads" Text="<% $NopResources:Admin.ProductVariantInfo.UnlimitedDownloads %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.UnlimitedDownloads.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUnlimitedDownloads" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMaxNumberOfDownloads">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMaxNumberOfDownloads" Text="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMaxNumberOfDownloads"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlDownloadExpirationDays">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadExpirationDays" Text="<% $NopResources: Admin.ProductVariantInfo.DownloadExpirationDays %>"
                ToolTip="<% $NopResources: Admin.ProductVariantInfo.DownloadExpirationDays.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDownloadExpirationDays" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlDownloadActivationType">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDownloadActivationType" Text="<% $NopResources:Admin.ProductVariantInfo.DownloadActivationType %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DownloadActivationType.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlDownloadActivationType" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlHasUserAgreement">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel ID="lblHasUserAgreement" runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.CbHasUserAgreement.Info %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.CbHasUserAgreement.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            :
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbHasUserAgreement" Checked="false" />
        </td>
    </tr>
    <tr id="pnlUserAgreementText">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel ID="lblUserAgreementText" runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.TxtUserAgreementText.Info %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.TxtUserAgreementText.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            :
        </td>
        <td class="adminData">
            <nopCommerce:NopHTMLEditor runat="server" ID="txtUserAgreementText" Height="350" />
        </td>
    </tr>
    <tr id="pnlHasSampleDownload">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblHasSampleDownload" Text="<% $NopResources:Admin.ProductVariantInfo.HasSampleDownload %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.HasSampleDownload.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbHasSampleDownload" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlUseSampleDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUseSampleDownloadURL" Text="<% $NopResources:Admin.ProductVariantInfo.UseSampleDownloadURL %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.UseSampleDownloadURL.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSampleDownloadURL" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlSampleDownloadURL">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSampleDownloadURL" Text="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadURL %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadURL.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSampleDownloadURL" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlSampleDownloadFile">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSampleDownloadFile" Text="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadFile %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadFile.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:HyperLink ID="hlProductVariantSampleDownload" runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadFile.Download %>" />
            <br />
            <asp:Button ID="btnRemoveProductVariantSampleDownload" CssClass="adminButton" CausesValidation="false"
                runat="server" Text="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadFile.Remove %>"
                OnClick="btnRemoveProductVariantSampleDownload_Click" Visible="false" />
            <br />
            <asp:FileUpload ID="fuProductVariantSampleDownload" CssClass="adminInput" runat="server" />
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIsRecurring" Text="<% $NopResources:Admin.ProductVariantInfo.IsRecurring %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.IsRecurring.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsRecurring" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlCycleLength">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCycleLength" Text="<% $NopResources: Admin.ProductVariantInfo.CycleLength %>"
                ToolTip="<% $NopResources: Admin.ProductVariantInfo.CycleLength.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtCycleLength"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.CycleLength.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="100" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.CycleLength.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlCyclePeriod">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCyclePeriod" Text="<% $NopResources:Admin.ProductVariantInfo.CyclePeriod %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.CyclePeriod.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCyclePeriod" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlTotalCycles">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTotalCycles" Text="<% $NopResources: Admin.ProductVariantInfo.TotalCycles %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.TotalCycles.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtTotalCycles"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.TotalCycles.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.TotalCycles.RangeErrorMessage %>">
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblShipEnabled" Text="<% $NopResources:Admin.ProductVariantInfo.ShipEnabled %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.ShipEnabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsShipEnabled" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlFreeShipping">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFreeShipping" Text="<% $NopResources:Admin.ProductVariantInfo.FreeShipping %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.FreeShipping.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsFreeShipping" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlAdditionalShippingCharge">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAdditionalShippingCharge" Text="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtAdditionalShippingCharge"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr id="pnlWeight">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWeight" Text="<% $NopResources:Admin.ProductVariantInfo.Weight %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Weight.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=MeasureManager.BaseWeightIn.Name%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtWeight" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Weight.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Weight.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr id="pnlLength">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLength" Text="<% $NopResources:Admin.ProductVariantInfo.Length %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Length.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=MeasureManager.BaseDimensionIn.Name%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtLength" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Length.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Length.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr id="pnlWidth">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWidth" Text="<% $NopResources:Admin.ProductVariantInfo.Width %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Width.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=MeasureManager.BaseDimensionIn.Name%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtWidth" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Width.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Width.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr id="pnlHeight">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblHeight" Text="<% $NopResources: Admin.ProductVariantInfo.Height%>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Height.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=MeasureManager.BaseDimensionIn.Name%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtHeight" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Height.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Height.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxExempt" Text="<% $NopResources:Admin.ProductVariantInfo.TaxExempt %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.TaxExempt.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsTaxExempt" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxCategory" Text="<% $NopResources:Admin.ProductVariantInfo.TaxCategory %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.TaxCategory.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlTaxCategory" AutoPostBack="False" CssClass="adminInput"
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblManageStock" Text="<% $NopResources:Admin.ProductVariantInfo.ManageStock %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.ManageStock.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblStockQuantity" Text="<% $NopResources:Admin.ProductVariantInfo.StockQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.StockQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtStockQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.StockQuantity.RequiredErrorMessage %>"
                MinimumValue="-999999" MaximumValue="999999" Value="10000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.StockQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlDisplayStockAvailability">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayStockAvailability" Text="<% $NopResources:Admin.ProductVariantInfo.DisplayStockAvailability %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DisplayStockAvailability.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisplayStockAvailability" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlDisplayStockQuantity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayStockQuantity" Text="<% $NopResources:Admin.ProductVariantInfo.DisplayStockQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DisplayStockQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisplayStockQuantity" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMinStockQuantity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMinStockQuantity" Text="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMinStockQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="0" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlLowStockActivity">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLowStockActivity" Text="<% $NopResources:Admin.ProductVariantInfo.LowStockActivity %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.LowStockActivity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLowStockActivity" AutoPostBack="False" CssClass="adminInput"
                runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlNotifyForQuantityBelow">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNotifyForQuantityBelow" Text="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNotifyForQuantityBelow"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="1" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr id="pnlBackorders">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBackorders" Text="<% $NopResources:Admin.ProductVariantInfo.Backorders %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Backorders.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlBackorders" CssClass="adminInput" runat="server">
                <asp:ListItem Text="<% $NopResources:Admin.ProductBackorderMode.NoBackorders %>" Value="0"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.ProductBackorderMode.AllowQtyBelow0 %>" Value="1"></asp:ListItem>
                <asp:ListItem Text="<% $NopResources:Admin.ProductBackorderMode.AllowQtyBelow0AndNotifyCustomer %>"
                    Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderMinimumQuantity" Text="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtOrderMinimumQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="1" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderMaximumQuantity" Text="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtOrderMaximumQuantity"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="999999" Value="10000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWarehouse" Text="<% $NopResources:Admin.ProductVariantInfo.Warehouse %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Warehouse.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlWarehouse" AutoPostBack="False" CssClass="adminInput" runat="server">
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
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductPublished" Text="<% $NopResources:Admin.ProductVariantInfo.Published %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Published.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtDisplayOrder"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder.RequiredErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
