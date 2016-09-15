var stripRows, stripPageIndex, stripPageSize;

$(document).ready(function () {
    stripRows = 0;
    stripPageIndex = 0;
    stripPageSize = 8;
    loadServicesStrip();
});

function onNextStrip() {
    if (stripPageIndex >= stripRows - 1)
        stripPageIndex = 0;
    else
        stripPageIndex++;
    loadServicesStrip();
}

function onPreviousStrip() {
    if (stripPageIndex <= 0)
        stripPageIndex = stripRows - 1;
    else
        stripPageIndex--;
    loadServicesStrip();
}

function loadServicesStrip() {
    $.ajax({
        url: "/Service/Service/GetServices",
        data: { index: stripPageIndex, size: stripPageSize },
        beforeSend: function () {
            $(".sprocket-strips-s").addClass("loading");
        },
        complete: function () {
            setTimeout(function () {
                $(".sprocket-strips-s").removeClass("loading");
            }, 1800);
        },
        success: function (response) {
            if (response.success) {
                var result = response.data;
                stripRows = result.pages.length;
                stripIndexPage = result.currentIndex;

                $("#toogle").fadeToggle(1500, "swing", function () {
                    $("#stripServiceList").html($("#stripServicesTemplate").tmpl(result.items));
                    $("#stripServicePagination").html($("#stripPagesTemplate").tmpl(result.pages));

                    var currentPage = stripIndexPage + 1;

                    $("ul#stripServicePagination li").each(function () {
                        var nodeStripPage = $(this).attr("data-strips-page");

                        if (nodeStripPage == currentPage) {
                            $(this).addClass("active");
                        }
                        else {
                            if ($(this).hasClass("active"))
                                $(this).removeClass("active");
                        }

                        $(this).click(function () {
                            stripPageIndex = nodeStripPage - 1;
                            loadServicesStrip();
                        });
                    });
                });
            }
        }
    });
}
