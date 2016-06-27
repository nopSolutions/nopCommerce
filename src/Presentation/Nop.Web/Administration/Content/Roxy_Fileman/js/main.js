/*
  RoxyFileman - web based file manager. Ready to use with CKEditor, TinyMCE. 
  Can be easily integrated with any other WYSIWYG editor or CMS.

  Copyright (C) 2013, RoxyFileman.com - Lyubomir Arsov. All rights reserved.
  For licensing, see LICENSE.txt or http://RoxyFileman.com/license

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.

  Contact: Lyubomir Arsov, liubo (at) web-lobby.com
*/

$.ajaxSetup ({cache: false});
function selectFile(item){
  $('#pnlFileList li').removeClass('selected');
  $(item).prop('class', 'selected');
  var html = RoxyUtils.GetFilename($(item).attr('data-path'));
  html += ' ('+t('Size')+': '+RoxyUtils.FormatFileSize($(item).attr('data-size'));
  if($(item).attr('data-w') > 0)
    html += ', '+t('Dimensions')+':'+$(item).attr('data-w')+'x'+$(item).attr('data-h');
  html += ')';
  $('#pnlStatus').html(html);
}
function getLastDir(){
  return RoxyUtils.GetCookie('roxyld');
}
function setLastDir(path){
  RoxyUtils.SetCookie('roxyld', path, 10);
}
function selectDir(item){
  var d = Directory.Parse($(item).parent('li').attr('data-path'));
  if(d){
    d.Select();
  }
}
function startDragDir(){

}
function startDragFile(){
  selectFile(this);
}
function dragFileOver(){
  $(this).children('img.dir').attr('src', 'images/folder-green.png');
}
function dragFileOut(){
  $('#pnlDirList').find('img.dir').attr('src', 'images/folder.png');
}
function makeDragFile(e){
  var f = new File($(e.target).closest('li').attr('data-path'));
  return '<div class="pnlDragFile" data-path="'+f.fullPath+'"><img src="'+f.bigIcon+'" align="absmiddle">&nbsp;'+f.name+'</div>';
}
function makeDragDir(e){
  var f = new Directory($(e.target).attr('data-path')?$(e.target).attr('data-path'):$(e.target).closest('li').attr('data-path'));
  return '<div class="pnlDragDir" data-path="'+f.fullPath+'"><img src="images/folder.png" align="absmiddle">&nbsp;'+f.name+'</div>';
}
function moveDir(e, ui, obj){
  var dir = Directory.Parse(ui.draggable.attr('data-path'));
  var target = Directory.Parse($(obj).parent('li').attr('data-path'));
  if(target.fullPath != dir.path)
    dir.Move(target.fullPath);
}
function moveFile(e, ui, obj){
  var f = new File(ui.draggable.attr('data-path'));
  var d = Directory.Parse($(obj).parent('li').attr('data-path'));
  var src = Directory.Parse(f.path);
  if(f.path != d.fullPath)
    f.Move(d.fullPath);
}
function moveObject(e, ui){
  e.stopPropagation();
  if(ui.draggable.hasClass('directory'))
    moveDir(e, ui, this);
  else
    moveFile(e, ui, this);
  dragFileOut();
}
function clickFirstOnEnter(elId){
  $('#'+elId).unbind('keypress');
  $('.actions input').each(function(){this.blur();});
  $('#'+elId).keypress(function(e) {
      if (e.keyCode == $.ui.keyCode.ENTER) {
        e.stopPropagation();
        $(this).parent().find('.ui-dialog-buttonset button').eq(0).trigger('click');
      }
    });
}
function addDir(){
  var f = getSelectedDir();
  if(!f)
    return;
  clickFirstOnEnter('pnlDirName');
  $('#txtDirName').val('');

  var dialogButtons = {};
  dialogButtons[t('CreateDir')] = function(){
    var newName = $.trim($('#txtDirName').val());
    if(!newName)
      alert(t('E_MissingDirName'));
    if(f.Create(newName)){
      $('#pnlDirName').dialog('close');
    }
  };
  dialogButtons[t('Cancel')] = function(){$('#pnlDirName').dialog('close');};
  $('#pnlDirName').dialog({title: t('T_CreateDir'),modal:true,buttons:dialogButtons});
}
var uploadFileList = new Array();
function showUploadList(files){
  var filesPane = $('#uploadFilesList');
  filesPane.html('');
  clearFileField();
  for(i = 0; i < files.length; i++){
    filesPane.append('<div class="fileUpload"><div class="fileName">'+files[i].name+' ('+RoxyUtils.FormatFileSize(files[i].size)+')<span class="progressPercent"></span><div class="uploadProgress"><div class="stripes"></div></div></div><a class="removeUpload" onclick="removeUpload(' + i + ')"></a></div>');
  }
  if(files.length > 0)
    $('#btnUpload').button('enable');
  else
    $('#btnUpload').button('disable');
}
function listUploadFiles(files){
  if(!window.FileList) {
    $('#btnUpload').button('enable');
  }
  else if(files.length > 0) {
    uploadFileList = new Array();
    addUploadFiles(files);
  }
}
function addUploadFiles(files){
  for(i = 0; i < files.length; i++)
    uploadFileList.push(files[i]);
  showUploadList(uploadFileList);
}
function removeUpload(i){
  var el = findUploadElement(i);
  el.remove();
  try{
    uploadFileList.splice(i, 1);
    showUploadList(uploadFileList);
  }
  catch(ex){
    //alert(ex); 
  }
}
function findUploadElement(i){
  return $('#uploadFilesList .fileUpload:eq(' + (i)+ ')');
} 
function updateUploadProgress(e, i){
  var el = findUploadElement(i);
  var percent = 99;
  if (e.lengthComputable) {
    percent = Math.floor((e.loaded / e.total) * 100);
  }
  if(percent > 99)
    percent = 99;
  el.find('.uploadProgress').css('width', percent + '%');
  el.find('.progressPercent').html(' - ' + percent + '%');
}
function uploadComplete(e, i){
  uploadFinished(e, i, 'ok');
}
function uploadError(e, i){
  setUploadError(i);
  uploadFinished(e, i, 'error');
}
function setUploadError(i){
  var el = findUploadElement(i);
  el.find('.uploadProgress').css('width', '100%').addClass('uploadError').removeClass('uploadComplete');
  el.find('.progressPercent').html(' - <span class="error">' + t('E_UploadingFile')+'</span>');
}
function setUploadSuccess(i){
  var el = findUploadElement(i);
  el.find('.uploadProgress').css('width', '100%').removeClass('uploadError').addClass('uploadComplete');
  el.find('.progressPercent').html(' - 100%');
}
function uploadCanceled(e, i){
  uploadFinished(e, i, 'error');
}
function uploadFinished(e, i, res){
  var el = findUploadElement(i);
  var httpRes = null;
  try{
    httpRes = JSON.parse(e.target.responseText);
  }
  catch(ex){}
  
  if((httpRes && httpRes.res == 'error') || res != 'ok'){
    res = 'error';
    setUploadError(i);
  }
  else{
    res = 'ok';
    setUploadSuccess(i)
  }
    
  el.attr('data-ulpoad', res);
  checkUploadResult();
}
function checkUploadResult(){
  var all = $('#uploadFilesList .fileUpload').length;
  var completed = $('#uploadFilesList .fileUpload[data-ulpoad]').length;
  var success = $('#uploadFilesList .fileUpload[data-ulpoad="ok"]').length;
  if(completed == all){
     //$('#uploadResult').html(success + ' files uploaded; '+(all - success)+' failed');
     uploadFileList = new Array();
     var d = Directory.Parse($('#hdDir').val());
     d.ListFiles(true);
     $('#btnUpload').button('disable');
  }
}
function fileUpload(f, i){
  var http = new XMLHttpRequest();
  var fData = new FormData();
  var el = findUploadElement(i);
  el.find('.removeUpload').remove();
  fData.append("action", 'upload');
  fData.append("method", 'ajax');
  fData.append("d", $('#hdDir').attr('value'));
  fData.append("files[]", f);
  http.upload.addEventListener("progress", function(e){updateUploadProgress(e, i);}, false);
  http.addEventListener("load", function(e){uploadComplete(e, i);}, false);
  http.addEventListener("error", function(e){uploadError(e, i);}, false);
  http.addEventListener("abort", function(e){uploadCanceled(e, i);}, false);
  http.open("POST", RoxyFilemanConf.UPLOAD, true);
  http.setRequestHeader("Accept", "*/*");
  http.send(fData);
}
function dropFiles(e, append){
  if(e && e.dataTransfer && e.dataTransfer.files){
    addFile();
    if(append)
      addUploadFiles(e.dataTransfer.files);
    else
      listUploadFiles(e.dataTransfer.files);
  }
  else
    addFile();
}
function clearFileField(selector){
  if(!selector)
    selector = '#fileUploads';
  try{
    $(selector).val('');
    $(selector).val(null);
  }
  catch(ex){}
}
function addFileClick(){
  $('#uploadResult').html('');
  showUploadList(new Array());
  addFile();
}
function addFile(){
  clickFirstOnEnter('dlgAddFile');
  $('#uploadResult').html('');
  clearFileField();
  var dialogButtons = {};
  dialogButtons[t('Upload')] = {id:'btnUpload', text: t('Upload'), disabled:true, click:function(){
    if(!$('#fileUploads').val() && (!uploadFileList || uploadFileList.length == 0))
      alert(t('E_SelectFiles'));
    else{
      if(!RoxyFilemanConf.UPLOAD){
        alert(t('E_ActionDisabled'));
        //$('#dlgAddFile').dialog('close');
      }
      else{
        if(window.FormData && window.XMLHttpRequest && window.FileList && uploadFileList && uploadFileList.length > 0){
          for(i = 0; i < uploadFileList.length; i++){
            fileUpload(uploadFileList[i], i);
          } 
        }
        else{
          document.forms['addfile'].action = RoxyFilemanConf.UPLOAD;
          document.forms['addfile'].submit();
        }
      }
    }
  }};
  
  dialogButtons[t('Cancel')] = function(){$('#dlgAddFile').dialog('close');};
  $('#dlgAddFile').dialog({title:t('T_AddFile'),modal:true,buttons:dialogButtons,width:400});
}
function fileUploaded(res){
  if(res.res == 'ok' && res.msg){
     $('#dlgAddFile').dialog('close');
     var d = Directory.Parse($('#hdDir').val());
     d.ListFiles(true);
     alert(res.msg);
  }
  else if(res.res == 'ok'){
     $('#dlgAddFile').dialog('close');
     var d = Directory.Parse($('#hdDir').val());
     d.ListFiles(true);
  }
  else
    alert(res.msg);
}
function renameDir(){
  var f = getSelectedDir();
  if(!f)
    return;
  if($('[data-path="'+f.fullPath+'"]').parents('li').length < 1){
     alert(t('E_CannotRenameRoot'));
     return;
  }
  clickFirstOnEnter('pnlDirName');
  $('#txtDirName').val(f.name);

  var dialogButtons = {};
  dialogButtons[t('RenameDir')] = function(){
    var newName = $.trim($('#txtDirName').val());
    if(!newName)
      alert(t('E_MissingDirName'));
    if(f.Rename(newName))
      $('#pnlDirName').dialog('close');
  };
  dialogButtons[t('Cancel')] = function(){$('#pnlDirName').dialog('close');};

  $('#pnlDirName').dialog({title:t('T_RenameDir'),modal:true,buttons:dialogButtons});
  RoxyUtils.SelectText('txtDirName', 0, new String(f.name).length);
}
function renameFile(){
  var f = getSelectedFile();
  if(!f)
    return;
  clickFirstOnEnter('pnlRenameFile');
  $('#txtFileName').val(f.name);

  var dialogButtons = {};
  dialogButtons[t('RenameFile')] = function(){
    var newName = $.trim($('#txtFileName').val());
    if(!newName)
      alert('Missing file name');
    else if(f.Rename(newName)){
      $('li[data-path="'+f.fullPath+'"] .name').text(newName);
      $('li[data-path="'+f.fullPath+'"]').attr('data-path', RoxyUtils.MakePath(f.path, newName));
      $('#pnlRenameFile').dialog('close');
    }
  };
  dialogButtons[t('Cancel')] = function(){$('#pnlRenameFile').dialog('close');};

  $('#pnlRenameFile').dialog({title:t('T_RenameFile'),modal:true,buttons:dialogButtons});
  if(f.name.lastIndexOf('.') > 0)
    RoxyUtils.SelectText('txtFileName', 0, f.name.lastIndexOf('.'));
}
function getSelectedFile(){
  var ret = null;
  if($('#pnlFileList .selected').length > 0)
    ret = new File($('#pnlFileList .selected').attr('data-path'));
  return ret;
}
function getSelectedDir(){
  var ret = null;
  if($('#pnlDirList .selected'))
    ret = Directory.Parse($('#pnlDirList .selected').closest('li').attr('data-path'));

  return ret;
}
function deleteDir(path){
  var d = null;
  if(path)
    d = Directory.Parse(path);
  else
    d = getSelectedDir();

  if(d && confirm(t('Q_DeleteFolder'))){
    d.Delete();
  }
}
function deleteFile(){
  var f = getSelectedFile();
  if(f && confirm(t('Q_DeleteFile'))){
    f.Delete();
  }
}
function previewFile(){
  var f = getSelectedFile();
  if(f){
    window.open(f.fullPath);
  }
}
function downloadFile(){
  var f = getSelectedFile();
  if(f && RoxyFilemanConf.DOWNLOAD){
    var url = RoxyUtils.AddParam(RoxyFilemanConf.DOWNLOAD, 'f', f.fullPath);
    window.frames['frmUploadFile'].location.href = url;
  }
  else if(!RoxyFilemanConf.DOWNLOAD)
    alert(t('E_ActionDisabled'));
}
function downloadDir(){
  var d = getSelectedDir();
  if(d && RoxyFilemanConf.DOWNLOADDIR){
    var url = RoxyUtils.AddParam(RoxyFilemanConf.DOWNLOADDIR, 'd', d.fullPath);
    window.frames['frmUploadFile'].location.href = url;
  }
  else if(!RoxyFilemanConf.DOWNLOAD)
    alert(t('E_ActionDisabled'));
}
function closeMenus(el){
  if(!el || el == 'dir')
    $('#menuDir').fadeOut();
  if(!el || el == 'file')
    $('#menuFile').fadeOut();
}
function selectFirst(){
  var item = $('#pnlDirList li:first').children('div').first();
  if(item.length > 0)
    selectDir(item);
  else
    window.setTimeout('selectFirst()', 300);
}
function tooltipContent(){
  if($('#menuFile').is(':visible'))
    return '';
  var html = '';
  var f = File.Parse($(this).attr('data-path'));
  if($('#hdViewType').val() == 'thumb' && f.IsImage()){
    html = f.fullPath+'<br><span class="filesize">'+t('Size')+': '+RoxyUtils.FormatFileSize(f.size) + ' '+t('Dimensions')+': '+f.width+'x'+f.height+'</span>';
  }
  else if(f.IsImage()){
    if(RoxyFilemanConf.GENERATETHUMB){
      imgUrl = RoxyUtils.AddParam(RoxyFilemanConf.GENERATETHUMB, 'f', f.fullPath);
      imgUrl = RoxyUtils.AddParam(imgUrl, 'width', RoxyFilemanConf.PREVIEW_THUMB_WIDTH);
      imgUrl = RoxyUtils.AddParam(imgUrl, 'height', RoxyFilemanConf.PREVIEW_THUMB_HEIGHT);
    }
    else
      imgUrl = f.fullPath;
    html = '<img src="'+imgUrl+'" class="imgPreview"><br>'+f.name+' <br><span class="filesize">'+t('Size')+': '+RoxyUtils.FormatFileSize(f.size) + ' '+t('Dimensions')+': '+f.width+'x'+f.height+'</span>';
  }
  else
    html = f.fullPath+' <span class="filesize">'+t('Size')+': '+RoxyUtils.FormatFileSize(f.size) + '</span>';
  return html;
}
function filterFiles(){
  var str = $('#txtSearch').val();
  $('#pnlSearchNoFiles').hide();
  if($('#pnlFileList li').length == 0)
    return;
  if(!str){
    $('#pnlFileList li').show();
    return;
  }
  var i = 0;
  $('#pnlFileList li').each(function(){
      var name = $(this).children('.name').text();
      if(name.toLowerCase().indexOf(str.toLowerCase()) > -1){
        i++;
        $(this).show();
      }
      else{
        $(this).removeClass('selected');
        $(this).hide();
      }
  });
  if(i == 0)
    $('#pnlSearchNoFiles').show();
}
function sortFiles(){
  var d = getSelectedDir();
  if(!d)
    return;
  d.ListFiles();
  filterFiles();
  switchView($('#hdViewType').val());
}
function switchView(t){
  if(t == $('#hdViewType').val())
    return;
  if(!t)
    t = $('#hdViewType').val();
  $('.btnView').removeClass('selected');
  if(t == 'thumb'){
    $('#pnlFileList .icon').attr('src', 'images/blank.gif');
    $('#pnlFileList').addClass('thumbView');
    if($('#dynStyle').length == 0){
      $('head').append('<style id="dynStyle" />');
      var rules = 'ul#pnlFileList.thumbView li{width:'+RoxyFilemanConf.THUMBS_VIEW_WIDTH+'px;}';
      rules += 'ul#pnlFileList.thumbView li{height:'+(parseInt(RoxyFilemanConf.THUMBS_VIEW_HEIGHT) + 20)+'px;}';
      rules += 'ul#pnlFileList.thumbView .icon{width:'+RoxyFilemanConf.THUMBS_VIEW_WIDTH+'px;}';
      rules += 'ul#pnlFileList.thumbView .icon{height:'+RoxyFilemanConf.THUMBS_VIEW_HEIGHT+'px;}';
      $('#dynStyle').html(rules);
    }
    $('#pnlFileList li').each(function(){
      
      //$('ul#pnlFileList.thumbView li').css('width', RoxyFilemanConf.THUMBS_VIEW_WIDTH + 'px');
      //$('ul#pnlFileList.thumbView li').css('height', (parseInt(RoxyFilemanConf.THUMBS_VIEW_HEIGHT) + 20) + 'px');
      //$('ul#pnlFileList.thumbView .icon').css('width', RoxyFilemanConf.THUMBS_VIEW_WIDTH + 'px');
      //$('ul#pnlFileList.thumbView .icon').css('height', RoxyFilemanConf.THUMBS_VIEW_HEIGHT + 'px');
      var imgUrl = $(this).attr('data-icon-big');
      if(RoxyFilemanConf.GENERATETHUMB && RoxyUtils.IsImage($(this).attr('data-path'))){
        imgUrl = RoxyUtils.AddParam(RoxyFilemanConf.GENERATETHUMB, 'f', imgUrl);
        imgUrl = RoxyUtils.AddParam(imgUrl, 'width', RoxyFilemanConf.THUMBS_VIEW_WIDTH);
        imgUrl = RoxyUtils.AddParam(imgUrl, 'height', RoxyFilemanConf.THUMBS_VIEW_HEIGHT);
      }
      $(this).children('.icon').css('background-image', 'url('+imgUrl+')');
      $(this).tooltip('option', 'show', {delay:50});
    });
    $('#btnThumbView').addClass('selected');
  }
  else{
    $('#pnlFileList').removeClass('thumbView');
    $('#pnlFileList li').each(function(){
      $(this).children('.icon').css('background-image','').attr('src', $(this).attr('data-icon'));
      $(this).tooltip('option', 'show', {delay:500});
    });
    $('#btnListView').addClass('selected');
  }
  $('#hdViewType').val(t);
  RoxyUtils.SetCookie('roxyview', t, 10);
}
var clipBoard = null;
function Clipboard(a, obj){
  this.action = a;
  this.obj = obj;
}
function cutDir(){
  var d = getSelectedDir();
  if(d){
    setClipboard('cut', d);
    d.GetElement().addClass('pale');
  }
}
function copyDir(){
  var d = getSelectedDir();
  if(d){
    setClipboard('copy', d);
  }
}
function cutFile(){
  var f = getSelectedFile();
  if(f){
    setClipboard('cut', f);
    f.GetElement().addClass('pale');
  }
}
function copyFile(){
  var f = getSelectedFile();
  if(f){
    setClipboard('copy', f);
  }
}
function pasteToFiles(e, el){
  if($(el).hasClass('pale')){
    e.stopPropagation();
    return false;
  }
  var d = getSelectedDir();
  if(!d)
    d = Directory.Parse($('#pnlDirList li:first').children('div').first());
  if(d && clipBoard && clipBoard.obj){
    if(clipBoard.action == 'copy')
      clipBoard.obj.Copy(d.fullPath);
    else{
      clipBoard.obj.Move(d.fullPath);
      clearClipboard();
    }
  }
  return true;
}
function pasteToDirs(e, el){
  if($(el).hasClass('pale')){
    e.stopPropagation();
    return false;
  }
  var d = getSelectedDir();
  if(!d)
    d = Directory.Parse($('#pnlDirList li:first').children('div').first());
  if(clipBoard && d){
    if(clipBoard.action == 'copy')
      clipBoard.obj.Copy(d.fullPath);
    else{
      clipBoard.obj.Move(d.fullPath);
      clearClipboard();
      d.ListFiles(true);
    }
  }
  else
    alert('error');
  return true;
}
function clearClipboard(){
  $('#pnlDirList li').removeClass('pale');
  $('#pnlFileList li').removeClass('pale');
  clipBoard = null;
  $('.paste').addClass('pale');
}
function setClipboard(a, obj){
  clearClipboard();
  if(obj){
    clipBoard = new Clipboard(a, obj);
    $('.paste').removeClass('pale');
  }
}
function ResizeLists(){
  var tmp = $(window).innerHeight() - $('#fileActions .actions').outerHeight() - $('.bottomLine').outerHeight();
  $('.scrollPane').css('height', tmp);
}
function removeDisabledActions(){
  if(RoxyFilemanConf.CREATEDIR == ''){
    $('#mnuCreateDir').next().remove();
    $('#mnuCreateDir').remove();
    $('#btnAddDir').remove();
  }
  if(RoxyFilemanConf.DELETEDIR == ''){
    $('#mnuDeleteDir').prev().remove();
    $('#mnuDeleteDir').remove();
    $('#btnDeleteDir').remove();
  }
  if(RoxyFilemanConf.MOVEDIR == ''){
    $('#mnuDirCut').next().remove();
    $('#mnuDirCut').remove();
  }
  if(RoxyFilemanConf.COPYDIR == ''){
    $('#mnuDirCopy').next().remove();
    $('#mnuDirCopy').remove();
  }
  if(RoxyFilemanConf.COPYDIR == '' && RoxyFilemanConf.MOVEDIR == ''){
    $('#mnuDirPaste').next().remove();
    $('#mnuDirPaste').remove();
  }
  if(RoxyFilemanConf.RENAMEDIR == ''){
    $('#mnuRenameDir').next().remove();
    $('#mnuRenameDir').remove();
    $('#btnRenameDir').remove();
  }
  if(RoxyFilemanConf.UPLOAD == ''){
    $('#btnAddFile').remove();
  }
  if(RoxyFilemanConf.DOWNLOAD == ''){
    $('#mnuDownload').next().remove();
    $('#mnuDownload').remove();
  }
  if(RoxyFilemanConf.DOWNLOADDIR == ''){
    $('#mnuDownloadDir').next().remove();
    $('#mnuDownloadDir').remove();
  }
  if(RoxyFilemanConf.DELETEFILE == ''){
    $('#mnuDeleteFile').prev().remove();
    $('#mnuDeleteFile').remove();
    $('#btnDeleteFile').remove();
  }
  if(RoxyFilemanConf.MOVEFILE == ''){
    $('#mnuFileCut').next().remove();
    $('#mnuFileCut').remove();
  }
  if(RoxyFilemanConf.COPYFILE == ''){
    $('#mnuFileCopy').next().remove();
    $('#mnuFileCopy').remove();
  }
  if(RoxyFilemanConf.COPYFILE == '' && RoxyFilemanConf.MOVEFILE == ''){
    $('#mnuFilePaste').next().remove();
    $('#mnuFilePaste').remove();
  }
  if(RoxyFilemanConf.RENAMEFILE == ''){
    $('#mnuRenameFile').next().remove();
    $('#mnuRenameFile').remove();
    $('#btnRenameFile').remove();
  }
}
function getPreselectedFile(){
  var filePath = RoxyUtils.GetUrlParam('selected');
  if(!filePath){
    switch(getFilemanIntegration()){
      case 'ckeditor':
        try{
          var dialog = window.opener.CKEDITOR.dialog.getCurrent();
          filePath = dialog.getValueOf('info', (dialog.getName() == 'link'?'url':'txtUrl'));
        }
        catch(ex){}
      break;
      case 'tinymce3':
        try{
          var win = tinyMCEPopup.getWindowArg("window");
          filePath = win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value;
          if(filePath.indexOf('..') == 0)
            filePath = filePath.substr(2);
        }
        catch(ex){}
      break;
      case 'tinymce4':
        try{
          var win = (window.opener?window.opener:window.parent);
          filePath = win.document.getElementById(RoxyUtils.GetUrlParam('input')).value;
          if(filePath.indexOf('..') == 0)
            filePath = filePath.substr(2);
        }
        catch(ex){}
      break;
      default:
        filePath = GetSelectedValue();     
      break;
    }
  }
  if(RoxyFilemanConf.RETURN_URL_PREFIX){
    var prefix = RoxyFilemanConf.RETURN_URL_PREFIX;
    if(filePath.indexOf(prefix) == 0){
      if(prefix.substr(-1) == '/')
        prefix = prefix.substr(0, prefix.length - 1);
      filePath = filePath.substr(prefix.length);
    }
  }
  
  return filePath;
}
function initSelection(filePath){
  var hasSelection = false, fileSelected = true;
  if(!filePath)
    filePath = getPreselectedFile();
  if(!filePath && RoxyUtils.ToBool(RoxyFilemanConf.OPEN_LAST_DIR)){
    filePath = getLastDir();
    fileSelected = false;
  }
  if(filePath){
    var p = (fileSelected? RoxyUtils.GetPath(filePath): filePath);
    var d = tmp = Directory.Parse(p);
    do{
      if(tmp){
        tmp.Expand(true);
        hasSelection = true; 
      }
      tmp = Directory.Parse(tmp.path);
    }while(tmp);
    
    if(d){
      d.Select(filePath);
      hasSelection = true; 
    }
  }
  if(!hasSelection)
    selectFirst();
}
$(function(){
  RoxyUtils.LoadConfig();
  var d = new Directory();
  d.LoadAll();
  $('#wraper').show();
  
  window.setTimeout('initSelection()', 100);

  RoxyUtils.Translate();
  $('body').click(function(){
    closeMenus();
  });
  
  var viewType = RoxyUtils.GetCookie('roxyview');
  if(!viewType)
    viewType = RoxyFilemanConf.DEFAULTVIEW;
  if(viewType)
    switchView(viewType);
    
  ResizeLists();
  $(".actions input").tooltip({track: true});
  $( window ).resize(ResizeLists);
  
  document.oncontextmenu = function() {return false;};
  removeDisabledActions();
  $('#copyYear').html(new Date().getFullYear());
  if(RoxyFilemanConf.UPLOAD && RoxyFilemanConf.UPLOAD != ''){
    var dropZone = document.getElementById('fileActions');
    dropZone.ondragover = function () { return false; };
    dropZone.ondragend = function () { return false; };
    dropZone.ondrop = function (e) {
      e.preventDefault();
      e.stopPropagation();
      dropFiles(e);
    };
    
    dropZone = document.getElementById('dlgAddFile');
    dropZone.ondragover = function () { return false; };
    dropZone.ondragend = function () { return false; };
    dropZone.ondrop = function (e) {
      e.preventDefault();
      e.stopPropagation();
      dropFiles(e, true);
    };
  }
  
  if(getFilemanIntegration() == 'tinymce3'){
    try {
      $('body').append('<script src="js/tiny_mce_popup.js"><\/script>');
    }
    catch(ex){}
  }
});
function getFilemanIntegration(){
  var integration = RoxyUtils.GetUrlParam('integration');
  if(!integration)
    integration = RoxyFilemanConf.INTEGRATION;
    
  return integration.toLowerCase();
}
function setFile(){
  var f = getSelectedFile();
  if(!f){
    alert(t('E_NoFileSelected'));
    return;
  }
  var insertPath = f.fullPath;
  if(RoxyFilemanConf.RETURN_URL_PREFIX){
    var prefix = RoxyFilemanConf.RETURN_URL_PREFIX;
    if(prefix.substr(-1) == '/')
      prefix = prefix.substr(0, prefix.length - 1);
    insertPath = prefix + (insertPath.substr(0, 1) != '/'? '/': '') + insertPath;
  }
  switch(getFilemanIntegration()){
      case 'ckeditor':
      window.opener.CKEDITOR.tools.callFunction(RoxyUtils.GetUrlParam('CKEditorFuncNum'), insertPath);
      self.close();
    break;
    case 'tinymce3':
      var win = tinyMCEPopup.getWindowArg("window");
      win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value = insertPath;
      if (typeof(win.ImageDialog) != "undefined") {
          if (win.ImageDialog.getImageData)
              win.ImageDialog.getImageData();

          if (win.ImageDialog.showPreviewImage)
              win.ImageDialog.showPreviewImage(insertPath);
      }
      tinyMCEPopup.close();
    break;
    case 'tinymce4':
      var win = (window.opener?window.opener:window.parent);
      win.document.getElementById(RoxyUtils.GetUrlParam('input')).value = insertPath;
      if (typeof(win.ImageDialog) != "undefined") {
          if (win.ImageDialog.getImageData)
              win.ImageDialog.getImageData();
          if (win.ImageDialog.showPreviewImage)
              win.ImageDialog.showPreviewImage(insertPath);
      }
      win.tinyMCE.activeEditor.windowManager.close();
    break;
    default:
      FileSelected(f);
    break;
  }
}