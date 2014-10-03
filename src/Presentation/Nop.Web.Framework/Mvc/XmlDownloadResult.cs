using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Nop.Web.Framework.Mvc
{
    public class XmlDownloadResult : ActionResult
    {
        public XmlDownloadResult(string xml, string fileDownloadName)
        {
            Xml = xml;
            FileDownloadName = fileDownloadName;
        }

        public string FileDownloadName
        {
            get;
            set;
        }

        public string Xml
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var document = new XmlDocument();
            document.LoadXml(Xml);
            var decl = document.FirstChild as XmlDeclaration;
            if (decl != null)
            {
                decl.Encoding = "utf-8";
            }
            context.HttpContext.Response.Charset = "utf-8";
            context.HttpContext.Response.ContentType = "text/xml";
            context.HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", FileDownloadName));
            context.HttpContext.Response.BinaryWrite(Encoding.UTF8.GetBytes(document.InnerXml));
            context.HttpContext.Response.End();
        }
    }
}
