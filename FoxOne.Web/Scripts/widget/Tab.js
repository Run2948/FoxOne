/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $(window.document).on("click", "div[widget='Tab']", function (e) {
        var span = $(e.target);
        if (!span.is("span")) {
            span = span.closest("span");
            if (!span || !span.attr("key") || span.attr("key") == '') {
                return;
            }
        }
        var pageId = $(this).attr("pageId");
        var div = $("div[tabItem='" + span.attr("key") + "']");
        var onTabItemClick = $.Event("tabItemClick", { span: span, div: div });
        $(this).trigger(onTabItemClick);
        if (onTabItemClick.isPropagationStopped()) {
            return;
        }
        if (div.attr("contentId") && !div.attr("loaded")) {
            var contentIds = div.attr("contentId").split('|');
            var url = "/Page/" + pageId + "/";
            for (var k = 0; k < contentIds.length; k++) {
                var contentId = contentIds[k];
                $.get(url + contentId, {}, function (res) {
                    div.append(res);
                    foxOne.autoHeight();
                }, "html");
            }
        }
        var iframe = div.find("iframe");
        if (iframe.length > 0 && iframe.attr("src") == '') {
            iframe.attr("src", div.attr("src1"));
        }
        span.closest("[widget]").find("[tabItem]").hide();
        var currentSelected = span.parent().children(".tab-item-selected");
        if (currentSelected.length != 0) {
            currentSelected.removeClass("tab-item-selected");
        }
        div.show();
        span.addClass("tab-item-selected");
    });
    $(window.document).ready(function () {
        $("div[widget='Tab']").each(function () {
            var index = parseInt($(this).attr("initindex")) || 0;
            var e = $.Event("click");
            $(this).find(".tab-item").eq(index).trigger(e);
        });
    });
})(window, jQuery);



