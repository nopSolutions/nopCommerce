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
<table class="adminContent">
    <tr runat="server" id="pnlProductVariantId">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblCreatedOnTitle" text="<% $NopResources:Admin.ProductVariantInfo.ID %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.ID.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblProductVariantId" runat="server"></asp:Label>
        </td>
    </tr>
</table>
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
                    <nopcommerce:tooltiplabel runat="server" id="lblProductVariantName" text="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName %>"
                        tooltip="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName.Tooltip %>"
                        tooltipimage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtName" runat="server" CssClass="adminInput"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopcommerce:tooltiplabel runat="server" id="lblDescription" text="<% $NopResources:Admin.ProductVariantInfo.Description %>"
                        tooltip="<% $NopResources:Admin.ProductVariantInfo.Description.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopcommerce:nophtmleditor id="txtDescription" runat="server" height="350" />
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
                            <nopcommerce:tooltiplabel runat="server" id="lblLocalizedProductVariantName" text="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName %>"
                                tooltip="<% $NopResources:Admin.ProductVariantInfo.ProductVariantName.Tooltip %>"
                                tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtLocalizedName" runat="server" CssClass="adminInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopcommerce:tooltiplabel runat="server" id="lblLocalizedDescription" text="<% $NopResources:Admin.ProductVariantInfo.Description %>"
                                tooltip="<% $NopResources:Admin.ProductVariantInfo.Description.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopcommerce:nophtmleditor id="txtLocalizedDescription" runat="server" height="350" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblSKU" text="<% $NopResources:Admin.ProductVariantInfo.SKU %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.SKU.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSKU" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblImage" text="<% $NopResources:Admin.ProductVariantInfo.Image %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.Image.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblAdminComment" text="<% $NopResources:Admin.ProductVariantInfo.AdminComment %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.AdminComment.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtAdminComment" runat="server" CssClass="adminInput" TextMode="MultiLine"
                Height="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblManufacturerPartNumber" text="<% $NopResources:Admin.ProductVariantInfo.ManufacturerPartNumber %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.ManufacturerPartNumber.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblPrice" text="<% $NopResources:Admin.ProductVariantInfo.Price %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.Price.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:decimaltextbox runat="server" cssclass="adminInput" id="txtPrice" value="0"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.Price.RequiredErrorMessage %>"
                minimumvalue="0" maximumvalue="100000000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.Price.RangeErrorMessage %>">
            </nopcommerce:decimaltextbox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblOldPrice" text="<% $NopResources:Admin.ProductVariantInfo.OldPrice %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.OldPrice.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:decimaltextbox runat="server" cssclass="adminInput" id="txtOldPrice"
                value="0" requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.OldPrice.RequiredErrorMessage %>"
                minimumvalue="0" maximumvalue="100000000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.OldPrice.RangeErrorMessage %>">
            </nopcommerce:decimaltextbox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblProductCost" text="<% $NopResources:Admin.ProductVariantInfo.ProductCost %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.ProductCost.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:decimaltextbox runat="server" cssclass="adminInput" id="txtProductCost"
                value="0" requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.ProductCost.RequiredErrorMessage %>"
                minimumvalue="0" maximumvalue="100000000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.ProductCost.RangeErrorMessage %>">
            </nopcommerce:decimaltextbox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDisableBuyButton" text="<% $NopResources:Admin.ProductVariantInfo.DisableBuyButton %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DisableBuyButton.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisableBuyButton" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblCallForPrice" text="<% $NopResources:Admin.ProductVariantInfo.CallForPrice %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.CallForPrice.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCallForPrice" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblCustomerEntersPrice" text="<% $NopResources:Admin.ProductVariantInfo.CustomerEntersPrice %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.CustomerEntersPrice.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCustomerEntersPrice" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMinimumCustomerEnteredPrice">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblMinimumCustomerEnteredPrice" text="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtMinimumCustomerEnteredPrice"
                value="0" requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.RequiredErrorMessage%>"
                minimumvalue="0" maximumvalue="100000000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.MinimumCustomerEnteredPrice.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr id="pnlMaximumCustomerEnteredPrice">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblMaximumCustomerEnteredPrice" text="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtMaximumCustomerEnteredPrice"
                value="1000" requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.RequiredErrorMessage%>"
                minimumvalue="0" maximumvalue="100000000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.MaximumCustomerEnteredPrice.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblAvailableStartDateTime" text="<% $NopResources:Admin.ProductVariantInfo.AvailableStartDateTime %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.AvailableStartDateTime.ToolTip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:datepicker runat="server" id="ctrlAvailableStartDateTimePicker" targetcontrolid="txtAvailableStartDateTime" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblAvailableEndDateTime" text="<% $NopResources:Admin.ProductVariantInfo.AvailableEndDateTime %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.AvailableEndDateTime.ToolTip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:datepicker runat="server" id="ctrlAvailableEndDateTimePicker" targetcontrolid="txtAvailableEndDateTime" />
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblIsGiftCard" text="<% $NopResources:Admin.ProductVariantInfo.IsGiftCard %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.IsGiftCard.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsGiftCard" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlGiftCardType">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblGiftCardType" text="<% $NopResources:Admin.ProductVariantInfo.GiftCardType %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.GiftCardType.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblIsDownload" text="<% $NopResources:Admin.ProductVariantInfo.IsDownload %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.IsDownload.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsDownload" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlUseDownloadURL">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblUseDownloadURL" text="<% $NopResources:Admin.ProductVariantInfo.UseDownloadURL %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.UseDownloadURL.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseDownloadURL" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlDownloadURL">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDownloadURL" text="<% $NopResources:Admin.ProductVariantInfo.DownloadURL %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DownloadURL.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDownloadURL" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlDownloadFile">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDownloadFile" text="<% $NopResources:Admin.ProductVariantInfo.DownloadFile %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DownloadFile.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblUnlimitedDownloads" text="<% $NopResources:Admin.ProductVariantInfo.UnlimitedDownloads %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.UnlimitedDownloads.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUnlimitedDownloads" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMaxNumberOfDownloads">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblMaxNumberOfDownloads" text="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtMaxNumberOfDownloads"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads.RequiredErrorMessage %>"
                minimumvalue="0" maximumvalue="999999" value="10" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.MaxNumberOfDownloads.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr id="pnlDownloadExpirationDays">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDownloadExpirationDays" text="<% $NopResources: Admin.ProductVariantInfo.DownloadExpirationDays %>"
                tooltip="<% $NopResources: Admin.ProductVariantInfo.DownloadExpirationDays.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDownloadExpirationDays" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlDownloadActivationType">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDownloadActivationType" text="<% $NopResources:Admin.ProductVariantInfo.DownloadActivationType %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DownloadActivationType.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlDownloadActivationType" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlHasUserAgreement">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel id="lblHasUserAgreement" runat="server" text="<% $NopResources:Admin.ProductVariantInfo.CbHasUserAgreement.Info %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.CbHasUserAgreement.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
            :
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbHasUserAgreement" Checked="false" />
        </td>
    </tr>
    <tr id="pnlUserAgreementText">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel id="lblUserAgreementText" runat="server" text="<% $NopResources:Admin.ProductVariantInfo.TxtUserAgreementText.Info %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.TxtUserAgreementText.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
            :
        </td>
        <td class="adminData">
            <nopcommerce:nophtmleditor runat="server" id="txtUserAgreementText" height="350" />
        </td>
    </tr>
    <tr id="pnlHasSampleDownload">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblHasSampleDownload" text="<% $NopResources:Admin.ProductVariantInfo.HasSampleDownload %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.HasSampleDownload.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbHasSampleDownload" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlUseSampleDownloadURL">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblUseSampleDownloadURL" text="<% $NopResources:Admin.ProductVariantInfo.UseSampleDownloadURL %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.UseSampleDownloadURL.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbUseSampleDownloadURL" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlSampleDownloadURL">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblSampleDownloadURL" text="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadURL %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadURL.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtSampleDownloadURL" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr id="pnlSampleDownloadFile">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblSampleDownloadFile" text="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadFile %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.SampleDownloadFile.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblIsRecurring" text="<% $NopResources:Admin.ProductVariantInfo.IsRecurring %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.IsRecurring.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsRecurring" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlCycleLength">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblCycleLength" text="<% $NopResources: Admin.ProductVariantInfo.CycleLength %>"
                tooltip="<% $NopResources: Admin.ProductVariantInfo.CycleLength.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtCycleLength"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.CycleLength.RequiredErrorMessage %>"
                minimumvalue="1" maximumvalue="999999" value="100" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.CycleLength.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr id="pnlCyclePeriod">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblCyclePeriod" text="<% $NopResources:Admin.ProductVariantInfo.CyclePeriod %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.CyclePeriod.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCyclePeriod" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlTotalCycles">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblTotalCycles" text="<% $NopResources: Admin.ProductVariantInfo.TotalCycles %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.TotalCycles.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtTotalCycles"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.TotalCycles.RequiredErrorMessage %>"
                minimumvalue="1" maximumvalue="999999" value="10" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.TotalCycles.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblShipEnabled" text="<% $NopResources:Admin.ProductVariantInfo.ShipEnabled %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.ShipEnabled.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsShipEnabled" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlFreeShipping">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblFreeShipping" text="<% $NopResources:Admin.ProductVariantInfo.FreeShipping %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.FreeShipping.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsFreeShipping" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlAdditionalShippingCharge">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblAdditionalShippingCharge" text="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtAdditionalShippingCharge"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.AdditionalShippingCharge.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr id="pnlWeight">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWeight" Text="<% $NopResources:Admin.ProductVariantInfo.Weight %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Weight.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtWeight" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Weight.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Weight.RangeErrorMessage %>">
            </nopcommerce:decimaltextbox> [<%=MeasureManager.BaseWeightIn.Name%>]
        </td>
    </tr>
    <tr id="pnlLength">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLength" Text="<% $NopResources:Admin.ProductVariantInfo.Length %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Length.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtLength" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Length.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Length.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=MeasureManager.BaseDimensionIn.Name%>]
        </td>
    </tr>
    <tr id="pnlWidth">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblWidth" Text="<% $NopResources:Admin.ProductVariantInfo.Width %>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Width.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtWidth" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Width.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Width.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
            [<%=MeasureManager.BaseDimensionIn.Name%>]
        </td>
    </tr>
    <tr id="pnlHeight">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblHeight" Text="<% $NopResources: Admin.ProductVariantInfo.Height%>"
                ToolTip="<% $NopResources:Admin.ProductVariantInfo.Height.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtHeight" Value="0"
                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Height.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.ProductVariantInfo.Height.RangeErrorMessage %>">
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
            <nopcommerce:tooltiplabel runat="server" id="lblTaxExempt" text="<% $NopResources:Admin.ProductVariantInfo.TaxExempt %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.TaxExempt.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbIsTaxExempt" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblTaxCategory" text="<% $NopResources:Admin.ProductVariantInfo.TaxCategory %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.TaxCategory.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblManageStock" text="<% $NopResources:Admin.ProductVariantInfo.ManageStock %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.ManageStock.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblStockQuantity" text="<% $NopResources:Admin.ProductVariantInfo.StockQuantity %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.StockQuantity.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtStockQuantity"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.StockQuantity.RequiredErrorMessage %>"
                minimumvalue="-999999" maximumvalue="999999" value="10000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.StockQuantity.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr id="pnlDisplayStockAvailability">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDisplayStockAvailability" text="<% $NopResources:Admin.ProductVariantInfo.DisplayStockAvailability %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DisplayStockAvailability.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisplayStockAvailability" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlDisplayStockQuantity">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDisplayStockQuantity" text="<% $NopResources:Admin.ProductVariantInfo.DisplayStockQuantity %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DisplayStockQuantity.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbDisplayStockQuantity" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr id="pnlMinStockQuantity">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblMinStockQuantity" text="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtMinStockQuantity"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity.RequiredErrorMessage %>"
                minimumvalue="0" maximumvalue="999999" value="0" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.MinStockQuantity.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr id="pnlLowStockActivity">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblLowStockActivity" text="<% $NopResources:Admin.ProductVariantInfo.LowStockActivity %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.LowStockActivity.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLowStockActivity" AutoPostBack="False" CssClass="adminInput"
                runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="pnlNotifyForQuantityBelow">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblNotifyForQuantityBelow" text="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtNotifyForQuantityBelow"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow.RequiredErrorMessage %>"
                minimumvalue="1" maximumvalue="999999" value="1" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.NotifyForQuantityBelow.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr id="pnlBackorders">
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblBackorders" text="<% $NopResources:Admin.ProductVariantInfo.Backorders %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.Backorders.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblOrderMinimumQuantity" text="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtOrderMinimumQuantity"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity.RequiredErrorMessage %>"
                minimumvalue="1" maximumvalue="999999" value="1" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.OrderMinimumQuantity.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblOrderMaximumQuantity" text="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity.Tooltip %>"
                tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtOrderMaximumQuantity"
                requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity.RequiredErrorMessage %>"
                minimumvalue="1" maximumvalue="999999" value="10000" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.OrderMaximumQuantity.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblWarehouse" text="<% $NopResources:Admin.ProductVariantInfo.Warehouse %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.Warehouse.Tooltip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
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
            <nopcommerce:tooltiplabel runat="server" id="lblProductPublished" text="<% $NopResources:Admin.ProductVariantInfo.Published %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.Published.ToolTip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopcommerce:tooltiplabel runat="server" id="lblDisplayOrder" text="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder %>"
                tooltip="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder.ToolTip %>" tooltipimage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopcommerce:numerictextbox runat="server" cssclass="adminInput" id="txtDisplayOrder"
                value="1" requirederrormessage="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder.RequiredErrorMessage %>"
                minimumvalue="-99999" maximumvalue="99999" rangeerrormessage="<% $NopResources:Admin.ProductVariantInfo.DisplayOrder.RangeErrorMessage %>">
            </nopcommerce:numerictextbox>
        </td>
    </tr>
</table>
