/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $(window.document).on("click", "div[widget='Toolbar']", function (e) {
        var input = $(e.target);
        if (!input.is("input")) return;
        var _this = $(this).data();
        if (input.attr("onclick")) return;
        var id = input.attr("id");
        var tableId = _this.target;
        if (tableId && tableId != '') {
            var table = $("#" + tableId);
            var data = table.data();
            var editUrl = data.editurl;
            var deleteUrl = data.deleteurl || foxOne.defaultDeleteUrl;
            var param = foxOne.getQueryString();
            param[foxOne.formViewMode] = 'Insert';
            var insertUrl = foxOne.buildUrl(data.inserturl, param, true);
            switch (id) {
                case "btnInsert":
                    $.modal({
                        url: insertUrl,
                        onClose: function (res) {
                            if (res != 'false') {
                                foxOne.refresh(tableId);
                            }
                        },
                        width: 900,
                        height: 500,
                        title: '新增',
                        overlayClose: true
                    });
                    break;
                case "btnBatchDelete":
                    var ids = [];
                    var keyValue = {};
                    keyValue[foxOne.ctrlId] = table.attr("id");
                    keyValue[foxOne.pageId] = table.attr("pageId");
                    table.find(":checked").not("[checkAll]").each(function () {
                        var tr = $(this).closest("tr").data();
                        var id = tr.key
                        if (id && id != '') {
                            ids.push(id);
                        }
                    });
                    if (ids.length == 0) {
                        foxOne.alert('请选择要删除的记录！');
                        return;
                    }
                    else {
                        if (!confirm("确认要删除选中的" + ids.length + "条记录吗？")) {
                            return;
                        }
                    }
                    keyValue[foxOne.keyName] = ids.join(',');
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
