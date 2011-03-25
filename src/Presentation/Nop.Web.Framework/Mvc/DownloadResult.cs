using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Nop.Web.Framework.Mvc
{
    public class XmlDownloadResult : ActionResult
    {
        #region Constructors (2)

        public XmlDownloadResult(string xml, string fileDownloadName)
        {
            Xml = xml;
            FileDownloadName = fileDownloadName;
        }

        #endregion Constructors

        #region Properties (2)

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

        #endregion Properties

        #region Methods (1)

        // Public Methods (1) 

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.AddHeader("content-disposition",
              "attachment; filename=" + this.FileDownloadName);

            XmlDocument document = new XmlDocument();
            document.LoadXml(Xml);
            XmlDeclaration decl = document.FirstChild as XmlDeclaration;
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

        #endregion Methods
    }
}
