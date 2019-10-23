(function (window, $) {
    var foxOne = window.foxOne;
    var param = foxOne.getQueryString();
    var values = [];
    var texts = [];
    var tableId = "";
    var cks = $("#" + tableId).find("input[type='checkbox']");
    var textCtrl = $("#" + param.text, window.parent.document);
    var valueCtrl = $("#" + param.id, window.parent.document);
    var v = valueCtrl.val();
    if (v && v != '') {
        values = v.split(',');
    }
    var t = textCtrl.val();
    if (t && t != '') {
        texts = t.split(',');
    }
    var initCk = function () {
        
        cks.bind("click", function () {
            onCkClick(this, this.checked);
        });
        if (!(param.multiple == "true")) {
            $("#btnCheckAll").hide();
            $("#btnCancelCheckAll").hide();
            cks.each(function () {
                $(this).attr("type", "radio");
            });
        }
        else {
            cks.each(function () {
                for (var i = 0; i < values.length; i++) {
                    if (values[i] == $(this).val()) {
                        $(this).attr("checked", true);
                        break;
                    }
                }
            });
        }
    };
    var onOk = function () {
        valueCtrl.val(values.join(','));
        textCtrl.val(texts.join(','));
        var onchangeFun = textCtrl.attr("onchange");
        if (onchangeFun && onchangeFun.length > 0) {
            eval("window.parent." + onchangeFun);
        }
    };

    var onClose = function () {
    };

    var checkAll = function (chk) {
        cks.each(function () {
            this.checked = chk;
            onCkClick(this, chk);
        });
    };

    var onCkClick = function (ck, checked) {
        if (!(param.multiple == "true")) {
            //单选
            if (checked) {
                values = [$(ck).val()];
                texts = [$(ck).attr("txt")];
            }
        }
        else {
            //多选
            if (checked) {
                if (values.indexOf($(ck).val()) == -1) {
                    values.push($(ck).val());
                    texts.push($(ck).attr("txt"));
                }
            }
            else {
                for (var i = 0; i < values.length; i++) {
                    if (values[i] == $(ck).val()) {
                        values.splice(i, 1);
                        texts.splice(i, 1);
                        break;
                    }
                }
            }
        }
    }
})(window, jQuery);


