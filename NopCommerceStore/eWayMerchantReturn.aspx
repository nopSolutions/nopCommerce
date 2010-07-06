<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.eWayMerchantReturnPage" Codebehind="eWayMerchantReturn.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
  <div id="divAuthorised" style="display:none" runat="server">
        <table>
        <tr><td colspan="2"><img src="_images/tick.gif" alt="Tick"/>Thank you for your payment! The details of the transaction are as follows:</td></tr>
        </table>
    </div>
    <div id="divFailed" style="display:none" runat="server">
        <img src="_images/Cross.gif" alt="Cross"/>Your Transaction Failed to be processed <asp:HyperLink ID="HyperLink1"
            runat="server" NavigateUrl="~/default.aspx">please try again </asp:HyperLink><br />
    </div>
    
        <div id="divFraud" style="display:none" runat="server">
        <img src="_images/Cross.gif" alt="Cross" />Unable to determine the result of the transaction   
    </div>
        <table>
        <tr>
            <th>Status:</th>
            <td><asp:Label ID="lblStatus" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>AuthCode:</th>
            <td><asp:Label ID="lblAuthCode" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>ResponseCode:</th>
            <td><asp:Label ID="lblResponseCode" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>ReturnAmount:</th>
            <td><asp:Label ID="lblReturnAmount" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>TrxnNumber:</th>
            <td><asp:Label ID="lblTrxnNumber" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>MerchnatOption1:</th>
            <td><asp:Label ID="lblMerchantOption1" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>MerchnatOption2:</th>
            <td><asp:Label ID="lblMerchantOption2" runat="server" Text=""/></td>
        </tr>
        <tr>
            <th>MerchnatOption3:</th>
            <td><asp:Label ID="lblMerchantOption3" runat="server" Text=""/></td>
        </tr>
        
        <tr>
            <th>TrxnReference:</th>
            <td><asp:Label ID="lblMerchantReference" runat="server" Text=""/></td>
        </tr>
        
        <tr>
            <th>TrxnInvoice:</th>
            <td><asp:Label ID="lblMerchantInvoice" runat="server" Text=""/></td>
        </tr>
        
        <tr>
            <th>TrxnResponseMessage:</th>
            <td><asp:Label ID="lblTrxnResponseMessage" runat="server" Text=""/></td>
        </tr>
     <tr>
            <th>ErrorMessage:</th>
            <td><asp:Label ID="lblErrorMessage" runat="server" Text=""/></td>
        </tr>
        </table>
    
    </form>
</asp:Content>
