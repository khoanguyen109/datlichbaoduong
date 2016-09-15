(function () {
    toastr.options = {
        closeButton: true,
        positionClass: 'toast-top-right',
        showDuration: 1000
    };

    window.notificationInfo = function (message) {
        return notification(message, "info");
    };

    window.notificationError = function (message) {
        return notification(message, "error");
    };

    window.notificationSuccess = function (message) {
        return notification(message, "confirmation");
    };

    window.notification = function (message, notificationType) {
        var errorTimeout, timeout;
        errorTimeout = 3000;
        timeout = 2000;
        switch (notificationType != null ? notificationType.toLowerCase() : void 0) {
            case "info":
                return toastr.info(message, "Info", {
                    timeOut: timeout
                });
            case "confirmation":
                return toastr.success(message, "Success", {
                    timeOut: timeout
                });
            default:
                return toastr.error(message, "Error", {
                    timeOut: errorTimeout,
                });
        }
    };

    window.hideAllModals = function ($container) {
        return ($container != null ? $container : $('body')).find('.modal').modal('hide');
    };

    window.handleAjaxError = (function (_this) {
        return function (response) {
            switch (response.status) {
                case 400:
                    return notification('A validation error has occured, please do not submit any HTML through the browser form', 'Error');
                case 403:
                    return notification('You are not authorised to perform the selected action', 'Error');
                default:
                    return notification('An error has occurred, please try again', 'Error');
            }
        };
    })(this);

    window.alert = function (message, args) {
        var $theModal, defaultArgs, markup;
        defaultArgs = {
            title: "Oops!",
            message: message,
            okCallBack: null,
            okButtonText: "Ok"
        };
        args = args != null ? args : {};
        args = $.extend(defaultArgs, args);
        markup = "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true'>\n    <div class=\"modal-dialog\">\n        <div class=\"modal-content\">\n            <div class='modal-header'>\n                <h4>" + args.title + "</h3>\n            </div>\n            <div class='modal-body'><p>" + args.message + "</p></div>\n            <div class='modal-footer'>\n                <a href='#' class='btn btn-default ok' data-dismiss='modal'>" + args.okButtonText + "</a>'\n            </div>\n        </div>\n    </div>\n</div>";
        $theModal = $(markup);
        $("body").append($theModal);
        $theModal.modal();
        return (function ($theModal) {
            return $theModal.on('hidden.bs.modal', function () {
                if (typeof args.okCallBack === "function") {
                    args.okCallBack();
                }
                return $theModal.remove();
            });
        })($theModal);
    };

    window.confirm = function (message, args) {
        var $theModal, defaultArgs, markup, top;
        defaultArgs = {
            title: "Confirm",
            message: message,
            okCallBack: null,
            cancelCallBack: null,
            okButtonText: "Ok",
            cancelButtonText: 'Cancel',
            top: null
        };
        args = args != null ? args : {};
        args = $.extend(defaultArgs, args);
        top = args.top === null ? "" : "style='top:" + args.top + "px'";
        markup = "<div class='modal fade' tabindex='-1' role='dialog' aria-hidden='true' " + top + ">\n    <div class=\"modal-dialog\">\n        <div class=\"modal-content\">\n            <div class='modal-header'>\n                <h4 class='modal-title'>" + args.title + "</h4>\n            </div>\n            <div class='modal-body'><p>" + args.message + "</p></div>\n            <div class='modal-footer'>\n                <button href='#' class='btn btn-secondary cancel' data-dismiss='modal'>" + args.cancelButtonText + "</button>\n                <button href='#' class='btn btn-default ok' data-dismiss='modal'>" + args.okButtonText + "</button>\n            </div>\n        </div>\n    </div>\n</div>";
        $theModal = $(markup);
        $("body").append($theModal);
        $theModal.modal();
        $theModal.on('click', '.ok', function () {
            if (typeof args.okCallBack === "function") {
                args.okCallBack();
            }
            return args.cancelCallBack = null;
        });
        return $theModal.on('hidden.bs.modal', function () {
            if (typeof args.cancelCallBack === "function") {
                args.cancelCallBack();
            }
            return $theModal.remove();
        });
    };

    $(function () {
        var $body, chk, enableOnCheck, showOnCheck, _i, _j, _len, _len1, _ref, _ref1;
        $body = $('body');
        $body.on('change', 'select.auto-submit', function () {
            return $(this).closest('form').submit();
        });
        $body.on('change', 'select.js-auto-link', function () {
            return window.location = $(this).val();
        });
        $body.on('hidden.bs.modal', '.js-modal-destroy-on-close', function () {
            return $(this).removeData('bs.modal');
        });
        $body.on('click', '.js-prevent-default', function (e) {
            return e.preventDefault();
        });
        $body.on('click', 'a.submit', function (e) {
            e.preventDefault();
            return $(this).closest('form').submit();
        });
        $body.on('click', '[data-submitform]', function (e) {
            e.preventDefault();
            return $('body').find("form#" + ($(this).data('submitform'))).submit();
        });

        enableOnCheck = function (chk) {
            var $chk;
            $chk = $(chk);
            return $($chk.data('enable')).prop('disabled', !$chk.prop('checked'));
        };
        _ref = $(':checkbox[data-enable]');
        for (_i = 0, _len = _ref.length; _i < _len; _i++) {
            chk = _ref[_i];
            enableOnCheck(chk);
        }
        $body.on('click', ':checkbox[data-enable]', function () {
            return enableOnCheck(this);
        });
        showOnCheck = function (chk) {
            var $chk;
            $chk = $(chk);
            return $($chk.data('show')).toggle($chk.data('hide') ? !$chk.prop('checked') : $chk.prop('checked'));
        };
        _ref1 = $(':checkbox[data-show]');
        for (_j = 0, _len1 = _ref1.length; _j < _len1; _j++) {
            chk = _ref1[_j];
            showOnCheck(chk);
        }
        $body.on('click', ':checkbox[data-show]', function () {
            return showOnCheck(this);
        });
        $body.on('click', '[data-confirm]', function (e) {
            var $form, $this;
            e.preventDefault();
            $this = $(this);
            $form = $this.closest('form');
            return Marketplace.confirm($this.data('confirm'), {
                okCallBack: (function (_this) {
                    return function () {
                        return $form.submit();
                    };
                })(this)
            });
        });
        $(".facet-collapse").click(function () {
            $(this).toggleClass("is-expanded");
            return $(this).parent().toggleClass("active");
        });
        $(".facet .title").click(function () {
            return $(this).next(".facet-collapse").toggleClass("is-expanded");
        });
        $(".facet").not(".active").children(".facet-collapse").removeClass("is-expanded");
    });
}).call(this);

var App = function () {

    // IE mode
    var isRTL = false;
    var isIE8 = false;
    var isIE9 = false;
    var isIE10 = false;

    var responsiveHandlers = [];

    // To get the correct viewport width based on  http://andylangton.co.uk/articles/javascript/get-viewport-size-javascript/
    var _getViewPort = function () {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        return {
            width: e[a + 'Width'],
            height: e[a + 'Height']
        }
    }

    // initializes main settings
    var handleInit = function () {

        if ($('body').css('direction') === 'rtl') {
            isRTL = true;
        }

        isIE8 = !!navigator.userAgent.match(/MSIE 8.0/);
        isIE9 = !!navigator.userAgent.match(/MSIE 9.0/);
        isIE10 = !!navigator.userAgent.match(/MSIE 10.0/);

        if (isIE10) {
            jQuery('html').addClass('ie10'); // detect IE10 version
        }

        if (isIE10 || isIE9 || isIE8) {
            jQuery('html').addClass('ie'); // detect IE10 version
        }

        /*
          Virtual keyboards:
          Also, note that if you're using inputs in your modal – iOS has a rendering bug which doesn't 
          update the position of fixed elements when the virtual keyboard is triggered  
        */
        var deviceAgent = navigator.userAgent.toLowerCase();
        if (deviceAgent.match(/(iphone|ipod|ipad)/)) {
            $(document).on('focus', 'input, textarea', function () {
                $('.page-header').hide();
                $('.page-footer').hide();
            });
            $(document).on('blur', 'input, textarea', function () {
                $('.page-header').show();
                $('.page-footer').show();
            });
        }
    }

    var handleSidebarState = function () {
        // remove sidebar toggler if window width smaller than 992(for tablet and phone mode)
        var viewport = _getViewPort();
        if (viewport.width < 992) {
            $('body').removeClass("page-sidebar-closed");
        }
    }

    // runs callback functions set by App.addResponsiveHandler().
    var runResponsiveHandlers = function () {
        // reinitialize other subscribed elements
        for (var i = 0; i < responsiveHandlers.length; i++) {
            var each = responsiveHandlers[i];
            each.call();
        }
    }

    // reinitialize the laypot on window resize
    var handleResponsive = function () {
        handleSidebarState();
        //handleSidebarAndContentHeight();
        //handleFixedSidebar();
        runResponsiveHandlers();
    }

    // initialize the layout on page load
    var handleResponsiveOnInit = function () {
        handleSidebarState();
        //handleSidebarAndContentHeight();
    }

    // handle the layout reinitialization on window resize
    var handleResponsiveOnResize = function () {
        var resize;
        if (isIE8) {
            var currheight;
            $(window).resize(function () {
                if (currheight == document.documentElement.clientHeight) {
                    return; //quite event since only body resized not window.
                }
                if (resize) {
                    clearTimeout(resize);
                }
                resize = setTimeout(function () {
                    handleResponsive();
                }, 50); // wait 50ms until window resize finishes.                
                currheight = document.documentElement.clientHeight; // store last body client height
            });
        } else {
            $(window).resize(function () {
                if (resize) {
                    clearTimeout(resize);
                }
                resize = setTimeout(function () {
                    handleResponsive();
                }, 50); // wait 50ms until window resize finishes.
            });
        }
    }

    // Handle sidebar menu
    var handleSidebarMenu = function () {
        jQuery('.page-sidebar').on('click', 'li > a', function (e) {
            if ($(this).next().hasClass('sub-menu') == false) {
                if ($('.btn-navbar').hasClass('collapsed') == false) {
                    $('.btn-navbar').click();
                }
                return;
            }

            if ($(this).next().hasClass('sub-menu always-open')) {
                return;
            }

            var parent = $(this).parent().parent();
            var the = $(this);
            var menu = $('.page-sidebar-menu');
            var sub = jQuery(this).next();

            var autoScroll = false;//menu.data("auto-scroll") ? menu.data("auto-scroll") : true;
            var slideSpeed = menu.data("slide-speed") ? parseInt(menu.data("slide-speed")) : 200;

            parent.children('li.open').children('a').children('.arrow').removeClass('open');
            parent.children('li.open').children('.sub-menu:not(.always-open)').slideUp(200);
            parent.children('li.open').removeClass('open');

            var slideOffeset = -200;

            if (sub.is(":visible")) {
                jQuery('.arrow', jQuery(this)).removeClass("open");
                jQuery(this).parent().removeClass("open");
                sub.slideUp(slideSpeed, function () {
                    if (autoScroll == true && $('body').hasClass('page-sidebar-closed') == false) {
                        if ($('body').hasClass('page-sidebar-fixed')) {
                            menu.slimScroll({ 'scrollTo': (the.position()).top });
                        } else {
                            App.scrollTo(the, slideOffeset);
                        }
                    }
                    //handleSidebarAndContentHeight();
                });
            } else {
                jQuery('.arrow', jQuery(this)).addClass("open");
                jQuery(this).parent().addClass("open");
                sub.slideDown(slideSpeed, function () {
                    if (autoScroll == true && $('body').hasClass('page-sidebar-closed') == false) {
                        if ($('body').hasClass('page-sidebar-fixed')) {
                            menu.slimScroll({ 'scrollTo': (the.position()).top });
                        } else {
                            App.scrollTo(the, slideOffeset);
                        }
                    }
                    //handleSidebarAndContentHeight();
                });
            }

            e.preventDefault();
        });
    }

    // Handles the go to top button at the footer
    var handleGoTop = function () {
        /* set variables locally for increased performance */
        jQuery('.footer').on('click', '.go-top', function (e) {
            App.scrollTo();
            e.preventDefault();
        });
    }

    // Handles portlet tools & actions
    var handlePortletTools = function () {
        jQuery('body').on('click', '.portlet > .portlet-title > .tools > .collapse, .portlet .portlet-title > .tools > .expand', function (e) {
            e.preventDefault();
            var el = jQuery(this).closest(".portlet").children(".portlet-body");
            if (jQuery(this).hasClass("collapse")) {
                jQuery(this).removeClass("collapse").addClass("expand");
                el.slideUp(200);
            } else {
                jQuery(this).removeClass("expand").addClass("collapse");
                el.slideDown(200);
            }
        });
    }

    // Handles Bootstrap Modals.
    var handleModals = function () {
        // fix stackable modal issue: when 2 or more modals opened, closing one of modal will remove .modal-open class. 
        $('body').on('hide.bs.modal', function () {
            if ($('.modal:visible').size() > 1 && $('html').hasClass('modal-open') == false) {
                $('html').addClass('modal-open');
            } else if ($('.modal:visible').size() <= 1) {
                $('html').removeClass('modal-open');
            }
        });

        $('body').on('show.bs.modal', '.modal', function () {
            if ($(this).hasClass("modal-scroll")) {
                $('body').addClass("modal-open-noscroll");
            }
        });

        $('body').on('hide.bs.modal', '.modal', function () {
            $('body').removeClass("modal-open-noscroll");
        });
    }

    // Handles Bootstrap Tooltips.
    var handleTooltips = function () {
        $(".tip").tooltip({ placement: 'top', container: 'body', html: true });
        //$(".tipR").tooltip({ placement: 'right', container: 'body', html: true });
        //$(".tipB").tooltip({ placement: 'bottom', container: 'body', html: true });
        $(".tipL").tooltip({ placement: 'left', container: 'body', html: true });
    }

    var handleBootstrapSelect = function () {
        var $elements = $('.bs-select');
        $elements.selectpicker({
            iconBase: 'fa',
            tickIcon: 'fa-check',
            width: $(this).data('width')
        });

        //need to show selects and hide them with visibility otherwise they dont get posted in forms
        $elements.show();
        $elements.css({
            visibility: "hidden",
            position: "absolute"
        });
    }

    //* END:CORE HANDLERS *//

    return {

        //main function to initiate the theme
        init: function () {

            //IMPORTANT!!!: Do not modify the core handlers call order.

            //core handlers
            handleInit(); // initialize core variables
            handleResponsiveOnResize(); // set and handle responsive  
            handleResponsiveOnInit(); // handler responsive elements on page load

            //layout handlers
            //handleFixedSidebar(); // handles fixed sidebar menu
            //handleFixedSidebarHoverable(); // handles fixed sidebar on hover effect 
            handleSidebarMenu(); // handles main menu
            //handleSidebarToggler(); // handles sidebar hide/show            
            //handleFixInputPlaceholderForIE(); // fixes/enables html5 placeholder attribute for IE9, IE8
            handleGoTop(); //handles scroll to top functionality in the footer

            //ui component handlers
            handlePortletTools(); // handles portlet action bar functionality(refresh, configure, toggle, remove)
            //handleTabs(); // handle tabs
            handleTooltips(); // handle bootstrap tooltips
            handleModals(); // handle modals
            handleBootstrapSelect();
        },

        initToolTips: function () {
            handleTooltips(); // handle bootstrap tooltips
        },

        //public function to add callback a function which will be called on window resize
        addResponsiveHandler: function (func) {
            responsiveHandlers.push(func);
        },

        // wrapper function to scroll(focus) to an element
        scrollTo: function (el, offeset) {
            var pos = (el && el.size() > 0) ? el.offset().top : 0;

            if (el) {
                if ($('body').hasClass('page-header-fixed')) {
                    pos = pos - $('.header').height();
                }
                pos = pos + (offeset ? offeset : -1 * el.height());
            }

            jQuery('html,body').animate({
                scrollTop: pos
            }, 'slow');
        },

        // function to scroll to the top
        scrollTop: function () {
            App.scrollTo();
        },

        // wrapper function to  block element(indicate loading)
        blockUI: function (options) {
            var options = $.extend(true, {}, options);
            var html = '';
            if (options.iconOnly) {
                html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img style="" src="/assets/shared/img/spinner-grey.gif" align=""></div>';
            } else if (options.textOnly) {
                html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
            } else {
                html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img style="" src="/assets/shared/img/spinner-grey.gif" align=""><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
            }

            if (options.target) { // element blocking
                var el = jQuery(options.target);
                if (el.height() <= ($(window).height())) {
                    options.centerY = true;
                }
                el.block({
                    message: html,
                    baseZ: options.zIndex ? options.zIndex : 1000,
                    centerY: options.centerY != undefined ? options.centerY : false,
                    css: {
                        border: '0',
                        padding: '0',
                        backgroundColor: 'none'
                    },
                    overlayCSS: {
                        backgroundColor: options.overlayColor ? options.overlayColor : '#000',
                        opacity: 0.1,
                        cursor: 'wait'
                    }
                });
            } else { // page blocking
                $.blockUI({
                    message: html,
                    baseZ: options.zIndex ? options.zIndex : 1000,
                    css: {
                        border: '0',
                        padding: '0',
                        backgroundColor: 'none'
                    },
                    overlayCSS: {
                        backgroundColor: options.overlayColor ? options.overlayColor : '#000',
                        opacity: 0.1,
                        cursor: 'wait'
                    }
                });
            }
        },
        test : function(){
            alert(1);
        },
        // wrapper function to  un-block element(finish loading)
        unblockUI: function (target) {
            if (target) {
                jQuery(target).unblock({
                    onUnblock: function () {
                        jQuery(target).css('position', '');
                        jQuery(target).css('zoom', '');
                    }
                });
            } else {
                $.unblockUI();
            }
        },

        //public helper function to get actual input value(used in IE9 and IE8 due to placeholder attribute not supported)
        getActualVal: function (el) {
            var el = jQuery(el);
            if (el.val() === el.attr("placeholder")) {
                return "";
            }
            return el.val();
        },

        //public function to get a paremeter by name from URL
        getURLParameter: function (paramName) {
            var searchString = window.location.search.substring(1),
                i, val, params = searchString.split("&");

            for (i = 0; i < params.length; i++) {
                val = params[i].split("=");
                if (val[0] == paramName) {
                    return unescape(val[1]);
                }
            }
            return null;
        },

        // check for device touch support
        isTouchDevice: function () {
            try {
                document.createEvent("TouchEvent");
                return true;
            } catch (e) {
                return false;
            }
        },

        // check IE8 mode
        isIE8: function () {
            return isIE8;
        },

        // check IE9 mode
        isIE9: function () {
            return isIE9;
        },

        //check RTL mode
        isRTL: function () {
            return isRTL;
        }
    };

}();

