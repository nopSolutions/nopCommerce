/**
 * Justboil.me - a TinyMCE image upload plugin
 * jbimages/plugin.js
 *
 * Released under Creative Commons Attribution 3.0 Unported License
 *
 * License: http://creativecommons.org/licenses/by/3.0/
 * Plugin info: http://justboil.me/
 * Author: Viktor Kuzhelnyi
 *
 * Version: 2.3 released 23/06/2013
 */
 
(function() {
	// Load plugin specific language pack
	tinymce.PluginManager.requireLangPack('jbimages');

	tinymce.create('tinymce.plugins.jbImagesPlugin', {
		/**
		 * Initializes the plugin, this will be executed after the plugin has been created.
		 * This call is done before the editor instance has finished it's initialization so use the onInit event
		 * of the editor instance to intercept that event.
		 *
		 * @param {tinymce.Editor} ed Editor instance that the plugin is initialized in.
		 * @param {string} url Absolute URL to where the plugin is located.
		 */
		init : function(ed, url) {
			// Register the command so that it can be invoked by using tinyMCE.activeEditor.execCommand('mceExample');
			ed.addCommand('jbImages', function() {
				var unixtime_ms = new Date().getTime();
				ed.windowManager.open({
					file : url + '/dialog.htm?z' + unixtime_ms,
					width : 330 + parseInt(ed.getLang('jbimages.delta_width', 0)),
					height : 155 + parseInt(ed.getLang('jbimages.delta_height', 0)),
					inline : 1
				}, {
					plugin_url : url // Plugin absolute URL
				});
			});

			// Register example button
			ed.addButton('jbimages', {
				title : 'jbimages.desc',
				cmd : 'jbImages',
				image : url + '/img/jbimages-bw.gif'
			});

			// Add a node change handler, selects the button in the UI when a image is selected
			ed.onNodeChange.add(function(ed, cm, n) {
				cm.setActive('jbimages', n.nodeName == 'IMG');
			});
		},

		/**
		 * Creates control instances based in the incomming name. This method is normally not
		 * needed since the addButton method of the tinymce.Editor class is a more easy way of adding buttons
		 * but you sometimes need to create more complex controls like listboxes, split buttons etc then this
		 * method can be used to create those.
		 *
		 * @param {String} n Name of the control to create.
		 * @param {tinymce.ControlManager} cm Control manager to use inorder to create new control.
		 * @return {tinymce.ui.Control} New control instance or null if no control was created.
		 */
		createControl : function(n, cm) {
			return null;
		},

		/**
		 * Returns information about the plugin as a name/value array.
		 * The current keys are longname, author, authorurl, infourl and version.
		 *
		 * @return {Object} Name/value array containing information about the plugin.
		 */
		getInfo : function() {
			return {
				longname : 'JustBoil.me Images Plugin',
				author : 'Viktor Kuzhelnyi',
				authorurl : 'http://justboil.me/',
				infourl : 'http://justboil.me/',
				version : "2.3"
			};
		}
	});

	// Register plugin
	tinymce.PluginManager.add('jbimages', tinymce.plugins.jbImagesPlugin);
})();
