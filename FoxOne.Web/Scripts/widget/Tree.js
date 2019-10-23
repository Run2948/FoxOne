/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    var setting = {
        view: {
            showLine: true,
            selectedMulti: false,
            dblClickExpand: false
        },
        data: {
            simpleData: {
                enable: true
            }
        },
        callback: {
            onClick: function (e, treeId, node) {
                var onNodeClick = $.Event("nodeClick", node);
                $("#" + treeId).trigger(onNodeClick);
            },
            onCheck: function (e, treeId, node) {
                var onNodeCheck = $.Event("nodeCheck", node);
                $("#" + treeId).trigger(onNodeCheck);
            }
        }
    };
    $("div[widget='Tree']").each(function () {
        var treeId = $(this).attr("id");
        var pageId = $(this).attr("PageId");
        var otherParam = foxOne.getQueryString();
        otherParam[foxOne.pageId] = pageId;
        otherParam[foxOne.ctrlId] = treeId;
        otherParam[foxOne.widgetType] = "Tree";
        var showcheck = $(this).attr("showcheck");
        setting.check = { enable: showcheck == "true" };
        var async = {
            enable: true,
            url: "/Home/GetWidgetData",
            autoParam: ["id=tree_node_id", "name=tree_node_name", "level=tree_node_level", "pId=tree_node_pId"],
            otherParam: otherParam,
            dataFilter: function (treeId, parentNode, response) {
                if (!response || !response.Data) return null;
                if (response.Result) {
                    if (response.NoAuthority) {
                        foxOne.alert(response.ErrorMessage);
                    }
                    else {
                        var dataLoad = $.Event("dataLoad", { d: response.Data });
                        $("#" + treeId).trigger(dataLoad);
                        return response.Data;
                    }
                }
                else {
                    foxOne.alert(response.ErrorMessage);
                }
            }
        };
        setting.async = async;
        $.fn.zTree.init($("#" + treeId + "-ul"), setting);
    });
})(window, jQuery);
