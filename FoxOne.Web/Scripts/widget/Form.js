/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $(window.document).on("submit", "[defaultForm]", function (e) {
        var _this = e.target;
        if ($.validation) {
            var validateInfo = $.validation.validate(_this);
            if (validateInfo.isError) {
                var ee = $.Event("form.validateError", validateInfo);
                $(_this).trigger(ee);
                return false;
            }
        }
        var form = $(_this);
        form.find("[disabled]").removeAttr("disabled");
        var widget = form.closest("[widget]");
        var param = {};
        param[foxOne.ctrlId] = widget.attr("id");
        param[foxOne.pageId] = widget.attr("pageId");
        var url = foxOne.buildUrl(form.attr('action'), param);
        foxOne.dataService(url, form.serialize(), function (res) {
            try {
                var afterSubmit = $.Event("form.afterSubmit", { d: res });
                form.trigger(afterSubmit);
                if (window.top && window.top.onDialogClose && window.top.onDialogClose.length > 0) {
                    window.top.onDialogClose.pop()(res);
                }
            } catch (e) {
                foxOne.alert(e);
            }
        });
        return false;
    });
    $("[description]").each(function () {
        var desc = $(this).attr("description");
        if (desc != '') {
            $(this).append("<img alt='' title='" + desc + "' src=\"../../Content/themes/default/images/icon-tip.png\" />");
        }
    });
})(window, jQuery);