(function ($) {
    var defaultOption = {
        url: "",
        width: 500,
        height: 400,
        title: "新窗口",
        onClose: function () { },
        overlayClose: false,
        targetWindow: window.top
    };
    $.fn.centerScreen = function (width, height, targetWindow) {
        var t = this;
        var marginLeft = -(width / 2);
        var marginTop = -(height / 2);
        var p = "absolute";
        if (!$.support.leadingWhitespace) {
            p = "absolute";
            marginTop += $(targetWindow).scrollTop();
        }
        t.css({ "position": p, "top": '50%', "left": '50%', 'margin-top': marginTop, 'margin-left': marginLeft });
        return this;
    };
    $.fn.modal = function (option) {
        option = $.extend({}, defaultOption, option);
        defaultOption = option;
        $.modalInner(this, option.overlayClose, option.onClose, option.width, option.height, option.targetWindow);
    };
    $.extend({
        modal: function (option) {
            option = $.extend({}, defaultOption, option);
            defaultOption = option;
            var newId = new Date().valueOf(),
                dialog = "<div class=\"modal-dialog\" style=\"width: " + option.width + "px; height: " + option.height + "px;\">",
                content = "<div class=\"modal-content\">",
                title = "<div class=\"modal-header\"><button title=\"关闭\" aria-label=\"Close\" data-dismiss=\"modal\" class=\"close btn-close\" type=\"button\"><span>×</span></button><h4 class=\"modal-title\">" + option.title + "</h4></div>",
                body = "<div class=\"modal-body\">",
                loading = "<div id='txtloading' class=\"loading\"></div>",
                iframe = "<iframe onload=\"$('#txtloading').remove();\" width=\"100%\" height=\"99%\" frameborder=\"0\" src=\"" + option.url + "\" scrolling=\"yes\"></iframe>";
            var html = [dialog, content, title, body, loading, iframe, "</div>", "</div>", "</div>"].join('');
            $.modalInner($(html), option.overlayClose, option.onClose, option.width, option.height, option.targetWindow, true);
        },
        modalInner: function (source, overlayClose, onClose, width, height, targetWindow, isRemove) {
            overlayClose = overlayClose || false;
            var targetBody = targetWindow.document.body;
            var overlayCount = $(targetBody).find(".overlay").length;
            var zIndex = (overlayCount * 2) + 10000;
            var overlayHeight = Math.max($(targetWindow).height(), $(targetWindow.document).height());
            overlay = $("<div class='overlay'></div>").css("min-height", overlayHeight).css("z-index", zIndex).appendTo(targetBody);
            if (overlayClose) {
                overlay.bind("click", function () {
                    try {
                        if (targetWindow && targetWindow.onDialogClose && targetWindow.onDialogClose.length > 0) {
                            targetWindow.onDialogClose.pop()('false');
                        }
                    } catch (e) { }
                });
            }
            var target = source.appendTo(targetBody);
            target.css("z-index", zIndex + 1).centerScreen(width, height, targetWindow);
            target.show();
            $(targetBody).find(".btn-close").click(function () {
                $.closeModal(isRemove);
            });
            if (!targetWindow.onDialogClose || targetWindow.onDialogClose.length == 0) {
                targetWindow.onDialogClose = [];
            }
            targetWindow.onDialogClose.push(function (res) {
                if (onClose && typeof (onClose) == 'function') {
                    onClose(res);
                    $.closeModal(isRemove);
                }
            });
            if (!targetWindow.dialogArray || targetWindow.dialogArray.length == 0) {
                targetWindow.dialogArray = [];
            }
            targetWindow.dialogArray.push(target);
        },
        closeModal: function (isRemove) {
            var targetWindow = defaultOption.targetWindow;
            var remove = isRemove === undefined ? true : isRemove;
            if (targetWindow && targetWindow.dialogArray) {
                if (targetWindow.dialogArray.length > 0) {
                    var target = targetWindow.dialogArray.pop();
                    target.prev(".overlay").remove();
                    if (remove) {
                        target.remove();
                    }
                    else {
                        target.hide();
                    }
                }
                if (targetWindow.dialogArray.length == 0) {
                    targetWindow.dialogArray = undefined;
                }
            }
        }
    });
})(jQuery);