/*
* Kendo UI Web v2013.3.1119 (http://kendoui.com)
* Copyright 2013 Telerik AD. All rights reserved.
*
* Kendo UI Web commercial licenses may be obtained at
* https://www.kendoui.com/purchase/license-agreement/kendo-ui-web-commercial.aspx
* If you do not own a commercial license, this file shall be governed by the
* GNU General Public License (GPL) version 3.
* For GPL requirements, please review: http://www.gnu.org/copyleft/gpl.html
*/
!function(define){return define(["./kendo.core.min","./kendo.binder.min"],function(){!function(e){var n=window.kendo,t=n.Observable,i="SCRIPT",r="init",o="show",d="hide",a=/unrecognized expression/,c=t.extend({init:function(e,n){var i=this;n=n||{},t.fn.init.call(i),i.content=e,i.tagName=n.tagName||"div",i.model=n.model,i._wrap=n.wrap!==!1,i.bind([r,o,d],n)},render:function(t){var i=this,d=!i.element;return d&&(i.element=i._createElement()),t&&e(t).append(i.element),d&&(n.bind(i.element,i.model),i.trigger(r)),t&&i.trigger(o),i.element},hide:function(){this.element.detach(),this.trigger(d)},destroy:function(){var e=this.element;e&&(n.unbind(e),n.destroy(e),e.remove())},_createElement:function(){var n,t,r=this;try{t=e(document.getElementById(r.content)||r.content)}catch(o){a.test(o.message)&&(t=r.content)}return n=e("<"+r.tagName+" />").append(t[0].tagName===i?t.html():t),r._wrap||(n=n.contents()),n}}),s=c.extend({init:function(e,n){c.fn.init.call(this,e,n),this.regions={}},showIn:function(e,n){var t=this.regions[e];t&&t.hide(),n.render(this.render().find(e),t),this.regions[e]=n}});n.Layout=s,n.View=c}(window.kendo.jQuery)})}("function"==typeof define&&define.amd?define:function(e,n){return n()});
//@ sourceMappingURL=kendo.view.min.js.map