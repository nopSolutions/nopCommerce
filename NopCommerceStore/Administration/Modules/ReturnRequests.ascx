<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ReturnRequestsControl"
    CodeBehind="ReturnRequests.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.ReturnRequests.Title")%>" />
        <%=GetLocaleResourceString("Admin.ReturnRequests.Title")%>
    </div>
    <div class="options">
    </div>
</div>
<table class="adminContent">
    <asp:GridView ID="gvReturnRequests" runat="server" AutoGenerateColumns="False" Width="100%"
        OnPageIndexChanging="gvReturnRequests_PageIndexChanging" AllowPaging="true" PageSize="15"
        OnRowDataBound="gvReturnRequests_OnRowDataBound">
        <Columns>
            <asp:BoundField DataField="ReturnRequestId" HeaderText="<% $NopResources:Admin.ReturnRequests.IDColumn %>"
                ItemStyle-Width="5%"></asp:BoundField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ReturnRequests.NameColumn %>"
                ItemStyle-Width="30%">
                <ItemTemplate>
                    <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                        <em>
                            <%#Eval("Quantity")%>
                            x <a href='<%#GetProductVariantUrl((OrderProductVariant)((ReturnRequest)Container.DataItem).OrderProductVariant)%>'>
                                <%#Server.HtmlEncode(GetProductVariantName((OrderProductVariant)((ReturnRequest)Container.DataItem).OrderProductVariant))%></a></em>
                        <%#GetAttributeDescription((OrderProductVariant)((ReturnRequest)Container.DataItem).OrderProductVariant)%>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ReturnRequests.CustomerColumn %>"
                ItemStyle-Width="15%">
                <ItemTemplate>
                    <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ReturnRequests.OrderColumn %>"
                ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%#GetOrderInfo(((OrderProductVariant)((ReturnRequest)Container.DataItem).OrderProductVariant).OrderId)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ReturnRequests.DateColumn %>"
                HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ReturnRequests.StatusColumn %>"
                ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%#OrderManager.GetReturnRequestStatusName((ReturnStatusEnum)(Eval("ReturnStatus")))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<% $NopResources:Admin.ReturnRequests.Edit %>" HeaderStyle-HorizontalAlign="Center"
                ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <a href="ReturnRequestDetails.aspx?ReturnRequestID=<%#Eval("ReturnRequestId")%>">
                        <%#GetLocaleResourceString("Admin.ReturnRequests.Edit")%>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    <EmptyDataTemplate>
        <%#GetLocaleResourceString("Admin.ReturnRequests.NoRecordsFound")%>
    </EmptyDataTemplate>
        <PagerSettings PageButtonCount="50" Position="TopAndBottom" />
    </asp:GridView>
</table>
