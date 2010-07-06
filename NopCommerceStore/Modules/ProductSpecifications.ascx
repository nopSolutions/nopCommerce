<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductSpecifications" CodeBehind="ProductSpecifications.ascx.cs" %>
<div class="productspec-box">
    <table width="100%">
        <tbody>
            <asp:Repeater ID="rptrProductSpecification" runat="server">
                <ItemTemplate>
                    <tr>
                        <td width="30%" style="padding: 2px">
                            <b><%#Server.HtmlEncode(((SpecificationAttribute)Eval("SpecificationAttribute")).LocalizedName)%></b>
                        </td>
                        <td width="70%" style="padding: 2px">
                            <%#Server.HtmlEncode(((SpecificationAttributeOption)Eval("SpecificationAttributeOption")).LocalizedName)%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</div>
