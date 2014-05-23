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
function selectDir(item){
  var d = Directory.Parse($(item).parent('li').attr('data-path'));
  if(d)
    d.Select();
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
function addFile(){
  clickFirstOnEnter('dlgAddFile');

  var dialogButtons = {};
  dialogButtons[t('Upload')] = function(){
    if(!$('#fileUploads').val())
      alert(t('E_SelectFiles'));
    else{
      if(!RoxyFilemanConf.UPLOAD){
        alert(t('E_ActionDisabled'));
        //$('#dlgAddFile').dialog('close');
      }
      else{
        document.forms['addfile'].action = RoxyFilemanConf.UPLOAD;
        document.forms['addfile'].submit();
      }
    }
  };
  dialogButtons[t('Cancel')] = function(){$('#dlgAddFile').dialog('close');};

  $('#dlgAddFile').dialog({title:t('T_AddFile'),modal:true,buttons:dialogButtons});
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
  dialogButtons[t('Rename')] = function(){
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
  dialogButtons[t('Rename')] = function(){
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
$(function(){
  RoxyUtils.LoadConfig();
  var d = new Directory();
  d.LoadAll();
  $('#wraper').show();
  window.setTimeout('selectFirst()', 300);
  RoxyUtils.Translate();
  $('body').click(function(){
    closeMenus();
  });
  if(RoxyFilemanConf.DEFAULTVIEW)
    switchView(RoxyFilemanConf.DEFAULTVIEW);
  ResizeLists();
  $(".actions input").tooltip({track: true});
  $( window ).resize(ResizeLists);
  if(RoxyFilemanConf.INTEGRATION.toLowerCase() == 'tinymce3'){
    $('body').append('<script src="js/tiny_mce_popup.js"><\/script>');
  }
  document.oncontextmenu = function() {return false;};
  $('#copyYear').html(new Date().getFullYear());
});
function setFile(){
  var f = getSelectedFile();
  if(!f){
    alert(t('E_NoFileSelected'));
    return;
  }
  var integration = RoxyUtils.GetUrlParam('integration');
  if(!integration)
    integration = RoxyFilemanConf.INTEGRATION;
  switch(integration.toLowerCase()){
      case 'ckeditor':
      window.opener.CKEDITOR.tools.callFunction(RoxyUtils.GetUrlParam('CKEditorFuncNum'), f.fullPath);
      self.close();
    break;
    case 'tinymce3':
      var URL = f.fullPath;
      var win = tinyMCEPopup.getWindowArg("window");
      win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value = URL;
      if (typeof(win.ImageDialog) != "undefined") {
          if (win.ImageDialog.getImageData)
              win.ImageDialog.getImageData();

          if (win.ImageDialog.showPreviewImage)
              win.ImageDialog.showPreviewImage(URL);
      }
      tinyMCEPopup.close();
    break;
    case 'tinymce4':
      var win = (window.opener?window.opener:window.parent);
      win.document.getElementById(RoxyUtils.GetUrlParam('input')).value = f.fullPath;
      if (typeof(win.ImageDialog) != "undefined") {
          if (win.ImageDialog.getImageData)
              win.ImageDialog.getImageData();
          if (win.ImageDialog.showPreviewImage)
              win.ImageDialog.showPreviewImage(f.fullPath);
      }
      win.tinyMCE.activeEditor.windowManager.close();
    break;
    default:
      FileSelected(f);
    break;
  }
}