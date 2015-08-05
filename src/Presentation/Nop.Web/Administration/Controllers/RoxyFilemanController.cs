using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web;
using Nop.Services.Security;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
{
    //Controller for Roxy fileman (http://www.roxyfileman.com/) for TinyMCE editor
    //the original file was \RoxyFileman-1.4.3-net\fileman\asp_net\main.ashx
    //some custom changes by wooncherk contribution

    //do not validate request token (XSRF)
    [AdminAntiForgery(true)]
    public class RoxyFilemanController : BaseAdminController
    {
        #region Fields

        Dictionary<string, string> _settings = null;
        Dictionary<string, string> _lang = null;
        //custom code by nopCommerce team
        string confFile = "~/Content/Roxy_Fileman/conf.json";
        
        //custom code by nopCommerce team
        private readonly IPermissionService _permissionService;
        private readonly HttpContextBase _context;
        private readonly HttpResponseBase _r;

        #endregion

        #region Ctor

        //custom code by nopCommerce team
        public RoxyFilemanController(IPermissionService permissionService, HttpContextBase context)
        {
            this._permissionService = permissionService;
            this._context = context;
            this._r = this._context.Response;
        }

        #endregion
        
        #region Methods

        public void ProcessRequest() {
            string action = "DIRLIST";

            //custom code by nopCommerce team
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                _r.Write(GetErrorRes("You don't have required permission"));

            try{
                if (_context.Request["a"] != null)
                    action = (string)_context.Request["a"];

                //custom code by nopCommerce team
                //VerifyAction(action);
                switch (action.ToUpper())
                {
                    case "DIRLIST":
                        ListDirTree(_context.Request["type"]);
                        break;
                    case "FILESLIST":
                        ListFiles(_context.Request["d"], _context.Request["type"]);
                        break;
                    case "COPYDIR":
                        CopyDir(_context.Request["d"], _context.Request["n"]);
                        break;
                    case "COPYFILE":
                        CopyFile(_context.Request["f"], _context.Request["n"]);
                        break;
                    case "CREATEDIR":
                        CreateDir(_context.Request["d"], _context.Request["n"]);
                        break;
                    case "DELETEDIR":
                        DeleteDir(_context.Request["d"]);
                        break;
                    case "DELETEFILE":
                        DeleteFile(_context.Request["f"]);
                        break;
                    case "DOWNLOAD":
                        DownloadFile(_context.Request["f"]);
                        break;
                    case "DOWNLOADDIR":
                        DownloadDir(_context.Request["d"]);
                        break;
                    case "MOVEDIR":
                        MoveDir(_context.Request["d"], _context.Request["n"]);
                        break;
                    case "MOVEFILE":
                        MoveFile(_context.Request["f"], _context.Request["n"]);
                        break;
                    case "RENAMEDIR":
                        RenameDir(_context.Request["d"], _context.Request["n"]);
                        break;
                    case "RENAMEFILE":
                        RenameFile(_context.Request["f"], _context.Request["n"]);
                        break;
                    case "GENERATETHUMB":
                        int w = 140, h = 0;
                        int.TryParse(_context.Request["width"].Replace("px", ""), out w);
                        int.TryParse(_context.Request["height"].Replace("px", ""), out h);
                        ShowThumbnail(_context.Request["f"], w, h);
                        break;
                    case "UPLOAD":
                        Upload(_context.Request["d"]);
                        break;
                    default:
                        _r.Write(GetErrorRes("This action is not implemented."));
                        break;
                }
        
            }
            catch(Exception ex){
                if (action == "UPLOAD" && !IsAjaxUpload())
                {
                    _r.Write("<script>");
                    _r.Write("parent.fileUploaded(" + GetErrorRes(LangRes("E_UploadNoFiles")) + ");");
                    _r.Write("</script>");
                }
                else{
                    _r.Write(GetErrorRes(ex.Message));
                }
            }
        
        }
        
        #endregion

        #region Utitlies

        private string FixPath(string path)
        {
            //custom code by nopCommerce team
            if (path == null)
                path = "";

            if (!path.StartsWith("~")){
                if (!path.StartsWith("/"))
                    path = "/" + path;
                path = "~" + path;
            }

            //custom code by nopCommerce team
            var rootDirectory = GetSetting("FILES_ROOT");
            if (!path.ToLowerInvariant().Contains(rootDirectory.ToLowerInvariant()))
                path = rootDirectory;

            return _context.Server.MapPath(path);
        }
        private string GetLangFile(){
            string filename = "../lang/" + GetSetting("LANG") + ".json";
            if (!System.IO.File.Exists(_context.Server.MapPath(filename)))
                filename = "../lang/en.json";
            return filename;
        }
        protected string LangRes(string name)
        {
            string ret = name;
            if (_lang == null)
                _lang = ParseJSON(GetLangFile());
            if (_lang.ContainsKey(name))
                ret = _lang[name];

            return ret;
        }
        protected string GetFileType(string ext){
            string ret = "file";
            ext = ext.ToLower();
            if(ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                ret = "image";
              else if(ext == ".swf" || ext == ".flv")
                ret = "flash";
            return ret;
        }
        protected bool CanHandleFile(string filename)
        {
            bool ret = false;
            FileInfo file = new FileInfo(filename);
            string ext = file.Extension.Replace(".", "").ToLower();
            string setting = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new ArrayList();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = true;
            }
            setting = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new ArrayList();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = false;
            }
        
            return ret;
        }
        protected Dictionary<string, string> ParseJSON(string file){
            Dictionary<string, string> ret = new Dictionary<string,string>();
            string json = "";
            try{
                json = System.IO.File.ReadAllText(_context.Server.MapPath(file), System.Text.Encoding.UTF8);
            }
            catch{}

            json = json.Trim();
            if(json != ""){
                if (json.StartsWith("{"))
                    json = json.Substring(1, json.Length - 2);
                json = json.Trim();
                json = json.Substring(1, json.Length - 2);
                string[] lines = Regex.Split(json, "\"\\s*,\\s*\"");
                foreach(string line in lines){
                    string[] tmp = Regex.Split(line, "\"\\s*:\\s*\"");
                    try{
                        if (tmp[0] != "" && !ret.ContainsKey(tmp[0]))
                        {
                           ret.Add(tmp[0], tmp[1]);
                        }
                    }
                    catch{}
                }
            }
            return ret;
        }
        protected string GetFilesRoot(){
            string ret = GetSetting("FILES_ROOT");
            if (GetSetting("SESSION_PATH_KEY") != "" && _context.Session[GetSetting("SESSION_PATH_KEY")] != null)
                ret = (string)_context.Session[GetSetting("SESSION_PATH_KEY")];
        
            if(ret == "")
                ret = _context.Server.MapPath("../Uploads");
            else
                ret = FixPath(ret);
            return ret;
        }
        protected void LoadConf(){
            if(_settings == null)
                _settings = ParseJSON(confFile);
        }
        protected string GetSetting(string name){
            string ret = "";
            LoadConf();
            if(_settings.ContainsKey(name))
                ret = _settings[name];
        
            return ret;
        }
        protected void CheckPath(string path)
        {
            if (FixPath(path).IndexOf(GetFilesRoot()) != 0)
            {
                throw new Exception("Access to " + path + " is denied");
            }
        }
        protected void VerifyAction(string action)
        {
            string setting = GetSetting(action);
            if (setting.IndexOf("?") > -1)
                setting = setting.Substring(0, setting.IndexOf("?"));
            if (!setting.StartsWith("/"))
                setting = "/" + setting;
            setting = ".." + setting;
        
            if (_context.Server.MapPath(setting) != _context.Server.MapPath(_context.Request.Url.LocalPath))
                throw new Exception(LangRes("E_ActionDisabled"));
        }
        protected string GetResultStr(string type, string msg)
        {
            return "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace("\"","\\\"") + "\"}";
        }
        protected string GetSuccessRes(string msg)
        {
            return GetResultStr("ok", msg);
        }
        protected string GetSuccessRes()
        {
            return GetSuccessRes("");
        }
        protected string GetErrorRes(string msg)
        {
            return GetResultStr("error", msg);
        }
        private void _copyDir(string path, string dest){
            if(!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            foreach(string f in  Directory.GetFiles(path)){
                FileInfo file = new FileInfo(f);
                if (!System.IO.File.Exists(Path.Combine(dest, file.Name)))
                {
                    System.IO.File.Copy(f, Path.Combine(dest, file.Name));
                }
            }
            foreach (string d in Directory.GetDirectories(path))
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                _copyDir(d, Path.Combine(dest, dir.Name));
            }
        }
        protected void CopyDir(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            DirectoryInfo dir = new  DirectoryInfo(FixPath(path));
            DirectoryInfo newDir = new DirectoryInfo(FixPath(newPath + "/" + dir.Name));
        
            if (!dir.Exists)
            {
                throw new Exception(LangRes("E_CopyDirInvalidPath"));    
            }
            else if (newDir.Exists)
            {
                throw new Exception(LangRes("E_DirAlreadyExists"));
            }
            else{
                _copyDir(dir.FullName, newDir.FullName);
            }
            _r.Write(GetSuccessRes());
        }
        protected string MakeUniqueFilename(string dir, string filename){
            string ret = filename;
            int i = 0;
            while (System.IO.File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Path.GetFileNameWithoutExtension(filename) + " - Copy " + i.ToString() + Path.GetExtension(filename);
            }
            return ret;
        }
        protected void CopyFile(string path, string newPath)
        {
            CheckPath(path);
            FileInfo file = new FileInfo(FixPath(path));
            newPath = FixPath(newPath);
            if (!file.Exists)
                throw new Exception(LangRes("E_CopyFileInvalisPath"));
            else{
                string newName = MakeUniqueFilename(newPath, file.Name);
                try{
                    System.IO.File.Copy(file.FullName, Path.Combine(newPath, newName));
                    _r.Write(GetSuccessRes());
                }
                catch{
                    throw new Exception(LangRes("E_CopyFile"));
                }
            }
        }
        protected void CreateDir(string path, string name)
        {
            CheckPath(path);
            path = FixPath(path);
            if(!Directory.Exists(path))
                throw new Exception(LangRes("E_CreateDirInvalidPath"));
            else{
                try
                {
                    path = Path.Combine(path, name);
                    if(!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    _r.Write(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CreateDirFailed"));
                }
            }
        }
        protected void DeleteDir(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!Directory.Exists(path))
                throw new Exception(LangRes("E_DeleteDirInvalidPath"));
            else if (path == GetFilesRoot())
                throw new Exception(LangRes("E_CannotDeleteRoot")); 
            else if(Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0)
                throw new Exception(LangRes("E_DeleteNonEmpty"));
            else
            {
                try
                {
                    Directory.Delete(path);
                    _r.Write(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CannotDeleteDir"));
                }
            }
        }
        protected void DeleteFile(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!System.IO.File.Exists(path))
                throw new Exception(LangRes("E_DeleteFileInvalidPath"));
            else
            {
                try
                {
                    System.IO.File.Delete(path);
                    _r.Write(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_DeletеFile"));
                }
            }
        }
        private List<string> GetFiles(string path, string type){
            List<string> ret = new List<string>();
            if(type == "#")
                type = "";
            string[] files = Directory.GetFiles(path);
            foreach(string f in files){
                if ((GetFileType(new FileInfo(f).Extension) == type) || (type == ""))
                    ret.Add(f);
            }
            return ret;
        }
        private ArrayList ListDirs(string path){
            string[] dirs = Directory.GetDirectories(path);
            ArrayList ret = new ArrayList();
            foreach(string dir in dirs){
                ret.Add(dir);
                ret.AddRange(ListDirs(dir));
            }
            return ret;
        }
        protected void ListDirTree(string type)
        {
            DirectoryInfo d = new DirectoryInfo(GetFilesRoot());
            if(!d.Exists)
                throw new Exception("Invalid files root directory. Check your configuration.");
            
            ArrayList dirs = ListDirs(d.FullName);
            dirs.Insert(0, d.FullName);
        
            string localPath = _context.Server.MapPath("~/");
            _r.Write("[");
            for(int i = 0; i <dirs.Count; i++){
                string dir = (string) dirs[i];
                _r.Write("{\"p\":\"/" + dir.Replace(localPath, "").Replace("\\", "/") + "\",\"f\":\"" + GetFiles(dir, type).Count.ToString() + "\",\"d\":\"" + Directory.GetDirectories(dir).Length.ToString() + "\"}");
                if(i < dirs.Count -1)
                    _r.Write(",");
            }
            _r.Write("]");
        }
        protected double LinuxTimestamp(DateTime d){
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);
        
            return timeSpan.TotalSeconds;

        }
        protected void ListFiles(string path, string type)
        {
            CheckPath(path);
            string fullPath = FixPath(path);
            List<string> files = GetFiles(fullPath, type);
            _r.Write("[");
            for(int i = 0; i < files.Count; i++){
                FileInfo f = new FileInfo(files[i]);
                int w = 0, h = 0;
                if (GetFileType(f.Extension) == "image"){
                    try{
                        FileStream fs = new FileStream(f.FullName, FileMode.Open);
                        Image img = Image.FromStream(fs);
                        w = img.Width;
                        h = img.Height;
                        fs.Close();
                        fs.Dispose();
                        img.Dispose();
                    }
                    catch(Exception ex){throw ex;}
                }
                _r.Write("{");
                _r.Write("\"p\":\""+path + "/" + f.Name+"\"");
                _r.Write(",\"t\":\"" + Math.Ceiling(LinuxTimestamp(f.LastWriteTime)).ToString() + "\"");
                _r.Write(",\"s\":\""+f.Length.ToString()+"\"");
                _r.Write(",\"w\":\""+w.ToString()+"\"");
                _r.Write(",\"h\":\""+h.ToString()+"\"");
                _r.Write("}");
                if (i < files.Count - 1)
                    _r.Write(",");
            }
            _r.Write("]");
        }
        public void DownloadDir(string path)
        {
            path = FixPath(path);
            if(!Directory.Exists(path))
                throw new Exception(LangRes("E_CreateArchive"));
            string dirName = new FileInfo(path).Name;
            string tmpZip = _context.Server.MapPath("../tmp/" + dirName + ".zip");
            if (System.IO.File.Exists(tmpZip))
                System.IO.File.Delete(tmpZip);
            ZipFile.CreateFromDirectory(path, tmpZip,CompressionLevel.Fastest, true);
            _r.Clear();
            _r.Headers.Add("Content-Disposition", "attachment; filename=\"" + dirName + ".zip\"");
            _r.ContentType = "application/force-download";
            _r.TransmitFile(tmpZip);
            _r.Flush();
            System.IO.File.Delete(tmpZip);
            _r.End();
        }
        protected void DownloadFile(string path)
        {
            CheckPath(path);
            FileInfo file = new FileInfo(FixPath(path));
            if(file.Exists){
                _r.Clear();
                _r.Headers.Add("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
                _r.ContentType = "application/force-download";
                _r.TransmitFile(file.FullName);
                _r.Flush();
                _r.End();
            }
        }
        protected void MoveDir(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            DirectoryInfo source = new DirectoryInfo(FixPath(path));
            DirectoryInfo dest = new DirectoryInfo(FixPath(Path.Combine(newPath, source.Name)));
            if(dest.FullName.IndexOf(source.FullName) == 0)
                throw new Exception(LangRes("E_CannotMoveDirToChild"));
            else if (!source.Exists)
                throw new Exception(LangRes("E_MoveDirInvalisPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_DirAlreadyExists"));
            else{
                try{
                    source.MoveTo(dest.FullName);
                    _r.Write(GetSuccessRes());
                }
                catch{
                    throw new Exception(LangRes("E_MoveDir") + " \"" + path + "\"");
                }
            }
        
        }
        protected void MoveFile(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            FileInfo source = new FileInfo(FixPath(path));
            FileInfo dest = new FileInfo(FixPath(newPath));
            if (!source.Exists)
                throw new Exception(LangRes("E_MoveFileInvalisPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_MoveFileAlreadyExists"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    _r.Write(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_MoveFile") + " \"" + path + "\"");
                }
            }
        }
        protected void RenameDir(string path, string name)
        {
            CheckPath(path);
            DirectoryInfo source = new DirectoryInfo(FixPath(path));
            DirectoryInfo dest = new DirectoryInfo(Path.Combine(source.Parent.FullName, name));
            if(source.FullName == GetFilesRoot())
                throw new Exception(LangRes("E_CannotRenameRoot"));
            else if (!source.Exists)
                throw new Exception(LangRes("E_RenameDirInvalidPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_DirAlreadyExists"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    _r.Write(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_RenameDir") + " \"" + path + "\"");
                }
            }
        }
        protected void RenameFile(string path, string name)
        {
            CheckPath(path);
            FileInfo source = new FileInfo(FixPath(path));
            FileInfo dest = new FileInfo(Path.Combine(source.Directory.FullName, name));
            if (!source.Exists)
                throw new Exception(LangRes("E_RenameFileInvalidPath"));
            else if (!CanHandleFile(name))
                throw new Exception(LangRes("E_FileExtensionForbidden"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    _r.Write(GetSuccessRes());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "; " + LangRes("E_RenameFile") + " \"" + path + "\"");
                }
            }
        }
        public bool ThumbnailCallback()
        {
            return false;
        }

        protected void ShowThumbnail(string path, int width, int height)
        {
            CheckPath(path);
            FileStream fs = new FileStream(FixPath(path), FileMode.Open);
            Bitmap img = new Bitmap(Bitmap.FromStream(fs));
            fs.Close();
            fs.Dispose();
            int cropWidth = img.Width, cropHeight = img.Height;
            int cropX = 0, cropY = 0;

            double imgRatio = (double)img.Width / (double)img.Height;
        
            if(height == 0)
                height = Convert.ToInt32(Math.Floor((double)width / imgRatio));

            if (width > img.Width)
                width = img.Width;
            if (height > img.Height)
                height = img.Height;

            double cropRatio = (double)width / (double)height;
            cropWidth = Convert.ToInt32(Math.Floor((double)img.Height * cropRatio));
            cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
            if (cropWidth > img.Width)
            {
                cropWidth = img.Width;
                cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
            }
            if (cropHeight > img.Height)
            {
                cropHeight = img.Height;
                cropWidth = Convert.ToInt32(Math.Floor((double)cropHeight * cropRatio));
            }
            if(cropWidth < img.Width){
                cropX = Convert.ToInt32(Math.Floor((double)(img.Width - cropWidth) / 2));
            }
            if(cropHeight < img.Height){
                cropY = Convert.ToInt32(Math.Floor((double)(img.Height - cropHeight) / 2));
            }

            Rectangle area = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropImg = img.Clone(area, System.Drawing.Imaging.PixelFormat.DontCare);
            img.Dispose();
            Image.GetThumbnailImageAbort imgCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            _r.AddHeader("Content-Type", "image/png");
            cropImg.GetThumbnailImage(width, height, imgCallback, IntPtr.Zero).Save(_r.OutputStream, ImageFormat.Png);
            _r.OutputStream.Close();
            cropImg.Dispose();
        }
        private ImageFormat GetImageFormat(string filename){
            ImageFormat ret = ImageFormat.Jpeg;
            switch(new FileInfo(filename).Extension.ToLower()){
                case ".png": ret = ImageFormat.Png; break;
                case ".gif": ret = ImageFormat.Gif; break;
            }
            return ret;
        }
        protected void ImageResize(string path, string dest, int width, int height)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            Image img = Image.FromStream(fs);
            fs.Close();
            fs.Dispose();
            float ratio = (float)img.Width / (float)img.Height;
            if ((img.Width <= width && img.Height <= height) || (width == 0 && height == 0))
                return;

            int newWidth = width;
            int newHeight = Convert.ToInt16(Math.Floor((float)newWidth / ratio));
            if ((height > 0 && newHeight > height) || (width == 0))
            {
                newHeight = height;
                newWidth = Convert.ToInt16(Math.Floor((float)newHeight * ratio));
            }
            Bitmap newImg = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)newImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, 0, 0, newWidth, newHeight);
            img.Dispose();
            g.Dispose();
            if(dest != ""){
                newImg.Save(dest, GetImageFormat(dest));
            }
            newImg.Dispose();
        }
        protected bool IsAjaxUpload()
        {
            return (_context.Request["method"] != null && _context.Request["method"].ToString() == "ajax");
        }
        protected void Upload(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            string res = GetSuccessRes();
            bool hasErrors = false;
            try{
                for(int i = 0; i < Request.Files.Count; i++){
                    if (CanHandleFile(Request.Files[i].FileName))
                    {
                        FileInfo f = new FileInfo(Request.Files[i].FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        Request.Files[i].SaveAs(dest);
                        if (GetFileType(new FileInfo(filename).Extension) == "image")
                        {
                            int w = 0;
                            int h = 0;
                            int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out w);
                            int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out h);
                            ImageResize(dest, dest, w, h);
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        res = GetSuccessRes(LangRes("E_UploadNotAll"));
                    }
                }
            }
            catch(Exception ex){
                res = GetErrorRes(ex.Message);
            }
            if (IsAjaxUpload())
            {
                if(hasErrors)
                    res = GetErrorRes(LangRes("E_UploadNotAll"));
                _r.Write(res);
            }
            else
            {
                _r.Write("<script>");
                _r.Write("parent.fileUploaded(" + res + ");");
                _r.Write("</script>");
            }
        }
        
        #endregion

    }
}