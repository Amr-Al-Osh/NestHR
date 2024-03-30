/*!
 * AdminLTE v3.2.0 (https://adminlte.io)
 * Copyright 2014-2022 Colorlib <https://colorlib.com>
 * Licensed under MIT (https://github.com/ColorlibHQ/AdminLTE/blob/master/LICENSE)
 */
(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports, require('jquery')) :
        typeof define === 'function' && define.amd ? define(['exports', 'jquery'], factory) :
            (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.adminlte = {}, global.jQuery));
})(this, (function (exports, $) {
    'use strict';

    function _interopDefaultLegacy(e) { return e && typeof e === 'object' && 'default' in e ? e : { 'default': e }; }

    var $__default = /*#__PURE__*/_interopDefaultLegacy($);


    //===========================================================
    //====================[Fullscreen]
    //===========================================================
    $("#fullscreen-toggle").click(function (event) {
        event.preventDefault();
        toggleFullScreen();
    });

    function toggleFullScreen() {
        var doc = window.document;
        var docEl = doc.documentElement;

        var requestFullScreen = docEl.requestFullscreen || docEl.mozRequestFullScreen || docEl.webkitRequestFullScreen || docEl.msRequestFullscreen;
        var cancelFullScreen = doc.exitFullscreen || doc.mozCancelFullScreen || doc.webkitExitFullscreen || doc.msExitFullscreen;

        if (!doc.fullscreenElement && !doc.mozFullScreenElement && !doc.webkitFullscreenElement && !doc.msFullscreenElement) {
            requestFullScreen.call(docEl);
        } else {
            cancelFullScreen.call(doc);
        }
    }

    //===========================================================
    //====================[PushMenu]
    //===========================================================
    var NAME = 'PushMenu';
    var DATA_KEY = 'lte.pushmenu';
    var EVENT_KEY = "." + DATA_KEY;
    var JQUERY_NO_CONFLICT = $.fn[NAME];
    var EVENT_COLLAPSED = "collapsed" + EVENT_KEY;
    var EVENT_COLLAPSED_DONE = "collapsed-done" + EVENT_KEY;
    var EVENT_SHOWN = "shown" + EVENT_KEY;
    var SELECTOR_TOGGLE_BUTTON = '[data-widget="pushmenu"]';
    var SELECTOR_BODY = 'body';
    var SELECTOR_OVERLAY = '#sidebar-overlay';
    var SELECTOR_WRAPPER = '.wrapper';
    var CLASS_NAME_COLLAPSED = 'sidebar-collapse';
    var CLASS_NAME_OPEN = 'sidebar-open';
    var CLASS_NAME_IS_OPENING = 'sidebar-is-opening';
    var CLASS_NAME_CLOSED = 'sidebar-closed';
    var Default = {
        autoCollapseSize: 992,
        enableRemember: false,
        noTransitionAfterReload: true,
        animationSpeed: 300
    };

    var PushMenu = function () {
        function PushMenu(element, options) {
            this._element = element;
            this._options = $.extend({}, Default, options);

            if ($(SELECTOR_OVERLAY).length === 0) {
                this._addOverlay();
            }

            this._init();
        }

        var _proto = PushMenu.prototype;

        _proto.expand = function expand() {
            var $bodySelector = $(SELECTOR_BODY);

            if (this._options.autoCollapseSize && $(window).width() <= this._options.autoCollapseSize) {
                $bodySelector.addClass(CLASS_NAME_OPEN);
            }

            $bodySelector.addClass(CLASS_NAME_IS_OPENING).removeClass(CLASS_NAME_COLLAPSED + " " + CLASS_NAME_CLOSED).delay(50).queue(function () {
                $bodySelector.removeClass(CLASS_NAME_IS_OPENING);
                $(this).dequeue();
            });

            if (this._options.enableRemember) {
                localStorage.setItem("remember" + EVENT_KEY, CLASS_NAME_OPEN);
            }

            $(this._element).trigger($.Event(EVENT_SHOWN));
        };

        _proto.collapse = function collapse() {
            var _this = this;

            var $bodySelector = $(SELECTOR_BODY);

            if (this._options.autoCollapseSize && $(window).width() <= this._options.autoCollapseSize) {
                $bodySelector.removeClass(CLASS_NAME_OPEN).addClass(CLASS_NAME_CLOSED);
            }

            $bodySelector.addClass(CLASS_NAME_COLLAPSED);

            if (this._options.enableRemember) {
                localStorage.setItem("remember" + EVENT_KEY, CLASS_NAME_COLLAPSED);
            }

            $(this._element).trigger($.Event(EVENT_COLLAPSED));
            setTimeout(function () {
                $(_this._element).trigger($.Event(EVENT_COLLAPSED_DONE));
            }, this._options.animationSpeed);
        };

        _proto.toggle = function toggle() {
            if ($(SELECTOR_BODY).hasClass(CLASS_NAME_COLLAPSED)) {
                this.expand();
            } else {
                this.collapse();
            }
        };

        _proto.autoCollapse = function autoCollapse(resize) {
            if (resize === void 0) {
                resize = false;
            }

            if (!this._options.autoCollapseSize) {
                return;
            }

            var $bodySelector = $(SELECTOR_BODY);

            if ($(window).width() <= this._options.autoCollapseSize) {
                if (!$bodySelector.hasClass(CLASS_NAME_OPEN)) {
                    this.collapse();
                }
            } else if (resize === true) {
                if ($bodySelector.hasClass(CLASS_NAME_OPEN)) {
                    $bodySelector.removeClass(CLASS_NAME_OPEN);
                } else if ($bodySelector.hasClass(CLASS_NAME_CLOSED)) {
                    this.expand();
                }
            }
        };

        _proto.remember = function remember() {
            if (!this._options.enableRemember) {
                return;
            }

            var $body = $('body');
            var toggleState = localStorage.getItem("remember" + EVENT_KEY);

            if (toggleState === CLASS_NAME_COLLAPSED) {
                if (this._options.noTransitionAfterReload) {
                    $body.addClass('hold-transition').addClass(CLASS_NAME_COLLAPSED).delay(50).queue(function () {
                        $(this).removeClass('hold-transition');
                        $(this).dequeue();
                    });
                } else {
                    $body.addClass(CLASS_NAME_COLLAPSED);
                }
            } else if (this._options.noTransitionAfterReload) {
                $body.addClass('hold-transition').removeClass(CLASS_NAME_COLLAPSED).delay(50).queue(function () {
                    $(this).removeClass('hold-transition');
                    $(this).dequeue();
                });
            } else {
                $body.removeClass(CLASS_NAME_COLLAPSED);
            }
        };

        _proto._init = function _init() {
            var _this2 = this;

            this.remember();
            this.autoCollapse();
            $(window).resize(function () {
                _this2.autoCollapse(true);
            });
        };

        _proto._addOverlay = function _addOverlay() {
            var _this3 = this;

            var overlay = $('<div />', {
                id: 'sidebar-overlay'
            });
            overlay.on('click', function () {
                _this3.collapse();
            });
            $(SELECTOR_WRAPPER).append(overlay);
        };

        PushMenu._jQueryInterface = function _jQueryInterface(operation) {
            return this.each(function () {
                var data = $(this).data(DATA_KEY);

                var _options = $.extend({}, Default, $(this).data());

                if (!data) {
                    data = new PushMenu(this, _options);
                    $(this).data(DATA_KEY, data);
                }

                if (typeof operation === 'string' && /collapse|expand|toggle/.test(operation)) {
                    data[operation]();
                }
            });
        };

        return PushMenu;
    }();

    $(document).on('click', SELECTOR_TOGGLE_BUTTON, function (event) {
        event.preventDefault();
        var button = event.currentTarget;

        if ($(button).data('widget') !== 'pushmenu') {
            button = $(button).closest(SELECTOR_TOGGLE_BUTTON);
        }

        PushMenu._jQueryInterface.call($(button), 'toggle');
    });

    $(window).on('load', function () {
        PushMenu._jQueryInterface.call($(SELECTOR_TOGGLE_BUTTON));
    });

    $.fn[NAME] = PushMenu._jQueryInterface;
    $.fn[NAME].Constructor = PushMenu;

    $.fn[NAME].noConflict = function () {
        $.fn[NAME] = JQUERY_NO_CONFLICT;
        return PushMenu._jQueryInterface;
    };

    //===========================================================
    //====================[SidebarSearch]
    //===========================================================
    var NAME$4 = 'SidebarSearch';
    var DATA_KEY$4 = 'lte.sidebar-search';
    var JQUERY_NO_CONFLICT$4 = $__default["default"].fn[NAME$4];
    var CLASS_NAME_OPEN$2 = 'sidebar-search-open';
    var CLASS_NAME_ICON_SEARCH = 'fa-search';
    var CLASS_NAME_ICON_CLOSE = 'fa-times';
    var CLASS_NAME_HEADER = 'nav-header';
    var CLASS_NAME_SEARCH_RESULTS = 'sidebar-search-results';
    var CLASS_NAME_LIST_GROUP = 'list-group';
    var SELECTOR_DATA_WIDGET$1 = '[data-widget="sidebar-search"]';
    var SELECTOR_SIDEBAR = '.main-sidebar .nav-sidebar';
    var SELECTOR_NAV_LINK = '.nav-link';
    var SELECTOR_NAV_TREEVIEW = '.nav-treeview';
    var SELECTOR_SEARCH_INPUT$1 = SELECTOR_DATA_WIDGET$1 + " .form-control";
    var SELECTOR_SEARCH_BUTTON = SELECTOR_DATA_WIDGET$1 + " .btn";
    var SELECTOR_SEARCH_ICON = SELECTOR_SEARCH_BUTTON + " i";
    var SELECTOR_SEARCH_LIST_GROUP = "." + CLASS_NAME_LIST_GROUP;
    var SELECTOR_SEARCH_RESULTS = "." + CLASS_NAME_SEARCH_RESULTS;
    var SELECTOR_SEARCH_RESULTS_GROUP = SELECTOR_SEARCH_RESULTS + " ." + CLASS_NAME_LIST_GROUP;
    var Default$4 = {
        arrowSign: '->',
        minLength: 3,
        maxResults: 7,
        highlightName: true,
        highlightPath: false,
        highlightClass: 'text-light',
        notFoundText: 'No element found!'
    };
    var SearchItems = [];

    var SidebarSearch = /*#__PURE__*/function () {
        function SidebarSearch(_element, _options) {
            this.element = _element;
            this.options = $__default["default"].extend({}, Default$4, _options);
            this.items = [];
        } // Public


        var _proto = SidebarSearch.prototype;

        _proto.init = function init() {
            var _this = this;

            if ($__default["default"](SELECTOR_DATA_WIDGET$1).length === 0) {
                return;
            }

            if ($__default["default"](SELECTOR_DATA_WIDGET$1).next(SELECTOR_SEARCH_RESULTS).length === 0) {
                $__default["default"](SELECTOR_DATA_WIDGET$1).after($__default["default"]('<div />', {
                    class: CLASS_NAME_SEARCH_RESULTS
                }));
            }

            if ($__default["default"](SELECTOR_SEARCH_RESULTS).children(SELECTOR_SEARCH_LIST_GROUP).length === 0) {
                $__default["default"](SELECTOR_SEARCH_RESULTS).append($__default["default"]('<div />', {
                    class: CLASS_NAME_LIST_GROUP
                }));
            }

            this._addNotFound();

            $__default["default"](SELECTOR_SIDEBAR).children().each(function (i, child) {
                _this._parseItem(child);
            });
        };

        _proto.search = function search() {
            var _this2 = this;

            var searchValue = $__default["default"](SELECTOR_SEARCH_INPUT$1).val().toLowerCase();

            if (searchValue.length < this.options.minLength) {
                $__default["default"](SELECTOR_SEARCH_RESULTS_GROUP).empty();

                this._addNotFound();

                this.close();
                return;
            }

            var searchResults = SearchItems.filter(function (item) {
                return item.name.toLowerCase().includes(searchValue);
            });
            var endResults = $__default["default"](searchResults.slice(0, this.options.maxResults));
            $__default["default"](SELECTOR_SEARCH_RESULTS_GROUP).empty();

            if (endResults.length === 0) {
                this._addNotFound();
            } else {
                endResults.each(function (i, result) {
                    $__default["default"](SELECTOR_SEARCH_RESULTS_GROUP).append(_this2._renderItem(escape(result.name), encodeURI(result.link), result.path));
                });
            }

            this.open();
        };

        _proto.open = function open() {
            $__default["default"](SELECTOR_DATA_WIDGET$1).parent().addClass(CLASS_NAME_OPEN$2);
            $__default["default"](SELECTOR_SEARCH_ICON).removeClass(CLASS_NAME_ICON_SEARCH).addClass(CLASS_NAME_ICON_CLOSE);
        };

        _proto.close = function close() {
            $__default["default"](SELECTOR_DATA_WIDGET$1).parent().removeClass(CLASS_NAME_OPEN$2);
            $__default["default"](SELECTOR_SEARCH_ICON).removeClass(CLASS_NAME_ICON_CLOSE).addClass(CLASS_NAME_ICON_SEARCH);
        };

        _proto.toggle = function toggle() {
            if ($__default["default"](SELECTOR_DATA_WIDGET$1).parent().hasClass(CLASS_NAME_OPEN$2)) {
                this.close();
            } else {
                this.open();
            }
        } // Private
            ;

        _proto._parseItem = function _parseItem(item, path) {
            var _this3 = this;

            if (path === void 0) {
                path = [];
            }

            if ($__default["default"](item).hasClass(CLASS_NAME_HEADER)) {
                return;
            }

            var itemObject = {};
            var navLink = $__default["default"](item).clone().find("> " + SELECTOR_NAV_LINK);
            var navTreeview = $__default["default"](item).clone().find("> " + SELECTOR_NAV_TREEVIEW);
            var link = navLink.attr('href');
            var name = navLink.find('p').children().remove().end().text();
            itemObject.name = this._trimText(name);
            itemObject.link = link;
            itemObject.path = path;

            if (navTreeview.length === 0) {
                SearchItems.push(itemObject);
            } else {
                var newPath = itemObject.path.concat([itemObject.name]);
                navTreeview.children().each(function (i, child) {
                    _this3._parseItem(child, newPath);
                });
            }
        };

        _proto._trimText = function _trimText(text) {
            return $.trim(text.replace(/(\r\n|\n|\r)/gm, ' '));
        };

        _proto._renderItem = function _renderItem(name, link, path) {
            var _this4 = this;

            path = path.join(" " + this.options.arrowSign + " ");
            name = unescape(name);
            link = decodeURI(link);

            if (this.options.highlightName || this.options.highlightPath) {
                var searchValue = $__default["default"](SELECTOR_SEARCH_INPUT$1).val().toLowerCase();
                var regExp = new RegExp(searchValue, 'gi');

                if (this.options.highlightName) {
                    name = name.replace(regExp, function (str) {
                        return "<strong class=\"" + _this4.options.highlightClass + "\">" + str + "</strong>";
                    });
                }

                if (this.options.highlightPath) {
                    path = path.replace(regExp, function (str) {
                        return "<strong class=\"" + _this4.options.highlightClass + "\">" + str + "</strong>";
                    });
                }
            }

            var groupItemElement = $__default["default"]('<a/>', {
                href: decodeURIComponent(link),
                class: 'list-group-item'
            });
            var searchTitleElement = $__default["default"]('<div/>', {
                class: 'search-title'
            }).html(name);
            var searchPathElement = $__default["default"]('<div/>', {
                class: 'search-path'
            }).html(path);
            groupItemElement.append(searchTitleElement).append(searchPathElement);
            return groupItemElement;
        };

        _proto._addNotFound = function _addNotFound() {
            $__default["default"](SELECTOR_SEARCH_RESULTS_GROUP).append(this._renderItem(this.options.notFoundText, '#', []));
        } // Static
            ;

        SidebarSearch._jQueryInterface = function _jQueryInterface(config) {
            var data = $__default["default"](this).data(DATA_KEY$4);

            if (!data) {
                data = $__default["default"](this).data();
            }

            var _options = $__default["default"].extend({}, Default$4, typeof config === 'object' ? config : data);

            var plugin = new SidebarSearch($__default["default"](this), _options);
            $__default["default"](this).data(DATA_KEY$4, typeof config === 'object' ? config : data);

            if (typeof config === 'string' && /init|toggle|close|open|search/.test(config)) {
                plugin[config]();
            } else {
                plugin.init();
            }
        };

        return SidebarSearch;
    }();

    $__default["default"](document).on('click', SELECTOR_SEARCH_BUTTON, function (event) {
        event.preventDefault();

        SidebarSearch._jQueryInterface.call($__default["default"](SELECTOR_DATA_WIDGET$1), 'toggle');
    });
    $__default["default"](document).on('keyup', SELECTOR_SEARCH_INPUT$1, function (event) {
        if (event.keyCode == 38) {
            event.preventDefault();
            $__default["default"](SELECTOR_SEARCH_RESULTS_GROUP).children().last().focus();
            return;
        }

        if (event.keyCode == 40) {
            event.preventDefault();
            $__default["default"](SELECTOR_SEARCH_RESULTS_GROUP).children().first().focus();
            return;
        }

        setTimeout(function () {
            SidebarSearch._jQueryInterface.call($__default["default"](SELECTOR_DATA_WIDGET$1), 'search');
        }, 100);
    });
    $__default["default"](document).on('keydown', SELECTOR_SEARCH_RESULTS_GROUP, function (event) {
        var $focused = $__default["default"](':focus');

        if (event.keyCode == 38) {
            event.preventDefault();

            if ($focused.is(':first-child')) {
                $focused.siblings().last().focus();
            } else {
                $focused.prev().focus();
            }
        }

        if (event.keyCode == 40) {
            event.preventDefault();

            if ($focused.is(':last-child')) {
                $focused.siblings().first().focus();
            } else {
                $focused.next().focus();
            }
        }
    });
    $__default["default"](window).on('load', function () {
        SidebarSearch._jQueryInterface.call($__default["default"](SELECTOR_DATA_WIDGET$1), 'init');
    });
    /**
     * jQuery API
     * ====================================================
     */

    $__default["default"].fn[NAME$4] = SidebarSearch._jQueryInterface;
    $__default["default"].fn[NAME$4].Constructor = SidebarSearch;

    $__default["default"].fn[NAME$4].noConflict = function () {
        $__default["default"].fn[NAME$4] = JQUERY_NO_CONFLICT$4;
        return SidebarSearch._jQueryInterface;
    };


    //===========================================================
    //====================[Treeview SaidBar]
    //===========================================================
    var NAME = 'Treeview';
    var DATA_KEY = 'lte.treeview';
    var EVENT_KEY = "." + DATA_KEY;
    var JQUERY_NO_CONFLICT = $__default["default"].fn[NAME];
    var EVENT_EXPANDED = "expanded" + EVENT_KEY;
    var EVENT_COLLAPSED = "collapsed" + EVENT_KEY;
    var EVENT_LOAD_DATA_API = "load" + EVENT_KEY;
    var SELECTOR_LI = '.nav-item';
    var SELECTOR_LINK = '.nav-link';
    var SELECTOR_TREEVIEW_MENU = '.nav-treeview';
    var SELECTOR_OPEN = '.menu-open';
    var SELECTOR_DATA_WIDGET = '[data-widget="treeview"]';
    var CLASS_NAME_OPEN = 'menu-open';
    var CLASS_NAME_IS_OPENING = 'menu-is-opening';
    var CLASS_NAME_SIDEBAR_COLLAPSED = 'sidebar-collapse';
    var Default = {
        trigger: SELECTOR_DATA_WIDGET + " " + SELECTOR_LINK,
        animationSpeed: 300,
        accordion: true,
        expandSidebar: false,
        sidebarButtonSelector: '[data-widget="pushmenu"]'
    };

    var Treeview = /*#__PURE__*/function () {
        function Treeview(element, config) {
            this._config = config;
            this._element = element;
        } // Public


        var _proto = Treeview.prototype;

        _proto.init = function init() {
            $__default["default"]("" + SELECTOR_LI + SELECTOR_OPEN + " " + SELECTOR_TREEVIEW_MENU + SELECTOR_OPEN).css('display', 'block');

            this._setupListeners();
        };

        _proto.expand = function expand(treeviewMenu, parentLi) {
            var _this = this;

            var expandedEvent = $__default["default"].Event(EVENT_EXPANDED);

            if (this._config.accordion) {
                var openMenuLi = parentLi.siblings(SELECTOR_OPEN).first();
                var openTreeview = openMenuLi.find(SELECTOR_TREEVIEW_MENU).first();
                this.collapse(openTreeview, openMenuLi);
            }

            parentLi.addClass(CLASS_NAME_IS_OPENING);
            treeviewMenu.stop().slideDown(this._config.animationSpeed, function () {
                parentLi.addClass(CLASS_NAME_OPEN);
                $__default["default"](_this._element).trigger(expandedEvent);
            });

            if (this._config.expandSidebar) {
                this._expandSidebar();
            }
        };

        _proto.collapse = function collapse(treeviewMenu, parentLi) {
            var _this2 = this;

            var collapsedEvent = $__default["default"].Event(EVENT_COLLAPSED);
            parentLi.removeClass(CLASS_NAME_IS_OPENING + " " + CLASS_NAME_OPEN);
            treeviewMenu.stop().slideUp(this._config.animationSpeed, function () {
                $__default["default"](_this2._element).trigger(collapsedEvent);
                treeviewMenu.find(SELECTOR_OPEN + " > " + SELECTOR_TREEVIEW_MENU).slideUp();
                treeviewMenu.find(SELECTOR_OPEN).removeClass(CLASS_NAME_IS_OPENING + " " + CLASS_NAME_OPEN);
            });
        };

        _proto.toggle = function toggle(event) {
            var $relativeTarget = $__default["default"](event.currentTarget);
            var $parent = $relativeTarget.parent();
            var treeviewMenu = $parent.find("> " + SELECTOR_TREEVIEW_MENU);

            if (!treeviewMenu.is(SELECTOR_TREEVIEW_MENU)) {
                if (!$parent.is(SELECTOR_LI)) {
                    treeviewMenu = $parent.parent().find("> " + SELECTOR_TREEVIEW_MENU);
                }

                if (!treeviewMenu.is(SELECTOR_TREEVIEW_MENU)) {
                    return;
                }
            }

            event.preventDefault();
            var parentLi = $relativeTarget.parents(SELECTOR_LI).first();
            var isOpen = parentLi.hasClass(CLASS_NAME_OPEN);

            if (isOpen) {
                this.collapse($__default["default"](treeviewMenu), parentLi);
            } else {
                this.expand($__default["default"](treeviewMenu), parentLi);
            }
        } // Private
            ;

        _proto._setupListeners = function _setupListeners() {
            var _this3 = this;

            var elementId = this._element.attr('id') !== undefined ? "#" + this._element.attr('id') : '';
            $__default["default"](document).on('click', "" + elementId + this._config.trigger, function (event) {
                _this3.toggle(event);
            });
        };

        _proto._expandSidebar = function _expandSidebar() {
            if ($__default["default"]('body').hasClass(CLASS_NAME_SIDEBAR_COLLAPSED)) {
                $__default["default"](this._config.sidebarButtonSelector).PushMenu('expand');
            }
        } // Static
            ;

        Treeview._jQueryInterface = function _jQueryInterface(config) {
            return this.each(function () {
                var data = $__default["default"](this).data(DATA_KEY);

                var _options = $__default["default"].extend({}, Default, $__default["default"](this).data());

                if (!data) {
                    data = new Treeview($__default["default"](this), _options);
                    $__default["default"](this).data(DATA_KEY, data);
                }

                if (config === 'init') {
                    data[config]();
                }
            });
        };

        return Treeview;
    }();
    /**
     * Data API
     * ====================================================
     */


    $__default["default"](window).on(EVENT_LOAD_DATA_API, function () {
        $__default["default"](SELECTOR_DATA_WIDGET).each(function () {
            Treeview._jQueryInterface.call($__default["default"](this), 'init');
        });
    });
    /**
     * jQuery API
     * ====================================================
     */

    $__default["default"].fn[NAME] = Treeview._jQueryInterface;
    $__default["default"].fn[NAME].Constructor = Treeview;

    $__default["default"].fn[NAME].noConflict = function () {
        $__default["default"].fn[NAME] = JQUERY_NO_CONFLICT;
        return Treeview._jQueryInterface;
    };

    exports.PushMenu = PushMenu;
    exports.SidebarSearch = SidebarSearch;
    exports.Treeview = Treeview;

    Object.defineProperty(exports, '__esModule', { value: true });

}));



