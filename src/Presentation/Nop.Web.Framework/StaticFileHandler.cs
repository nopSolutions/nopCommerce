//Source: http://omaralzabir.com/deploy_asp_net_mvc_on_iis_6__solve_404__compression_and_performance_problems/
//Contributor: Omar Al Zabir

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Threading;
using System.Web.Caching;
using System.Diagnostics;
using System.Net;

namespace Nop.Web.Framework
{
    public class StaticFileHandler : IHttpAsyncHandler
    {
        private readonly static TimeSpan DEFAULT_CACHE_DURATION = TimeSpan.FromDays(30);
        private readonly static string[] FILE_TYPES =
            new string[] { ".css", ".js", ".html", ".htm", ".png", ".jpeg", ".jpg", ".gif", ".bmp" };

        private readonly static string[] COMPRESS_FILE_TYPES =
            new string[] { ".css", ".js", ".html", ".htm" };

        static StaticFileHandler()
        {
            Array.Sort(FILE_TYPES);
            Array.Sort(COMPRESS_FILE_TYPES);
        }

        private enum ResponseCompressionType
        {
            None,
            GZip,
            Deflate
        }

        private class CachedContent
        {
            public byte[] ResponseBytes;
            public DateTime LastModified;

            public CachedContent(byte[] bytes, DateTime lastModified)
            {
                this.ResponseBytes = bytes;
                this.LastModified = lastModified;
            }
        }

        private HttpContext _context;

        #region IHttpAsyncHandler Members

        public virtual IAsyncResult BeginProcessRequest(HttpContext context,
            AsyncCallback callback, object state)
        {
            this._context = context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            try
            {
                EnsureProperRequest(request);

                string physicalFilePath = request.PhysicalPath;

                ResponseCompressionType compressionType = this.GetCompressionMode(request);

                FileInfo file = new FileInfo(physicalFilePath);
                string fileExtension = file.Extension.ToLower();

                // Do we handle such file types?
                if (Array.BinarySearch<string>(FILE_TYPES, fileExtension) >= 0)
                {
                    // Yes we do. 

                    // If this is a binary file like image, then we won't compress it.
                    if (Array.BinarySearch<string>(COMPRESS_FILE_TYPES, fileExtension) < 0)
                        compressionType = ResponseCompressionType.None;

                    // If the response bytes are already cached, then deliver the bytes directly from cache
                    string cacheKey = typeof(StaticFileHandler).ToString() + ":"
                        + compressionType.ToString()
                        + ":" + physicalFilePath;

                    if (DeliverFromCache(context, request, response,
                        cacheKey, physicalFilePath, compressionType))
                    {
                        // Delivered from cache
                    }
                    else
                    {
                        if (file.Exists)
                        {
                            // When not compressed, buffer is the size of the file but when compressed, 
                            // initial buffer size is one third of the file size. Assuming, compression 
                            // will give us less than 1/3rd of the size
                            using (MemoryStream memoryStream = new MemoryStream(
                                compressionType == ResponseCompressionType.None ?
                                    Convert.ToInt32(file.Length) :
                                    Convert.ToInt32((double)file.Length / 3)))
                            {
                                ReadFileData(compressionType, file, memoryStream);

                                this.CacheAndDeliver(context, request, response, physicalFilePath,
                                    compressionType, cacheKey, memoryStream, file);
                            }
                        }
                        else
                        {
                            throw new HttpException((int)HttpStatusCode.NotFound, request.FilePath + " Not Found");
                        }
                    }
                }
                else
                {
                    this.TransmitFileUsingHttpResponse(request, response, physicalFilePath, compressionType, file);
                }

                return new HttpAsyncResult(callback, state, true, null, null);

            }
            catch (Exception x)
            {
                if (x is HttpException)
                {
                    HttpException h = x as HttpException;
                    response.StatusCode = h.GetHttpCode();
                    Debug.WriteLine(h.Message);
                }
                return new HttpAsyncResult(callback, state, true, null, x);
            }

        }

        public virtual void EndProcessRequest(IAsyncResult result)
        {

        }

        #endregion

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private bool DeliverFromCache(HttpContext context,
            HttpRequest request, HttpResponse response,
            string cacheKey,
            string physicalFilePath, ResponseCompressionType compressionType)
        {
            CachedContent cachedContent = context.Cache[cacheKey] as CachedContent;
            if (null != cachedContent)
            {
                byte[] cachedBytes = cachedContent.ResponseBytes;

                // We have it cached
                this.ProduceResponseHeader(response, cachedBytes.Length, compressionType,
                    physicalFilePath, cachedContent.LastModified);
                this.WriteResponse(response, cachedBytes, compressionType, physicalFilePath);

                Debug.WriteLine("StaticFileHandler: Cached: " + request.FilePath);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CacheAndDeliver(HttpContext context,
            HttpRequest request, HttpResponse response,
            string physicalFilePath, ResponseCompressionType compressionType,
            string cacheKey, MemoryStream memoryStream, FileInfo file)
        {
            // Cache the content in ASP.NET Cache
            byte[] responseBytes = memoryStream.ToArray();
            context.Cache.Insert(cacheKey, new CachedContent(responseBytes, file.LastWriteTimeUtc),
                new CacheDependency(physicalFilePath),
                DateTime.Now.Add(DEFAULT_CACHE_DURATION), Cache.NoSlidingExpiration);

            this.ProduceResponseHeader(response, responseBytes.Length, compressionType,
                physicalFilePath, file.LastWriteTimeUtc);
            this.WriteResponse(response, responseBytes, compressionType, physicalFilePath);

            Debug.WriteLine("StaticFileHandler: NonCached: " + request.FilePath);
        }

        private static void ReadFileData(ResponseCompressionType compressionType,
            FileInfo file, MemoryStream memoryStream)
        {
            using (Stream outputStream =
                (compressionType == ResponseCompressionType.None ? memoryStream :
                (compressionType == ResponseCompressionType.GZip ?
                    (Stream)new GZipStream(memoryStream, CompressionMode.Compress, true) :
                    (Stream)new DeflateStream(memoryStream, CompressionMode.Compress))))
            {
                // We can compress and cache this file
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    int bufSize = Convert.ToInt32(Math.Min(file.Length, 8 * 1024));
                    byte[] buffer = new byte[bufSize];

                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, bufSize)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }

                outputStream.Flush();
            }
        }

        private static void EnsureProperRequest(HttpRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                throw new HttpException((int)HttpStatusCode.MethodNotAllowed, "Method not allowed");
            }
            if (request.FilePath.EndsWith(".asp", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "Path forbidden");
            }
        }

        private void TransmitFileUsingHttpResponse(HttpRequest request, HttpResponse response,
            string physicalFilePath, ResponseCompressionType compressionType, FileInfo file)
        {
            if (file.Exists)
            {
                // We don't cache/compress such file types. Must be some binary file that's better
                // to let IIS handle
                this.ProduceResponseHeader(response, Convert.ToInt32(file.Length), compressionType,
                    physicalFilePath, file.LastWriteTimeUtc);
                response.TransmitFile(physicalFilePath);

                Debug.WriteLine("TransmitFile: " + request.FilePath);
            }
            else
            {
                throw new HttpException((int)HttpStatusCode.NotFound, request.FilePath + " Not Found");
            }
        }

        private void WriteResponse(HttpResponse response, byte[] bytes,
            ResponseCompressionType mode, string physicalFilePath)
        {
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.OutputStream.Flush();
        }

        private void ProduceResponseHeader(HttpResponse response, int count,
            ResponseCompressionType mode, string physicalFilePath,
            DateTime lastModified)
        {
            response.Buffer = false;
            response.BufferOutput = false;

            // Emit content type and encoding based on the file extension and 
            // whether the response is compressed
            response.ContentType = MimeMapping.GetMimeMapping(physicalFilePath);
            if (mode != ResponseCompressionType.None)
                response.AppendHeader("Content-Encoding", mode.ToString().ToLower());
            response.AppendHeader("Content-Length", count.ToString());

            // Emit proper cache headers that will cache the response in browser's 
            // cache for the default cache duration
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            response.Cache.SetMaxAge(DEFAULT_CACHE_DURATION);
            response.Cache.SetExpires(DateTime.Now.Add(DEFAULT_CACHE_DURATION));
            response.Cache.SetLastModified(lastModified);
        }

        private ResponseCompressionType GetCompressionMode(HttpRequest request)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) return ResponseCompressionType.None;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            if (acceptEncoding.Contains("GZIP"))
                return ResponseCompressionType.GZip;
            else if (acceptEncoding.Contains("DEFLATE"))
                return ResponseCompressionType.Deflate;
            else
                return ResponseCompressionType.None;
        }

        #endregion
    }


    #region MimeMapping
    internal class MimeMapping
    {
        // Fields
        private static Hashtable _extensionToMimeMappingTable = new Hashtable(190, StringComparer.CurrentCultureIgnoreCase);

        // Methods
        static MimeMapping()
        {
            AddMimeMapping(".323", "text/h323");
            AddMimeMapping(".asx", "video/x-ms-asf");
            AddMimeMapping(".acx", "application/internet-property-stream");
            AddMimeMapping(".ai", "application/postscript");
            AddMimeMapping(".aif", "audio/x-aiff");
            AddMimeMapping(".aiff", "audio/aiff");
            AddMimeMapping(".axs", "application/olescript");
            AddMimeMapping(".aifc", "audio/aiff");
            AddMimeMapping(".asr", "video/x-ms-asf");
            AddMimeMapping(".avi", "video/x-msvideo");
            AddMimeMapping(".asf", "video/x-ms-asf");
            AddMimeMapping(".au", "audio/basic");
            AddMimeMapping(".application", "application/x-ms-application");
            AddMimeMapping(".bin", "application/octet-stream");
            AddMimeMapping(".bas", "text/plain");
            AddMimeMapping(".bcpio", "application/x-bcpio");
            AddMimeMapping(".bmp", "image/bmp");
            AddMimeMapping(".cdf", "application/x-cdf");
            AddMimeMapping(".cat", "application/vndms-pkiseccat");
            AddMimeMapping(".crt", "application/x-x509-ca-cert");
            AddMimeMapping(".c", "text/plain");
            AddMimeMapping(".css", "text/css");
            AddMimeMapping(".cer", "application/x-x509-ca-cert");
            AddMimeMapping(".crl", "application/pkix-crl");
            AddMimeMapping(".cmx", "image/x-cmx");
            AddMimeMapping(".csh", "application/x-csh");
            AddMimeMapping(".cod", "image/cis-cod");
            AddMimeMapping(".cpio", "application/x-cpio");
            AddMimeMapping(".clp", "application/x-msclip");
            AddMimeMapping(".crd", "application/x-mscardfile");
            AddMimeMapping(".deploy", "application/octet-stream");
            AddMimeMapping(".dll", "application/x-msdownload");
            AddMimeMapping(".dot", "application/msword");
            AddMimeMapping(".doc", "application/msword");
            AddMimeMapping(".dvi", "application/x-dvi");
            AddMimeMapping(".dir", "application/x-director");
            AddMimeMapping(".dxr", "application/x-director");
            AddMimeMapping(".der", "application/x-x509-ca-cert");
            AddMimeMapping(".dib", "image/bmp");
            AddMimeMapping(".dcr", "application/x-director");
            AddMimeMapping(".disco", "text/xml");
            AddMimeMapping(".exe", "application/octet-stream");
            AddMimeMapping(".etx", "text/x-setext");
            AddMimeMapping(".evy", "application/envoy");
            AddMimeMapping(".eml", "message/rfc822");
            AddMimeMapping(".eps", "application/postscript");
            AddMimeMapping(".flr", "x-world/x-vrml");
            AddMimeMapping(".fif", "application/fractals");
            AddMimeMapping(".gtar", "application/x-gtar");
            AddMimeMapping(".gif", "image/gif");
            AddMimeMapping(".gz", "application/x-gzip");
            AddMimeMapping(".hta", "application/hta");
            AddMimeMapping(".htc", "text/x-component");
            AddMimeMapping(".htt", "text/webviewhtml");
            AddMimeMapping(".h", "text/plain");
            AddMimeMapping(".hdf", "application/x-hdf");
            AddMimeMapping(".hlp", "application/winhlp");
            AddMimeMapping(".html", "text/html");
            AddMimeMapping(".htm", "text/html");
            AddMimeMapping(".hqx", "application/mac-binhex40");
            AddMimeMapping(".isp", "application/x-internet-signup");
            AddMimeMapping(".iii", "application/x-iphone");
            AddMimeMapping(".ief", "image/ief");
            AddMimeMapping(".ivf", "video/x-ivf");
            AddMimeMapping(".ins", "application/x-internet-signup");
            AddMimeMapping(".ico", "image/x-icon");
            AddMimeMapping(".jpg", "image/jpeg");
            AddMimeMapping(".jfif", "image/pjpeg");
            AddMimeMapping(".jpe", "image/jpeg");
            AddMimeMapping(".jpeg", "image/jpeg");
            AddMimeMapping(".js", "application/x-javascript");
            AddMimeMapping(".lsx", "video/x-la-asf");
            AddMimeMapping(".latex", "application/x-latex");
            AddMimeMapping(".lsf", "video/x-la-asf");
            AddMimeMapping(".manifest", "application/x-ms-manifest");
            AddMimeMapping(".mhtml", "message/rfc822");
            AddMimeMapping(".mny", "application/x-msmoney");
            AddMimeMapping(".mht", "message/rfc822");
            AddMimeMapping(".mid", "audio/mid");
            AddMimeMapping(".mpv2", "video/mpeg");
            AddMimeMapping(".man", "application/x-troff-man");
            AddMimeMapping(".mvb", "application/x-msmediaview");
            AddMimeMapping(".mpeg", "video/mpeg");
            AddMimeMapping(".m3u", "audio/x-mpegurl");
            AddMimeMapping(".mdb", "application/x-msaccess");
            AddMimeMapping(".mpp", "application/vnd.ms-project");
            AddMimeMapping(".m1v", "video/mpeg");
            AddMimeMapping(".mpa", "video/mpeg");
            AddMimeMapping(".me", "application/x-troff-me");
            AddMimeMapping(".m13", "application/x-msmediaview");
            AddMimeMapping(".movie", "video/x-sgi-movie");
            AddMimeMapping(".m14", "application/x-msmediaview");
            AddMimeMapping(".mpe", "video/mpeg");
            AddMimeMapping(".mp2", "video/mpeg");
            AddMimeMapping(".mov", "video/quicktime");
            AddMimeMapping(".mp3", "audio/mpeg");
            AddMimeMapping(".mpg", "video/mpeg");
            AddMimeMapping(".ms", "application/x-troff-ms");
            AddMimeMapping(".nc", "application/x-netcdf");
            AddMimeMapping(".nws", "message/rfc822");
            AddMimeMapping(".oda", "application/oda");
            AddMimeMapping(".ods", "application/oleobject");
            AddMimeMapping(".pmc", "application/x-perfmon");
            AddMimeMapping(".p7r", "application/x-pkcs7-certreqresp");
            AddMimeMapping(".p7b", "application/x-pkcs7-certificates");
            AddMimeMapping(".p7s", "application/pkcs7-signature");
            AddMimeMapping(".pmw", "application/x-perfmon");
            AddMimeMapping(".ps", "application/postscript");
            AddMimeMapping(".p7c", "application/pkcs7-mime");
            AddMimeMapping(".pbm", "image/x-portable-bitmap");
            AddMimeMapping(".ppm", "image/x-portable-pixmap");
            AddMimeMapping(".pub", "application/x-mspublisher");
            AddMimeMapping(".pnm", "image/x-portable-anymap");
            AddMimeMapping(".pml", "application/x-perfmon");
            AddMimeMapping(".p10", "application/pkcs10");
            AddMimeMapping(".pfx", "application/x-pkcs12");
            AddMimeMapping(".p12", "application/x-pkcs12");
            AddMimeMapping(".pdf", "application/pdf");
            AddMimeMapping(".pps", "application/vnd.ms-powerpoint");
            AddMimeMapping(".p7m", "application/pkcs7-mime");
            AddMimeMapping(".pko", "application/vndms-pkipko");
            AddMimeMapping(".ppt", "application/vnd.ms-powerpoint");
            AddMimeMapping(".pmr", "application/x-perfmon");
            AddMimeMapping(".pma", "application/x-perfmon");
            AddMimeMapping(".pot", "application/vnd.ms-powerpoint");
            AddMimeMapping(".prf", "application/pics-rules");
            AddMimeMapping(".pgm", "image/x-portable-graymap");
            AddMimeMapping(".qt", "video/quicktime");
            AddMimeMapping(".ra", "audio/x-pn-realaudio");
            AddMimeMapping(".rgb", "image/x-rgb");
            AddMimeMapping(".ram", "audio/x-pn-realaudio");
            AddMimeMapping(".rmi", "audio/mid");
            AddMimeMapping(".ras", "image/x-cmu-raster");
            AddMimeMapping(".roff", "application/x-troff");
            AddMimeMapping(".rtf", "application/rtf");
            AddMimeMapping(".rtx", "text/richtext");
            AddMimeMapping(".sv4crc", "application/x-sv4crc");
            AddMimeMapping(".spc", "application/x-pkcs7-certificates");
            AddMimeMapping(".setreg", "application/set-registration-initiation");
            AddMimeMapping(".snd", "audio/basic");
            AddMimeMapping(".stl", "application/vndms-pkistl");
            AddMimeMapping(".setpay", "application/set-payment-initiation");
            AddMimeMapping(".stm", "text/html");
            AddMimeMapping(".shar", "application/x-shar");
            AddMimeMapping(".sh", "application/x-sh");
            AddMimeMapping(".sit", "application/x-stuffit");
            AddMimeMapping(".spl", "application/futuresplash");
            AddMimeMapping(".sct", "text/scriptlet");
            AddMimeMapping(".scd", "application/x-msschedule");
            AddMimeMapping(".sst", "application/vndms-pkicertstore");
            AddMimeMapping(".src", "application/x-wais-source");
            AddMimeMapping(".sv4cpio", "application/x-sv4cpio");
            AddMimeMapping(".tex", "application/x-tex");
            AddMimeMapping(".tgz", "application/x-compressed");
            AddMimeMapping(".t", "application/x-troff");
            AddMimeMapping(".tar", "application/x-tar");
            AddMimeMapping(".tr", "application/x-troff");
            AddMimeMapping(".tif", "image/tiff");
            AddMimeMapping(".txt", "text/plain");
            AddMimeMapping(".texinfo", "application/x-texinfo");
            AddMimeMapping(".trm", "application/x-msterminal");
            AddMimeMapping(".tiff", "image/tiff");
            AddMimeMapping(".tcl", "application/x-tcl");
            AddMimeMapping(".texi", "application/x-texinfo");
            AddMimeMapping(".tsv", "text/tab-separated-values");
            AddMimeMapping(".ustar", "application/x-ustar");
            AddMimeMapping(".uls", "text/iuls");
            AddMimeMapping(".vcf", "text/x-vcard");
            AddMimeMapping(".wps", "application/vnd.ms-works");
            AddMimeMapping(".wav", "audio/wav");
            AddMimeMapping(".wrz", "x-world/x-vrml");
            AddMimeMapping(".wri", "application/x-mswrite");
            AddMimeMapping(".wks", "application/vnd.ms-works");
            AddMimeMapping(".wmf", "application/x-msmetafile");
            AddMimeMapping(".wcm", "application/vnd.ms-works");
            AddMimeMapping(".wrl", "x-world/x-vrml");
            AddMimeMapping(".wdb", "application/vnd.ms-works");
            AddMimeMapping(".wsdl", "text/xml");
            AddMimeMapping(".xml", "text/xml");
            AddMimeMapping(".xlm", "application/vnd.ms-excel");
            AddMimeMapping(".xaf", "x-world/x-vrml");
            AddMimeMapping(".xla", "application/vnd.ms-excel");
            AddMimeMapping(".xls", "application/vnd.ms-excel");
            AddMimeMapping(".xof", "x-world/x-vrml");
            AddMimeMapping(".xlt", "application/vnd.ms-excel");
            AddMimeMapping(".xlc", "application/vnd.ms-excel");
            AddMimeMapping(".xsl", "text/xml");
            AddMimeMapping(".xbm", "image/x-xbitmap");
            AddMimeMapping(".xlw", "application/vnd.ms-excel");
            AddMimeMapping(".xpm", "image/x-xpixmap");
            AddMimeMapping(".xwd", "image/x-xwindowdump");
            AddMimeMapping(".xsd", "text/xml");
            AddMimeMapping(".z", "application/x-compress");
            AddMimeMapping(".zip", "application/x-zip-compressed");
            AddMimeMapping(".*", "application/octet-stream");
        }

        private MimeMapping()
        {
        }

        private static void AddMimeMapping(string extension, string MimeType)
        {
            _extensionToMimeMappingTable.Add(extension, MimeType);
        }

        internal static string GetMimeMapping(string FileName)
        {
            string str = null;
            int startIndex = FileName.LastIndexOf('.');
            if ((0 < startIndex) && (startIndex > FileName.LastIndexOf('\\')))
            {
                str = (string)_extensionToMimeMappingTable[FileName.Substring(startIndex)];
            }
            if (str == null)
            {
                str = (string)_extensionToMimeMappingTable[".*"];
            }
            return str;
        }
    }
    #endregion

    #region HttpAsyncResult

    internal class HttpAsyncResult : IAsyncResult
    {
        // Fields
        private object _asyncState;
        private AsyncCallback _callback;
        private bool _completed;
        private bool _completedSynchronously;
        private Exception _error;
        private object _result;
        private RequestNotificationStatus _status;

        // Methods
        internal HttpAsyncResult(AsyncCallback cb, object state)
        {
            this._callback = cb;
            this._asyncState = state;
            this._status = RequestNotificationStatus.Continue;
        }

        internal HttpAsyncResult(AsyncCallback cb, object state, bool completed, object result, Exception error)
        {
            this._callback = cb;
            this._asyncState = state;
            this._completed = completed;
            this._completedSynchronously = completed;
            this._result = result;
            this._error = error;
            this._status = RequestNotificationStatus.Continue;
            if (this._completed && (this._callback != null))
            {
                this._callback(this);
            }
        }

        internal void Complete(bool synchronous, object result, Exception error)
        {
            this.Complete(synchronous, result, error, RequestNotificationStatus.Continue);
        }

        internal void Complete(bool synchronous, object result, Exception error, RequestNotificationStatus status)
        {
            this._completed = true;
            this._completedSynchronously = synchronous;
            this._result = result;
            this._error = error;
            this._status = status;
            if (this._callback != null)
            {
                this._callback(this);
            }
        }

        internal object End()
        {
            if (this._error != null)
            {
                throw new HttpException(null, this._error);
            }
            return this._result;
        }

        internal void SetComplete()
        {
            this._completed = true;
        }

        // Properties
        public object AsyncState
        {
            get
            {
                return this._asyncState;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return this._completedSynchronously;
            }
        }

        internal Exception Error
        {
            get
            {
                return this._error;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this._completed;
            }
        }

        internal RequestNotificationStatus Status
        {
            get
            {
                return this._status;
            }
        }
    }




    #endregion
}
