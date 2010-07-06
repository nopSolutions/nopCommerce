<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.DiscountsControl"
    CodeBehind="Discounts.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.Discounts.Title")%>" />
        <%=GetLocaleResourceString("Admin.Discounts.Title")%>
    </div>
    <div class="options">
        <input type="button" onclick="location.href='DiscountAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Discounts.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Discounts.AddNewButton.Tooltip")%>" />
    </div>
</div>
<asp:GridView ID="gvDiscounts" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Discounts.Name %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="UsePercentage" HeaderText="<% $NopResources:Admin.Discounts.UsePercentage %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="DiscountPercentage" HeaderText="<% $NopResources:Admin.Discounts.DiscountPercentage %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="DiscountAmount" HeaderText="<% $NopResources:Admin.Discounts.DiscountAmount %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="StartDate" HeaderText="<% $NopResources:Admin.Discounts.StartDate %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="EndDate" HeaderText="<% $NopResources:Admin.Discounts.EndDate %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Discounts.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="DiscountDetails.aspx?DiscountID=<%#Eval("DiscountId")%>" title="<%#GetLocaleResourceString("Admin.Discounts.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Discounts.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
