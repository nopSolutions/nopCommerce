<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.EstimateShippingControl"
    CodeBehind="EstimateShipping.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<div class="estimate-shipping">
    <b>
        <%=GetLocaleResourceString("EstimateShipping.Title")%></b>
    <br />
    <%=GetLocaleResourceString("EstimateShipping.Tooltip")%>
    <br />
    <div class="shipping-options">
        <div>
            <table>
                <tr>
                    <td>
                        <%=GetLocaleResourceString("EstimateShipping.Country")%>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCountry" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"
                            Width="137px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%=GetLocaleResourceString("EstimateShipping.StateProvince")%>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStateProvince" AutoPostBack="False" runat="server" Width="137px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%=GetLocaleResourceString("EstimateShipping.ZipPostalCode")%>
                    </td>
                    <td>
                        <nopCommerce:SimpleTextBox runat="server" ID="txtZipPostalCode" ValidationGroup="EstimateShipping"
                            ErrorMessage="<% $NopResources:EstimateShipping.ZipPostalCodeIsRequired %>">
                        </nopCommerce:SimpleTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnGetQuote" Text="<% $NopResources:EstimateShipping.GetQuoteButton %>"
                            OnClick="btnGetQuote_Click" CssClass="estimateshippingbutton" ValidationGroup="EstimateShipping" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="clear">
        </div>
        <asp:Panel runat="server" ID="phShippingMethods">
            <asp:DataList runat="server" ID="dlShippingOptions" EnableViewState="false">
                <ItemTemplate>
                    <div class="shipping-option-item">
                        <div class="option-name">
                            <%#Server.HtmlEncode(Eval("Name").ToString()) %>
                            <%#Server.HtmlEncode(FormatShippingOption(((ShippingOption)Container.DataItem)))%>
                        </div>
                        <div class="option-description">
                            <%#Eval("Description") %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </asp:Panel>
        <div class="clear">
        </div>
        <asp:Panel runat="server" ID="pnlWarnings" CssClass="warning-box" EnableViewState="false"
            Visible="false">
            <br />
            <asp:Label runat="server" ID="lblWarning" CssClass="warning-text" EnableViewState="false"
                Visible="false"></asp:Label>
        </asp:Panel>
    </div>
</div>
