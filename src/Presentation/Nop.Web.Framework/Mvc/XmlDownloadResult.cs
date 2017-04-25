using System;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Mvc
{
    /// <summary>
    /// Represents a XML download result implementation of the action result
    /// </summary>
    public class XmlDownloadResult : ActionResult
    {
        private const string DefaultContentType = "text/xml; charset=utf-8";

        #region Properties

        /// <summary>
        /// Gets or sets the name of download file
        /// </summary>
        public string FileDownloadName { get; set; }

        /// <summary>
        /// Gets or sets XML of the file
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type header for the response
        /// </summary>
        public string ContentType { get; set; }

        #endregion

        #region Ctor

        public XmlDownloadResult(string xml, string fileDownloadName)
        {
            Xml = xml;
            FileDownloadName = fileDownloadName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the result operation of the action method synchronously. This method is called by MVC to process the result of an action method.
        /// </summary>
        /// <param name="context">Action context</param>
        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var response = context.HttpContext.Response;

            //create XML document from passed XML
            var document = new XmlDocument();
            document.LoadXml(this.Xml);
            if (document.FirstChild is XmlDeclaration declaration)
                declaration.Encoding = "utf-8";

            //get content type and encoding
            ResponseContentTypeHelper.ResolveContentTypeAndEncoding(ContentType, response.ContentType, DefaultContentType,
                out string resolvedContentType, out Encoding resolvedContentTypeEncoding);

            //set response details
            response.ContentType = resolvedContentType;
            response.Headers.Add("content-disposition", string.Format("attachment; filename={0}", FileDownloadName));
            response.ContentLength = resolvedContentTypeEncoding.GetByteCount(document.InnerXml);

            //try get IHttpResponseStreamWriterFactory
            var httpResponseStreamWriterFactory = EngineContext.Current.Resolve<IHttpResponseStreamWriterFactory>();

            //write XML to response
            using (var textWriter = httpResponseStreamWriterFactory.CreateWriter(response.Body, resolvedContentTypeEncoding))
            {
                document.Save(textWriter);
                textWriter.Flush();
            }
        }

        #endregion
    }
}