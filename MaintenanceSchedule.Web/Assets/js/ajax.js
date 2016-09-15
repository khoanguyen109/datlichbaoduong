(function () {
    $(function () {
        var $body = $('body');
        var ajaxError, ajaxSuccess, submitClicked;
        submitClicked = null;
        $body.on('click', 'form :submit', function () {
            return submitClicked = this;
        });
        $body.on('submit', 'form.ajax-container, .ajax-container form', function (event) {
            var $form, $submitClicked, $submitShim, blockUI;
            event.preventDefault();
            $form = $(this);
            blockUI = $form.hasClass('js-block-ui');
            if (submitClicked != null) {
                $submitClicked = $(submitClicked);
                $submitClicked.prop('disabled', true);
                $submitClicked.data('normal-text', $submitClicked.text());
                if ($submitClicked.data["loading-text"] !== 'undefined')
                    $submitClicked.text($submitClicked.data["loading-text"]);
            }
            (function (submitClicked) {
                return $form.ajaxSubmit({
                    beforeSubmit: function (e, form) {
                        if (blockUI) {
                            App.blockUI({
                                boxed: true,
                                message: "Please wait..."
                            });
                        }
                        return $form.trigger('beforesubmit', e);
                    },
                    success: function (response) {
                        if (blockUI) {
                            App.unblockUI();
                        }
                        if ($submitClicked != null) {
                            $(submitClicked).text($(submitClicked).data('normal-text'));
                            $submitClicked.prop('disabled', false);
                        }
                        Ajax.ajaxSuccess(response, $form);
                        if (response.success)
                            $form[0].reset();
                    },
                    error: function (response) {
                        if (blockUI) {
                            App.unblockUI();
                        }
                        if ($submitClicked != null) {
                            $(submitClicked).text($(submitClicked).data('normal-text'));
                            $submitClicked.prop('disabled', false);
                        }
                        Ajax.ajaxError(response, $form);
                    }
                });
            })(submitClicked);
            return submitClicked = null;
        });

    });
}).call(this);

///declare an Ajax object to handle binding event to a specified link, form dynamically.
///for example: you have a request to server and recevie a json object. the json object will be parsed into html elements (such as <a>)
///but, at this time, the $document ready has passed over, so that those html elements won't have handlers for click event when user clicks <a>

var Ajax = function () {
    var _ajaxError = function (response, $target) {
        if (typeof window.handleAjaxError === "function")
            return window.handleAjaxError(response);
        return $target.trigger('ajaxError', [response]);
    };

    var _ajaxSuccess = function (response, $target) {
        var $ajaxContainer, $html, e;
        if (response.success) {
            if (typeof window.hideAllModals === "function") {
                window.hideAllModals();
            }
            if ((response.message != null) && response.message.length > 0) {
                if (typeof window.notificationInfo === "function")
                    window.notificationSuccess(response.message);
            }
        } else {
            if ((response.message != null) && response.message.length > 0) {
                if (typeof window.notificationError === "function")
                    window.notificationError(response.message);
            }
        }
        return $target.trigger('ajaxSuccess', [response]);
    }

    return {
        handleAjaxLink: function ($container, ids, confirmMessage) {
            $container.find(ids).on("click", function (event) {
                event.preventDefault();
                var $a = $(this);
                var args = {
                    okCallBack: function () {
                        return $.ajax({
                            url: $a.attr("href"),
                            type: 'post',
                            success: function (response) {
                                return _ajaxSuccess(response, $a);
                            },
                            error : _ajaxError
                        });
                    }
                };
                confirm(confirmMessage, args);
            });
        },

        ajaxSuccess: function (response, $target) {
            return _ajaxSuccess(response, $target);
        },

        ajaxError: function (response, $target) {
            return _ajaxError(response, $target);
        }
    }
}();