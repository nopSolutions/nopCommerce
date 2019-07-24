/*
* Kendo UI Web v2014.1.318 (http://kendoui.com)
* Copyright 2014 Telerik AD. All rights reserved.
*
* Kendo UI Web commercial licenses may be obtained at
* http://www.telerik.com/purchase/license-agreement/kendo-ui-web
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/
!function(e,define){define(["./kendo.popup.min"],e)}(function(){return function(e,t){var n=window.kendo,i=n.mobile.ui,r=n.ui.Popup,o='<div class="km-shim"/>',a=i.Widget,s=a.extend({init:function(t,i){var s=this,l=n.mobile.application,u=n.support.mobileOS,c=l?l.os.name:u?u.name:"ios",d="ios"===c||"wp"===c||(l?l.os.skin:!1),p="blackberry"===c,f=i.align||(d?"bottom center":p?"center right":"center center"),h=i.position||(d?"bottom center":p?"center right":"center center"),g=i.effect||(d?"slideIn:up":p?"slideIn:left":"fade:in"),m=e(o).handler(s).hide();a.fn.init.call(s,t,i),s.shim=m,t=s.element,i=s.options,i.className&&s.shim.addClass(i.className),i.modal||s.shim.on("up","hide"),(l?l.element:e(document.body)).append(m),s.popup=new r(s.element,{anchor:m,modal:!0,appendTo:m,origin:f,position:h,animation:{open:{effects:g,duration:i.duration},close:{duration:i.duration}},deactivate:function(){m.hide()},open:function(){m.show()}}),n.notify(s)},options:{name:"Shim",modal:!1,align:t,position:t,effect:t,duration:200},show:function(){this.popup.open()},hide:function(t){t&&e.contains(this.shim.children().children(".k-popup")[0],t.target)||this.popup.close()},destroy:function(){a.fn.destroy.call(this),this.shim.kendoDestroy(),this.popup.destroy(),this.shim.remove()}});i.plugin(s)}(window.kendo.jQuery),window.kendo},"function"==typeof define&&define.amd?define:function(e,t){t()});