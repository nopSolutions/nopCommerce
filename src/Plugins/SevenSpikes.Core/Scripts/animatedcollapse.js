//** Animated Collapsible DIV v2.0- (c) Dynamic Drive DHTML code library: http://www.dynamicdrive.com.
//** May 24th, 08'- Script rewritten and updated to 2.0.
//** June 4th, 08'- Version 2.01: Bug fix to work with jquery 1.2.6 (which changed the way attr() behaves).
//** March 5th, 09'- Version 2.2, which adds the following:
//1) ontoggle($, divobj, state) event that fires each time a DIV is expanded/collapsed, including when the page 1st loads
//2) Ability to expand a DIV via a URL parameter string, ie: index.htm?expanddiv=jason or index.htm?expanddiv=jason,kelly

//** March 9th, 09'- Version 2.2.1: Optimized ontoggle event handler slightly.
//** July 3rd, 09'- Version 2.4, which adds the following:
//1) You can now insert rel="expand[divid] | collapse[divid] | toggle[divid]" inside arbitrary links to act as DIV togglers
//2) For image toggler links, you can insert the attributes "data-openimage" and "data-closedimage" to update its image based on the DIV state


/***********************************************
* Animated Collapsible DIV v2.4- (c) Dynamic Drive DHTML code library (www.dynamicdrive.com)
* This notice MUST stay intact for legal use
* Visit Dynamic Drive at http://www.dynamicdrive.com/ for this script and 100s more
***********************************************/

var animatedcollapse = {
    divholders: {}, //structure: {div.id, div.attrs, div.$divref, div.$togglerimage}
    divgroups: {}, //structure: {groupname.count, groupname.lastactivedivid}
    lastactiveingroup: {}, //structure: {lastactivediv.id}
    preloadimages: [],

    show: function (divids) { //public method
        if (typeof divids == "object") {
            for (var i = 0; i < divids.length; i++)
                this.showhide(divids[i], "show");
        }
        else
            this.showhide(divids, "show");
    },

    hide: function (divids) { //public method
        if (typeof divids == "object") {
            for (var i = 0; i < divids.length; i++)
                this.showhide(divids[i], "hide");
        }
        else
            this.showhide(divids, "hide");
    },

    toggle: function (divid) { //public method
        if (typeof divid == "object")
            divid = divid[0];
        this.showhide(divid, "toggle");
    },

    addDiv: function (divid, attrstring) { //public function
        this.divholders[divid] = ({ id: divid, $divref: null, attrs: attrstring });
        this.divholders[divid].getAttr = function (name) { //assign getAttr() function to each divholder object
            var attr = new RegExp(name + "=([^,]+)", "i"); //get name/value config pair (ie: width=400px,)
            return (attr.test(this.attrs) && parseInt(RegExp.$1) != 0) ? RegExp.$1 : null; //return value portion (string), or 0 (false) if none found
        };
        this.currentid = divid; //keep track of current div object being manipulated (in the event of chaining)
        return this;
    },

    showhide: function (divid, action) {
        var $divref = this.divholders[divid].$divref; //reference collapsible DIV
        if (this.divholders[divid] && $divref.length == 1) { //if DIV exists
            var targetgroup = this.divgroups[$divref.attr('groupname')]; //find out which group DIV belongs to (if any)
            if ($divref.attr('groupname') && targetgroup.count > 1 && (action == "show" || action == "toggle" && $divref.css('display') == 'none')) { //If current DIV belongs to a group
                if (targetgroup.lastactivedivid && targetgroup.lastactivedivid != divid) //if last active DIV is set
                    this.slideengine(targetgroup.lastactivedivid, 'hide'); //hide last active DIV within group first
                this.slideengine(divid, 'show');
                targetgroup.lastactivedivid = divid; //remember last active DIV
            }
            else {
                this.slideengine(divid, action);
            }
        }
    },

    slideengine: function (divid, action) {
        var $divref = this.divholders[divid].$divref;
        var $togglerimage = this.divholders[divid].$togglerimage;
        if (this.divholders[divid] && $divref.length == 1) { //if this DIV exists
            var animateSetting = { height: action };
            if ($divref.attr('fade'))
                animateSetting.opacity = action;
            $divref.animate(animateSetting, $divref.attr('speed') ? parseInt($divref.attr('speed')) : 500, function () {
                if ($togglerimage) {
                    $togglerimage.attr('src', ($divref.css('display') == "none") ? $togglerimage.data('srcs').closed : $togglerimage.data('srcs').open);
                }
                if (animatedcollapse.ontoggle) {
                    try {
                        animatedcollapse.ontoggle(jQuery, $divref.get(0), $divref.css('display'));
                    }
                    catch (e) {
                        alert("An error exists inside your \"ontoggle\" function:\n\n" + e + "\n\nAborting execution of function.");
                    }
                }
            });
            return false;
        }
    },

    generatemap: function () {
        var map = {};
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i][1] != null) { //do not generate name/value pair if value is null
                map[arguments[i][0]] = arguments[i][1];
            }
        }
        return map;
    },

    init: function () {
        var ac = this;
        jQuery(document).ready(function ($) {
            animatedcollapse.ontoggle = animatedcollapse.ontoggle || null;
            var urlparamopenids = animatedcollapse.urlparamselect(); //Get div ids that should be expanded based on the url (['div1','div2',etc])
            var persistopenids = ac.getCookie('acopendivids'); //Get list of div ids that should be expanded due to persistence ('div1,div2,etc')
            var groupswithpersist = ac.getCookie('acgroupswithpersist'); //Get list of group names that have 1 or more divs with "persist" attribute defined
            if (persistopenids != null) //if cookie isn't null (is null if first time page loads, and cookie hasnt been set yet)
                persistopenids = (persistopenids == 'nada') ? [] : persistopenids.split(','); //if no divs are persisted, set to empty array, else, array of div ids
            groupswithpersist = (groupswithpersist == null || groupswithpersist == 'nada') ? [] : groupswithpersist.split(','); //Get list of groups with divs that are persisted
            jQuery.each(ac.divholders, function () { //loop through each collapsible DIV object
                this.$divref = $('#' + this.id);
                if ((this.getAttr('persist') || jQuery.inArray(this.getAttr('group'), groupswithpersist) != -1) && persistopenids != null) { //if this div carries a user "persist" setting, or belong to a group with at least one div that does
                    var cssdisplay = (jQuery.inArray(this.id, persistopenids) != -1) ? 'block' : 'none';
                }
                else {
                    var cssdisplay = this.getAttr('hide') ? 'none' : null;
                }
                if (urlparamopenids[0] == "all" || jQuery.inArray(this.id, urlparamopenids) != -1) { //if url parameter string contains the single array element "all", or this div's ID
                    cssdisplay = 'block'; //set div to "block", overriding any other setting
                }
                else if (urlparamopenids[0] == "none") {
                    cssdisplay = 'none'; //set div to "none", overriding any other setting
                }
                this.$divref.css(ac.generatemap(['height', this.getAttr('height')], ['display', cssdisplay]));
                this.$divref.attr(ac.generatemap(['groupname', this.getAttr('group')], ['fade', this.getAttr('fade')], ['speed', this.getAttr('speed')]));
                if (this.getAttr('group')) { //if this DIV has the "group" attr defined
                    var targetgroup = ac.divgroups[this.getAttr('group')] || (ac.divgroups[this.getAttr('group')] = {}); //Get settings for this group, or if it no settings exist yet, create blank object to store them in
                    targetgroup.count = (targetgroup.count || 0) + 1; //count # of DIVs within this group
                    if (jQuery.inArray(this.id, urlparamopenids) != -1) { //if url parameter string contains this div's ID
                        targetgroup.lastactivedivid = this.id; //remember this DIV as the last "active" DIV (this DIV will be expanded). Overrides other settings
                        targetgroup.overridepersist = 1; //Indicate to override persisted div that would have been expanded
                    }
                    if (!targetgroup.lastactivedivid && this.$divref.css('display') != 'none' || cssdisplay == "block" && typeof targetgroup.overridepersist == "undefined") //if this DIV was open by default or should be open due to persistence								
                        targetgroup.lastactivedivid = this.id; //remember this DIV as the last "active" DIV (this DIV will be expanded)
                    this.$divref.css({ display: 'none' }); //hide any DIV that's part of said group for now
                }
            }); //end divholders.each
            jQuery.each(ac.divgroups, function () { //loop through each group
                if (this.lastactivedivid && urlparamopenids[0] != "none") //show last "active" DIV within each group (one that should be expanded), unless url param="none"
                    ac.divholders[this.lastactivedivid].$divref.show();
            });
            if (animatedcollapse.ontoggle) {
                jQuery.each(ac.divholders, function () { //loop through each collapsible DIV object and fire ontoggle event
                    animatedcollapse.ontoggle(jQuery, this.$divref.get(0), this.$divref.css('display'));
                });
            }
            //Parse page for links containing rel attribute
            var $allcontrols = $('a[rel]').filter('[rel^="collapse["], [rel^="expand["], [rel^="toggle["]'); //get all elements on page with rel="collapse[]", "expand[]" and "toggle[]"
            $allcontrols.each(function () { //loop though each control link
                this._divids = this.getAttribute('rel').replace(/(^\w+)|(\s+)/g, "").replace(/[\[\]']/g, ""); //cache value 'div1,div2,etc' within identifier[div1,div2,etc]
                if (this.getElementsByTagName('img').length == 1 && ac.divholders[this._divids]) { //if control is an image link that toggles a single DIV (must be one to one to update status image)
                    animatedcollapse.preloadimage(this.getAttribute('data-openimage'), this.getAttribute('data-closedimage')); //preload control images (if defined)
                    $togglerimage = $(this).find('img').eq(0).data('srcs', { open: this.getAttribute('data-openimage'), closed: this.getAttribute('data-closedimage') }); //remember open and closed images' paths
                    ac.divholders[this._divids].$togglerimage = $(this).find('img').eq(0); //save reference to toggler image (to be updated inside slideengine()
                    ac.divholders[this._divids].$togglerimage.attr('src', (ac.divholders[this._divids].$divref.css('display') == "none") ? $togglerimage.data('srcs').closed : $togglerimage.data('srcs').open);
                }
                $(this).click(function () { //assign click behavior to each control link
                    var relattr = this.getAttribute('rel');
                    var divids = (this._divids == "") ? [] : this._divids.split(','); //convert 'div1,div2,etc' to array 
                    if (divids.length > 0) {
                        animatedcollapse[/expand/i.test(relattr) ? 'show' : /collapse/i.test(relattr) ? 'hide' : 'toggle'](divids); //call corresponding public function
                        return false;
                    }
                }); //end control.click
            }); // end control.each

            $(window).bind('unload', function () {
                ac.uninit();
            });
        }); //end doc.ready()
    },

    uninit: function () {
        var opendivids = '', groupswithpersist = '';
        jQuery.each(this.divholders, function () {
            if (this.$divref.css('display') != 'none') {
                opendivids += this.id + ','; //store ids of DIVs that are expanded when page unloads: 'div1,div2,etc'
            }
            if (this.getAttr('group') && this.getAttr('persist'))
                groupswithpersist += this.getAttr('group') + ','; //store groups with which at least one DIV has persistance enabled: 'group1,group2,etc'
        });
        opendivids = (opendivids == '') ? 'nada' : opendivids.replace(/,$/, '');
        groupswithpersist = (groupswithpersist == '') ? 'nada' : groupswithpersist.replace(/,$/, '');
        this.setCookie('acopendivids', opendivids);
        this.setCookie('acgroupswithpersist', groupswithpersist);
    },

    getCookie: function (Name) {
        var re = new RegExp(Name + "=[^;]*", "i"); //construct RE to search for target name/value pair
        if (document.cookie.match(re)) //if cookie found
            return document.cookie.match(re)[0].split("=")[1]; //return its value
        return null;
    },

    setCookie: function (name, value, days) {
        if (typeof days != "undefined") { //if set persistent cookie
            var expireDate = new Date();
            expireDate.setDate(expireDate.getDate() + days);
            document.cookie = name + "=" + value + "; path=/; expires=" + expireDate.toGMTString();
        }
        else //else if this is a session only cookie
            document.cookie = name + "=" + value + "; path=/";
    },

    urlparamselect: function () {
        window.location.search.match(/expanddiv=([\w\-_,]+)/i); //search for expanddiv=divid or divid1,divid2,etc
        return (RegExp.$1 != "") ? RegExp.$1.split(",") : [];
    },

    preloadimage: function () {
        var preloadimages = this.preloadimages;
        for (var i = 0; i < arguments.length; i++) {
            if (arguments[i] && arguments[i].length > 0) {
                preloadimages[preloadimages.length] = new Image();
                preloadimages[preloadimages.length - 1].src = arguments[i];
            }
        }
    }

}