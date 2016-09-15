(function ($) {
    $.fn.textfill = function (options) {
        var fontSize = options.maxFontPixels;
        var ourText = $(options.element + ':visible:first', this);
        var maxHeight = $(this).height();
        var maxWidth = $(this).width();
        var textHeight;
        var textWidth;
        do {
            ourText.css('font-size', fontSize);
            textHeight = ourText.height();
            textWidth = ourText.width();
            fontSize = fontSize - 1;
        } while ((textHeight > maxHeight || textWidth > maxWidth) && fontSize > 3);
        return this;
    }
})(jQuery);

$(document).ready(function () {
    $("#rch-select-sign-in").on("click", function () {
        if ($("#rch-sign-in").is(":visible"))
            $("#rch-sign-in").hide();
        else
            $("#rch-sign-in").show();
    });

    $('.rch-toggle').click(function () {
        // Toggle buttons when burger menu clicked
        if ($('.rch-navigation').css('display') == 'none') {
            $('.rch-navigation').css('display', 'table-cell');
            $('.rch-welcome-message').hide();
        } else {
            $('.rch-navigation').hide();
            $('.rch-navigation-button').removeClass("active");
        }
    });

    // Clicking away closes dropdowns
    $('html').click(function () {
        $('.rch-drop-down').hide();
        $('.rch-navigation-button').removeClass("active");
    });

    // Tapping away closes dropdowns
    $('html').bind('touchend', function () {
        $('.rch-drop-down').hide();
        $('.rch-navigation-button').removeClass("active");
    });

    // Close all dropdowns and open specific one on click
    $('.rch-navigation-button').click(function (event) {

        if ($(this).hasClass("active")) {
            // Set all tab buttons and dropdowns to default
            $('.rch-drop-down').hide();
            $('.rch-navigation-button').removeClass("active");
        } else {
            // Set all tab buttons and dropdowns to default
            $('.rch-drop-down').hide();
            $('.rch-navigation-button').removeClass("active");

            // Apply clicked style and show dropdown
            $(this).children(".rch-drop-down").show();
            $(this).addClass("active");
        }

        // Apply clicked style and show dropdown (GlobalNav)
        if ($('#rch-select-global-nav').hasClass("active")) {
            $("#rch-global-navigation").show();
        }
        event.stopPropagation();
    });


    $('.rch-drop-down').click(function (event) {
        event.stopPropagation();
    });
    $('.rch-drop-down, .rch-navigation-button').bind('touchend', function (event) {
        event.stopPropagation();
    });

    $('#title').textfill({ maxFontPixels: 52, element: 'h1' });
    $('#subTitle').textfill({ maxFontPixels: 24, element: 'p' });

    //login
    $('#createAccountModalOpen').click(function (e) {
        GAQPush('_trackEvent', 'FE-3765', "Open Modal", "vi", 1, true);
        createAccountModalOpen();
        e.preventDefault();
    });

    $("#createAccountModalClose").click(function () {
        GAQPush('_trackEvent', 'FE-3765', "Closed Modal", "vi", 1, true);
        $("#modal_create_account").trigger("close");
    });

    $("#crmModalCreateEmail").bind('input', function () {
        $("#crmModalCreateEmail").removeClass("error");
        $("#crmModalCreateExists").hide();
        $("#crmModalCreateError").hide();
    });

    $("#crmModalCreatePass").bind('input', function () {
        $("#crmModalCreatePass").removeClass("error");
    });

    $("#crmModalCreateAccount").click(function () {
        var valid = true;
        if ($("#crmModalCreateEmail").val() === "") {
            $("#crmModalCreateEmail").addClass("error");
            valid = false;
        }
        if ($("#crmModalCreatePass").val() === "") {
            $("#crmModalCreatePass").addClass("error");
            valid = false;
        }
        if (valid) {
            $.ajax({
                type: "POST",
                data: { email: $("#crmModalCreateEmail").val() },
                url: '/CheckEmailRegistered.do;jsessionid=0E59AB08FCE8E5E5C454421FC1B54728.node153a',
                cache: false,
                success: function (data) {
                    if (data.emailRegistered) {
                        // Email already exists on account.
                        $("#crmModalCreateExists").show();
                        $("#crmModalCreateEmail").addClass("error");
                        GAQPush('_trackEvent', 'FE-3765', "Email already registered", "vi", 1, true);
                    }
                    else {
                        // Create account.
                        $("#crmCreateEmail").val($("#crmModalCreateEmail").val());
                        $("#crmCreatePsw").val($("#crmModalCreatePass").val());
                        GAQPush('_trackEvent', 'FE-3765', "Created Account", "vi", 1, true);
                        GAQPush('_trackEvent', 'CRM Event', 'New CRM Account', $("#crmModalCreateEmail").val(), 1, true);
                        GAQPush('_trackEvent', 'CRM Event', 'Loyalty entry', $("#crmModalCreateEmail").val(), 1, true);
                        if (createAccount() === false) {
                            $("#crmModalCreateEmail").addClass("error");
                        }
                    }
                }
            });
        }
    });
});

function createAccountModalOpen() {
    $('#btn-sign-in').removeClass('active');
    var $modalPromoCustoms = $(".modal_promo input[name='signupSigninModalCustomName']");
    var $modalPromoCustom;
    $modalPromoCustoms.each(function (index) {
        if ($(this).val() === "CreateAccount") {
            $modalPromoCustom = $(this);
        }
    });
    if ($modalPromoCustom != null) {
        $modalPromoCustom.parent(".modal_promo").lightbox_me({
            centered: true,
            overlayCSS:
            {
                background: "#000",
                opacity: 0.8
            }
        });
    } else {
        var data = {};
        data['layout'] = 'createAccountModal';
        $.ajax({
            type: "POST",
            url: '/RequestData.do;jsessionid=0E59AB08FCE8E5E5C454421FC1B54728.node153a',
            data: data,
            cache: false,
            success: function (data) {
                $("#modal_create_account").html(data);
                var $modalPromoCustoms = $(".modal_promo input[name='signupSigninModalCustomName']");
                var $modalPromoCustom;
                $modalPromoCustoms.each(function (index) {
                    if ($(this).val() === "CreateAccount") {
                        $modalPromoCustom = $(this);
                    }
                });
                $modalPromoCustom.parent(".modal_promo").lightbox_me({
                    centered: true,
                    overlayCSS:
                    {
                        background: "#000",
                        opacity: 0.8
                    }
                });
            },
            error: function () {
            }
        });
    }
    $('#btn-my-booking').removeClass('active');
}