/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $(window.document).on("click", "div[widget='TabCondition']", function (e) {
        var span = $(e.target);
        if (!span.is("span")) {
            span = span.closest("span");
            if (!span || !span.attr("key") || span.attr("key") == '') {
                return;
            }
        }
        var div = span.closest("div");
        var data = div.data();
        var targetId = data.target;
        var idArray = targetId.split(',');
        for (var i = 0; i < idArray.length; i++) {
            var tableId = idArray[i];
            var key = span.attr("key");
            var currentSelected = div.children(".tab-item-selected");
            if (currentSelected.length != 0) {
                currentSelected.removeClass("tab-item-selected");
            }
            span.addClass("tab-item-selected");
            var setting = foxOne.setting(tableId);
            setting[data.field] = key;
            foxOne.refresh(tableId);
        }
    });
})(window, jQuery);
