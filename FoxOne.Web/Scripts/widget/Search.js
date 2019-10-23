/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $(window.document).on("submit", "[searchForm]", function (e) {
        var data = $(e.target).parent().data();
        var idArray = data.target.split(',');
        for (var i = 0; i < idArray.length; i++) {
            var tableId = idArray[i];
            var tableContext = foxOne.setting(tableId);
            tableContext.pageIndex = 1;
            var form = $(e.target).serialize();
            if (form != '') {
                var formCollection = form.split('&');
                if (formCollection.length > 0) {
                    for (var k = 0; k < formCollection.length; k++) {
                        if (formCollection[k] != '' && formCollection[k].indexOf('=') > 0) {
                            var kv = formCollection[k].split('=');
                            tableContext[kv[0]] = decodeURI(kv[1]);
                        }
                    }
                }
            }
            foxOne.refresh(tableId);
        }
        return false;
    });
})(window, jQuery);