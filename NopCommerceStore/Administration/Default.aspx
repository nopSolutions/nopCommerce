<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Default"
    CodeBehind="Default.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="nopCommerceNews" Src="Modules/nopCommerceNews.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="Warnings" Src="Modules/Warnings.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SearchTermStat" Src="Modules/SearchTermStat.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="BestSellersStat" Src="Modules/BestSellersStat.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="OrderAverageReport" Src="Modules/OrderAverageReport.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="OrderStatistics" Src="Modules/OrderStatistics.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerStatistics" Src="Modules/CustomerStatistics.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-title">
        <img src="Common/ico-dashboard.png" alt="<%=GetLocaleResourceString("Admin.Dashboard")%>" />
        <%=GetLocaleResourceString("Admin.Dashboard")%>
    </div>
    <table class="dashboard">
        <tr>
            <td class="maincol">
                <div class="section-header">
                    <div class="title">
                        <img src="Common/ico-stat1.gif" alt="<%=GetLocaleResourceString("Admin.StoreStatistics")%>" />
                         <%=GetLocaleResourceString("Admin.StoreStatistics")%>
                    </div>
                </div>
                
                <table class="stats">
                    <tbody>
                        <tr>
                            <td class="orderaveragereport">
                                <nopCommerce:OrderAverageReport runat="server" ID="ctrlOrderAverageReport" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table class="stats">
                    <tbody>
                        <tr>
                            <td class="orderstatistics">
                                <nopCommerce:OrderStatistics runat="server" ID="ctrlOrderStatistics" />
                            </td> 
                            <td class="customerstatistics">
                                <nopCommerce:CustomerStatistics runat="server" ID="ctrlCustomerStatistics" DisplayTitle="true" />
                            </td>                                    
                        </tr>
                    </tbody>
                </table>
                <table class="stats">
                    <tr>
                        <td class="bestsellers">
                            <nopCommerce:BestSellersStat runat="server" ID="ctrlBestSellersStat" />
                        </td>
                        <td class="searchterms">
                            <nopCommerce:SearchTermStat runat="server" ID="ctrlSearchTermStat" />
                        </td>
                    </tr>
                </table>
            </td>
            <td class="rightcol">
                <nopCommerce:nopCommerceNews runat="server" ID="ctrlNews" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="basecol">
                <nopCommerce:Warnings runat="server" ID="ctrlWarnings" />
            </td>
        </tr>
    </table>
</asp:Content>
