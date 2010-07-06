<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ManufacturerNavigation"
    CodeBehind="ManufacturerNavigation.ascx.cs" %>
<div class="block block-manufacturer-navigation">
    <div class="title">
        <%=GetLocaleResourceString("Manufacturer.Manufacturers")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <asp:Repeater ID="rptrManufacturers" runat="server" OnItemDataBound="rptrManufacturers_ItemDataBound">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <asp:HyperLink ID="hlManufacturer" runat="server" Text='<%#Server.HtmlEncode(Eval("LocalizedName").ToString()) %>'
                        CssClass='<%# ((int)Eval("ManufacturerId") == this.ManufacturerId) ? "active" : "inactive" %>' />
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
        <div class="viewall">
            <a href="<%=CommonHelper.GetStoreLocation()%>manufacturers.aspx">
                <%=GetLocaleResourceString("Manufacturer.ViewAll")%></a>
        </div>
    </div>
</div>
