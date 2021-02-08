<%@ Application Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.IO"  %>
<%@ Import Namespace="PdfSharp"  %>
<%@ Import Namespace="PdfFontLib"  %>
<script runat="server">
void Application_OnStart()
{
   PdfSharp.Fonts.GlobalFontSettings.FontResolver = new Resolver();
}

</script>