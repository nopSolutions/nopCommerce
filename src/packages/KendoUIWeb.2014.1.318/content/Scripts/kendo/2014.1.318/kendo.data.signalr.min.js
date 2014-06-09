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
!function(e,define){define(["./kendo.data.min"],e)}(function(){return function(){kendo.data.transports.signalr=kendo.data.RemoteTransport.extend({init:function(e){var t,n=e&&e.signalr?e.signalr:{},i=n.promise;if(!i)throw Error('The "promise" option must be set.');if("function"!=typeof i.done||"function"!=typeof i.fail)throw Error('The "promise" option must be a Promise.');if(this.promise=i,t=n.hub,!t)throw Error('The "hub" option must be set.');if("function"!=typeof t.on||"function"!=typeof t.invoke)throw Error('The "hub" option is not a valid SignalR hub proxy.');this.hub=t,kendo.data.RemoteTransport.fn.init.call(this,e)},push:function(e){var t=this.options.signalr.client||{};t.create&&this.hub.on(t.create,e.pushCreate),t.update&&this.hub.on(t.update,e.pushUpdate),t.destroy&&this.hub.on(t.destroy,e.pushDestroy)},_crud:function(e,t){var n,i,r=this.hub,o=this.options.signalr.server;if(!o||!o[t])throw Error(kendo.format('The "server.{0}" option must be set.',t));n=[o[t]],i=this.parameterMap(e.data,t),$.isEmptyObject(i)||n.push(i),this.promise.done(function(){r.invoke.apply(r,n).done(e.success).fail(e.error)})},read:function(e){this._crud(e,"read")},create:function(e){this._crud(e,"create")},update:function(e){this._crud(e,"update")},destroy:function(e){this._crud(e,"destroy")}})}(),window.kendo},"function"==typeof define&&define.amd?define:function(e,t){t()});