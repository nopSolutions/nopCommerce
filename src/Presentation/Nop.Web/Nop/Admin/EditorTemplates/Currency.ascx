<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<decimal?>" %>

<%= Html.Telerik().CurrencyTextBox()
        .Name(ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty))
        .InputHtmlAttributes(new {style="width:100%"})
        .MinValue(0)
        .Value(Model)
%>
