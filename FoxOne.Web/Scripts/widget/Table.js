/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $(window.document).on("click", "[checkAll]", function () {
        var allCk = $(this).closest("table").find("input[type='checkbox']").not("[checkAll]");
        var checked = this.checked;
        allCk.each(function () {
            this.checked = checked;
        });
    });
    $(window.document).on("click", "div[widget='Table']", function (e) {
        var src = e.target;
        var table = $(this);
        var tableId = table.attr("id");
        var pageId = table.attr("PageId");
        if (src.tagName.toLowerCase() != 'a') return;
        var a = $(src);
        if (a.attr("sortField")) {
            field = a.attr("sortField");
            var tableContext = foxOne.setting(tableId);
            var exp = tableContext.sortExpression;
            if (exp == "") {
                exp = field + " asc";
            }
            else {
                if (exp.indexOf(field) >= 0) {
                    if (exp.indexOf(field + " asc") >= 0) {
                        exp = exp.replace(field + " asc", field + " desc");
                    }
                    else {
                        exp = exp.replace(field + " desc", field + " asc");
                    }
                }
                else {
                    exp += "," + field + " asc";
                }
            }
            tableContext.sortExpression = exp;
            tableContext.pageIndex = 1;
            foxOne.refresh(tableId);
        }
        if (a.attr("trButton") && !a.attr("onclick")) {
            var id = a.attr("trButton");
            var d = table.data();
            var editUrl = d.editurl;
            var deleteUrl = d.deleteurl || foxOne.defaultDeleteUrl;
            var tr = a.closest("tr");
            d = tr.data();
            var keyValue = {};
            keyValue[foxOne.keyName] = d.key;
            keyValue[foxOne.ctrlId] = tableId;
            keyValue[foxOne.pageId] = pageId;
            switch (id) {
                case "btnEdit":
                    delete keyValue[foxOne.ctrlId]
                    delete keyValue[foxOne.pageId]
                    $.modal({
                        url: foxOne.buildUrl(editUrl, keyValue, true),
                        overlayClose: true,
                        onClose: function (res) {
                            if (res != 'false') {
                                foxOne.refresh(tableId);
                            }
                        },
                        title: '编辑',
                        width: 800,
                        height: 500
                    });
                    break;
                case "btnDelete":
                    if (!confirm("确认要删除当前选中的记录吗？")) {
                        return;
                    }
                    foxOne.dataService(deleteUrl, keyValue, function (res) {
                        if (res) {
                            foxOne.refresh(tableId);
                        }
                    });
                    break;
            }
        }

    });
})(window, jQuery);
