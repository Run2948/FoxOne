/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne,
        PAGE_INDEX = "pageIndex",
        PAGE_SIZE="pageSize",
        PRE = "Pre",
        NEXT = "Next";

    $(window.document).on("click", "div[widget='Pager']", function (e) {
        var a = $(e.target);
        if (!a.is("a")) return;
        var pagerData = $(this).data();
        var idArray = pagerData.target.split(',');
        for (var i = 0; i < idArray.length; i++) {
            var tableId = idArray[i];
            var setting = foxOne.setting(tableId);
            var currentPageIndex = setting[foxOne.pageIndex];
            if (a.attr(PAGE_INDEX)) {
                var pageCount = parseInt(pagerData.pageCount);
                var pageIndex = a.attr(PAGE_INDEX);
                if (pageIndex == PRE) {
                    if (currentPageIndex > 1) {
                        currentPageIndex--;
                    }
                    else {
                        return false;
                    }
                }
                else if (pageIndex == NEXT) {
                    if (currentPageIndex >= pageCount) {
                        return false;
                    }
                    else {
                        currentPageIndex++;
                    }
                }
                else {
                    currentPageIndex = pageIndex;
                }
            }
            if (a.attr(PAGE_SIZE)) {
                currentPageIndex = 1;
                setting[foxOne.pageSize] = a.attr(PAGE_SIZE);
            }
            setting[foxOne.pageIndex] = currentPageIndex;
            foxOne.refresh(tableId);
        }
    });
})(window, jQuery);