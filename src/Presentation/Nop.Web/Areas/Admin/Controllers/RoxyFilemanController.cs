#if NET451
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
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
        string confFile = "~/wwwroot/lib/Roxy_Fileman/conf.json";
        
        //custom code by nopCommerce team
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        //custom code by nopCommerce team
        public RoxyFilemanController(IPermissionService permissionService, IHttpContextAccessor httpContextAccessor)
        {
            this._permissionService = permissionService;
            this._httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        public virtual void ProcessRequest() {
            string action = "DIRLIST";

            //custom code by nopCommerce team
            if (!_permissionService.Authorize(StandardPermissionProvider.HtmlEditorManagePictures))
                _httpContextAccessor.HttpContext.Response.WriteAsync(GetErrorRes("You don't have required permission"));

            try{
                if (!StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Form["a"]))
                    action = _httpContextAccessor.HttpContext.Request.Form["a"];

                //custom code by nopCommerce team
                //VerifyAction(action);
                switch (action.ToUpper())
                {
                    case "DIRLIST":
                        ListDirTree(_httpContextAccessor.HttpContext.Request.Form["type"]);
                        break;
                    case "FILESLIST":
                        ListFiles(_httpContextAccessor.HttpContext.Request.Form["d"], _httpContextAccessor.HttpContext.Request.Form["type"]);
                        break;
                    case "COPYDIR":
                        CopyDir(_httpContextAccessor.HttpContext.Request.Form["d"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "COPYFILE":
                        CopyFile(_httpContextAccessor.HttpContext.Request.Form["f"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "CREATEDIR":
                        CreateDir(_httpContextAccessor.HttpContext.Request.Form["d"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "DELETEDIR":
                        DeleteDir(_httpContextAccessor.HttpContext.Request.Form["d"]);
                        break;
                    case "DELETEFILE":
                        DeleteFile(_httpContextAccessor.HttpContext.Request.Form["f"]);
                        break;
                    case "DOWNLOAD":
                        DownloadFile(_httpContextAccessor.HttpContext.Request.Form["f"]);
                        break;
                    case "DOWNLOADDIR":
                        DownloadDir(_httpContextAccessor.HttpContext.Request.Form["d"]);
                        break;
                    case "MOVEDIR":
                        MoveDir(_httpContextAccessor.HttpContext.Request.Form["d"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "MOVEFILE":
                        MoveFile(_httpContextAccessor.HttpContext.Request.Form["f"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "RENAMEDIR":
                        RenameDir(_httpContextAccessor.HttpContext.Request.Form["d"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "RENAMEFILE":
                        RenameFile(_httpContextAccessor.HttpContext.Request.Form["f"], _httpContextAccessor.HttpContext.Request.Form["n"]);
                        break;
                    case "GENERATETHUMB":
                        int w = 140, h = 0;
                        int.TryParse(_httpContextAccessor.HttpContext.Request.Form["width"].ToString().Replace("px", ""), out w);
                        int.TryParse(_httpContextAccessor.HttpContext.Request.Form["height"].ToString().Replace("px", ""), out h);
                        ShowThumbnail(_httpContextAccessor.HttpContext.Request.Form["f"], w, h);
                        break;
                    case "UPLOAD":
                        Upload(_httpContextAccessor.HttpContext.Request.Form["d"]);
                        break;
                    default:
                        _httpContextAccessor.HttpContext.Response.WriteAsync(GetErrorRes("This action is not implemented."));
                        break;
                }
        
            }
            catch(Exception ex){
                if (action == "UPLOAD" && !IsAjaxUpload())
                {
                    _httpContextAccessor.HttpContext.Response.WriteAsync("<script>");
                    _httpContextAccessor.HttpContext.Response.WriteAsync(
                        "parent.fileUploaded(" + GetErrorRes(LangRes("E_UploadNoFiles")) + ");");
                    _httpContextAccessor.HttpContext.Response.WriteAsync("</script>");
                }
                else{
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetErrorRes(ex.Message));
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

            return CommonHelper.MapPath(path);
        }

        private string GetLangFile(){
            string filename = "../lang/" + GetSetting("LANG") + ".json";
            if (!System.IO.File.Exists(CommonHelper.MapPath(filename)))
                filename = "../lang/en.json";
            return filename;
        }

        protected virtual string LangRes(string name)
        {
            string ret = name;
            if (_lang == null)
                _lang = ParseJSON(GetLangFile());
            if (_lang.ContainsKey(name))
                ret = _lang[name];

            return ret;
        }

        protected virtual string GetFileType(string ext){
            string ret = "file";
            ext = ext.ToLower();
            if(ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                ret = "image";
              else if(ext == ".swf" || ext == ".flv")
                ret = "flash";
            return ret;
        }

        protected virtual bool CanHandleFile(string filename)
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

        protected virtual Dictionary<string, string> ParseJSON(string file){
            Dictionary<string, string> ret = new Dictionary<string,string>();
            string json = "";
            try{
                json = System.IO.File.ReadAllText(CommonHelper.MapPath(file), System.Text.Encoding.UTF8);
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

        protected virtual string GetFilesRoot(){
            string ret = GetSetting("FILES_ROOT");
            if (GetSetting("SESSION_PATH_KEY") != "" &&
                _httpContextAccessor.HttpContext.Session.GetString(GetSetting("SESSION_PATH_KEY")) != null)
                ret = _httpContextAccessor.HttpContext.Session.GetString(GetSetting("SESSION_PATH_KEY"));
        
            if(ret == "")
                ret = CommonHelper.MapPath("../Uploads");
            else
                ret = FixPath(ret);
            return ret;
        }

        protected virtual void LoadConf(){
            if(_settings == null)
                _settings = ParseJSON(confFile);
        }

        protected virtual string GetSetting(string name){
            string ret = "";
            LoadConf();
            if(_settings.ContainsKey(name))
                ret = _settings[name];
        
            return ret;
        }

        protected virtual void CheckPath(string path)
        {
            if (FixPath(path).IndexOf(GetFilesRoot()) != 0)
            {
                throw new Exception("Access to " + path + " is denied");
            }
        }

        protected virtual void VerifyAction(string action)
        {
            string setting = GetSetting(action);
            if (setting.IndexOf("?") > -1)
                setting = setting.Substring(0, setting.IndexOf("?"));
            if (!setting.StartsWith("/"))
                setting = "/" + setting;
            setting = ".." + setting;
        
            if (CommonHelper.MapPath(setting) != CommonHelper.MapPath(_httpContextAccessor.HttpContext.Request.Path))
                throw new Exception(LangRes("E_ActionDisabled"));
        }

        protected virtual string GetResultStr(string type, string msg)
        {
            return "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace("\"","\\\"") + "\"}";
        }

        protected virtual string GetSuccessRes(string msg)
        {
            return GetResultStr("ok", msg);
        }

        protected virtual string GetSuccessRes()
        {
            return GetSuccessRes("");
        }

        protected virtual string GetErrorRes(string msg)
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

        protected virtual void CopyDir(string path, string newPath)
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
            _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
        }

        protected virtual string MakeUniqueFilename(string dir, string filename){
            string ret = filename;
            int i = 0;
            while (System.IO.File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Path.GetFileNameWithoutExtension(filename) + " - Copy " + i.ToString() + Path.GetExtension(filename);
            }
            return ret;
        }

        protected virtual void CopyFile(string path, string newPath)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch{
                    throw new Exception(LangRes("E_CopyFile"));
                }
            }
        }

        protected virtual void CreateDir(string path, string name)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CreateDirFailed"));
                }
            }
        }

        protected virtual void DeleteDir(string path)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CannotDeleteDir"));
                }
            }
        }

        protected virtual void DeleteFile(string path)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
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

        protected virtual void ListDirTree(string type)
        {
            DirectoryInfo d = new DirectoryInfo(GetFilesRoot());
            if(!d.Exists)
                throw new Exception("Invalid files root directory. Check your configuration.");
            
            ArrayList dirs = ListDirs(d.FullName);
            dirs.Insert(0, d.FullName);
        
            string localPath = CommonHelper.MapPath("~/");
            _httpContextAccessor.HttpContext.Response.WriteAsync("[");
            for(int i = 0; i <dirs.Count; i++){
                string dir = (string) dirs[i];
                _httpContextAccessor.HttpContext.Response.WriteAsync(
                    "{\"p\":\"/" + dir.Replace(localPath, "").Replace("\\", "/") + "\",\"f\":\"" +
                    GetFiles(dir, type).Count + "\",\"d\":\"" +
                    Directory.GetDirectories(dir).Length + "\"}");
                if(i < dirs.Count -1)
                    _httpContextAccessor.HttpContext.Response.WriteAsync(",");
            }
            _httpContextAccessor.HttpContext.Response.WriteAsync("]");
        }

        protected virtual double LinuxTimestamp(DateTime d){
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);
        
            return timeSpan.TotalSeconds;

        }

        protected virtual void ListFiles(string path, string type)
        {
            CheckPath(path);
            string fullPath = FixPath(path);
            List<string> files = GetFiles(fullPath, type);
            _httpContextAccessor.HttpContext.Response.WriteAsync("[");
            for(int i = 0; i < files.Count; i++){
                FileInfo f = new FileInfo(files[i]);
                int w = 0, h = 0;
                if (GetFileType(f.Extension) == "image"){
                    try{
                        using (FileStream fs = new FileStream(f.FullName, FileMode.Open))
                        {
                            using (Image img = Image.FromStream(fs))
                            {
                                w = img.Width;
                                h = img.Height;
                            }
                        }                        
                    }
                    catch(Exception ex){throw ex;}
                }
                _httpContextAccessor.HttpContext.Response.WriteAsync("{");
                _httpContextAccessor.HttpContext.Response.WriteAsync("\"p\":\"" + path + "/" + f.Name + "\"");
                _httpContextAccessor.HttpContext.Response.WriteAsync(",\"t\":\"" + Math.Ceiling(LinuxTimestamp(f.LastWriteTime)) + "\"");
                _httpContextAccessor.HttpContext.Response.WriteAsync(",\"s\":\"" + f.Length + "\"");
                _httpContextAccessor.HttpContext.Response.WriteAsync(",\"w\":\"" + w + "\"");
                _httpContextAccessor.HttpContext.Response.WriteAsync(",\"h\":\"" + h + "\"");
                _httpContextAccessor.HttpContext.Response.WriteAsync("}");
                if (i < files.Count - 1)
                    _httpContextAccessor.HttpContext.Response.WriteAsync(",");
            }
            _httpContextAccessor.HttpContext.Response.WriteAsync("]");
        }

        public virtual void DownloadDir(string path)
        {
            path = FixPath(path);
            if(!Directory.Exists(path))
                throw new Exception(LangRes("E_CreateArchive"));
            string dirName = new FileInfo(path).Name;
            string tmpZip = CommonHelper.MapPath("../tmp/" + dirName + ".zip");
            if (System.IO.File.Exists(tmpZip))
                System.IO.File.Delete(tmpZip);
            ZipFile.CreateFromDirectory(path, tmpZip,CompressionLevel.Fastest, true);
            _httpContextAccessor.HttpContext.Response.Clear();
            _httpContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + dirName + ".zip\"");
            _httpContextAccessor.HttpContext.Response.ContentType = MimeTypes.ApplicationForceDownload;
            _r.TransmitFile(tmpZip);
            _r.Flush();
            System.IO.File.Delete(tmpZip);
            _r.End();
        }

        protected virtual void DownloadFile(string path)
        {
            CheckPath(path);
            FileInfo file = new FileInfo(FixPath(path));
            if(file.Exists){
                _r.Clear();
                _r.Headers.Add("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
                _r.ContentType = MimeTypes.ApplicationForceDownload;
                _r.TransmitFile(file.FullName);
                _r.Flush();
                _r.End();
            }
        }

        protected virtual void MoveDir(string path, string newPath)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch{
                    throw new Exception(LangRes("E_MoveDir") + " \"" + path + "\"");
                }
            }
        
        }

        protected virtual void MoveFile(string path, string newPath)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_MoveFile") + " \"" + path + "\"");
                }
            }
        }

        protected virtual void RenameDir(string path, string name)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_RenameDir") + " \"" + path + "\"");
                }
            }
        }

        protected virtual void RenameFile(string path, string name)
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
                    _httpContextAccessor.HttpContext.Response.WriteAsync(GetSuccessRes());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "; " + LangRes("E_RenameFile") + " \"" + path + "\"");
                }
            }
        }

        public virtual bool ThumbnailCallback()
        {
            return false;
        }

        protected virtual void ShowThumbnail(string path, int width, int height)
        {
            CheckPath(path);
            FileStream fs = new FileStream(FixPath(path), FileMode.Open);
            Bitmap img = new Bitmap(Bitmap.FromStream(fs));
            fs.Close();
            fs.Dispose();
            int cropX = 0, cropY = 0;

            double imgRatio = (double)img.Width / (double)img.Height;
        
            if(height == 0)
                height = Convert.ToInt32(Math.Floor((double)width / imgRatio));

            if (width > img.Width)
                width = img.Width;
            if (height > img.Height)
                height = img.Height;

            double cropRatio = (double)width / (double)height;
            int cropWidth = Convert.ToInt32(Math.Floor((double)img.Height * cropRatio));
            int cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
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

            _httpContextAccessor.HttpContext.Response.Headers.Add("Content-Type", MimeTypes.ImagePng);
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
        protected virtual void ImageResize(string path, string dest, int width, int height)
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
        protected virtual bool IsAjaxUpload()
        {
            return !StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Form["method"]) &&
                   _httpContextAccessor.HttpContext.Request.Form["method"].ToString() == "ajax";
        }
        protected virtual void Upload(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            string res = GetSuccessRes();
            bool hasErrors = false;
            try{
                for(int i = 0; i < _httpContextAccessor.HttpContext.Request.Form.Files.Count; i++){
                    if (CanHandleFile(_httpContextAccessor.HttpContext.Request.Form.Files[i].FileName))
                    {
                        FileInfo f = new FileInfo(_httpContextAccessor.HttpContext.Request.Form.Files[i].FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        _httpContextAccessor.HttpContext.Request.Form.Files[i].SaveAs(dest);
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
                _httpContextAccessor.HttpContext.Response.WriteAsync(res);
            }
            else
            {
                _httpContextAccessor.HttpContext.Response.WriteAsync("<script>");
                _httpContextAccessor.HttpContext.Response.WriteAsync("parent.fileUploaded(" + res + ");");
                _httpContextAccessor.HttpContext.Response.WriteAsync("</script>");
            }
        }
        
        #endregion
    }
}
#endif