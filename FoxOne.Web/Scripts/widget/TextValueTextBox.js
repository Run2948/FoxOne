/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    var sufix = "_SLIDE_DIV";
    $("input[data-target]").bind("click", function (e) {
        var data = $(this).data();
        var width = data.dialogwidth;
        var height = data.dialogheight;
        var targetValueId = data.target;
        var targetTextId = $(this).attr("id");
        var selectUrl = "/Page/" + data.selector;
        var e = $.Event('onBeforeSelect');
        var el = $("#" + targetValueId);
        var param = { _input_id: targetValueId, _input_text: targetTextId, _is_multiple: data.multiple };
        el.trigger(e);
        if (e && e.Data) {
            param = $.extend(param, e.Data);
        }
        selectUrl = foxOne.buildUrl(selectUrl, param);
        if (data.showtype.toLowerCase() == "modal") {
            $.modal({
                url: selectUrl, width: width, height: height, overlayClose: true, onClose: function (res) {

                }
            });
        }
        else {
            var containerId = targetTextId + sufix;
            var $div = $("#" + containerId);
            $("div[id*='" + sufix + "']").hide();
            if ($div.length > 0) {
                $div.show();
            }
            else {
                $div = $("<div></div>")
                    .attr("id", containerId)
                    .addClass("alert-box")
                    .css("width", width)
                    .css("height", height);
                $("<iframe >")
                    .attr("src", selectUrl)
                    .attr("width", "100%")
                    .attr("height", "100%")
                    .attr("frameborder", "0")
                    .attr("scrolling", "no")
                    .attr("allowtransparency", "true").appendTo($div);
                $div.appendTo("body");
                foxOne.slideDown($(this), $div, 1);
            }
        }
    });
    $(document).bind("click", function (e) {
        var target = $(e.target);
        if (target.is("input[data-target]")) return;
        $("div[id*='" + sufix + "']").hide();
    });
})(window, jQuery);