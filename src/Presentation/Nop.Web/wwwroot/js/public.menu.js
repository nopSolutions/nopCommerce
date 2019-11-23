var mainMenu = {
    rootRoute: '',
    subCatRoute: '',
    topMenuRootSelector: '',
    mobileMenuRootSelector: '',
    localized_data: false,

    init: function (rootRoute, subCatRoute, topMenuRootSelector, mobileMenuRootSelector, localized_data) {
        this.rootRoute = rootRoute;
        this.subCatRoute = subCatRoute;
        this.localized_data = localized_data;
        this.topMenuRootSelector = $(topMenuRootSelector);
        this.mobileMenuRootSelector = $(mobileMenuRootSelector);

        this.topMenuLineAttr = 'topMenuId';
        this.mobileMenuLineAttr = 'mobileMenuId';

        this.getRoot();
    },

    getRoot: function () {
        let self = this;
        $.ajax({
            cache: false,
            url: this.rootRoute,
            type: "POST",
            success: function (data, textStatus, jqXHR) {
                let lis = self.categoryList(data, true);
                $(self.topMenuRootSelector).append(lis);

                lis = self.categoryList(data, false);
                $(self.mobileMenuRootSelector).append(lis);
            },
            error: this.ajaxFailure
        });
    },

    getSubRoot: function (id, isTopMenu) {
        let self = this;
        if (isTopMenu) {
            return self.getTopMenuSubRoot(id);
        }
        else {
            return self.getMobileMenuSubRoot(id);
        }
    },

    getTopMenuSubRoot: function (id) {
        let selfTop = this;

        let catSel = 'li[' + this.topMenuLineAttr + ' = ' + id + ']';
        if ($(catSel).hasClass("loaded")) { return; }

        $.ajax({
            cache: false,
            data: {
                "id": id
            },
            url: this.subCatRoute,
            type: "POST",
            success: function (data, textStatus, jqXHR) {
                let listItems = selfTop.categoryList(data, true);
                if (listItems.length === 0) { 
                  $(catSel).addClass("loaded"); return;
                }
                $('<ul/>', { class: 'sublist' }).appendTo($(catSel));
                $(catSel).addClass("loaded");
                $(catSel + ' > ul').append(listItems);
            },
            error: this.ajaxFailure
        });
    },

    getMobileMenuSubRoot: function (id) {
        let selfMobile = this;

        let catSel = 'li[' + this.mobileMenuLineAttr + ' = ' + id + ']';
        if ($(catSel).hasClass("loaded")) { return; }

        $.ajax({
            cache: false,
            data: {
                "id": id
            },
            url: this.subCatRoute,
            type: "POST",
            success: function (data, textStatus, jqXHR) {
                let listItems = selfMobile.categoryList(data, false);
                let ul = $('<ul/>', { 'class': 'sublist' });

                $(catSel).addClass("loaded");
                $(catSel).append(ul);
                $(catSel + ' > ul').append(listItems);

                $('.top-menu.mobile .sublist-toggle').unbind().on('click', function () {
                    $(this).siblings('.sublist').slideToggle('slow');
                });
                $(catSel + ' > ul').slideToggle('slow');
            },
            error: this.ajaxFailure
        });
    },

    categoryList: function (data, isTopMenu) {
        listItems = [];

        let self = this;

        for (i = 0; i < data.length; i++) {
            if (!data[i].IncludeInTopMenu) { continue; }
            listItems.push(self.categoryLine(data[i], isTopMenu));
        }
        return listItems;
    },

    categoryLine: function (data, isTopMenu) {
        let self = this;

        if (isTopMenu) {
            return self.topMenuCategoryLine(data);
        }
        else {
            return self.mobileMenuCategoryLine(data);
        }
    },

    topMenuCategoryLine: function (data) {
        let selfTop = this;

        let li = $('<li/>');

        $('<a/>', { href: data.Route, text: data.Name }).appendTo(li);

        if (data.HaveSubCategories) {
            $('<div/>', { class: 'sublist-toggle' }).appendTo(li);
            li.on('mouseenter', function () {
                $(this).addClass("inside");
                selfTop.getSubRoot(data.Id, true);
            });
        }

        li.attr(this.topMenuLineAttr, data.Id);

        return li;
    },

    mobileMenuCategoryLine: function (data) {
        let selfMobile = this;

        let li = $('<li/>');

        $('<a/>', { href: data.Route, text: data.Name }).appendTo(li);

        if (data.HaveSubCategories) {
            let div = $('<div/>', { class: 'sublist-toggle' }).appendTo(li);
            div.get(0).addEventListener('click', function () { selfMobile.getSubRoot(data.Id, false); });
        }

        li.attr(this.mobileMenuLineAttr, data.Id);

        return li;
    },

    ajaxFailure: function () {
        alert(this.localized_data.AjaxFailure);
    }
};