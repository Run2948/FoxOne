(function (window, $) {
    var foxOne = function () {
    };
    foxOne.prototype = {
        pageId: "_PAGE_ID",
        parentId: "_PARENT_ID",
        targetId: "_TARGET_ID",
        baseType: "_BASE_TYPE",
        ctrlId: "_CTRL_ID",
        typeName: "_TYPE_NAME",
        formViewMode: "_FORM_VIEW_MODE",
        widgetType: "_WIDGET_TYPE",
        keyName: "_FORM_KEY",
        pageIndex: "_PAGE_INDEX",
        pageSize: "_PAGE_SIZE",
        sortExpression: "_SORT_EXPRESSION",
        defaultDeleteUrl:"/Entity/Delete",
        contextPath: document.location.protocol + "//" + document.location.host,
        constructor: foxOne,
        /*
        dataService三大功能：
        1.从服务端获取SQL语句执行结果 sqlid:xxxxxx
        2.执行SQL语句 exec:xxxxxx
        3.获取指定url返回的值 /{Controller}/{Action}
        url:/{Controller}/{Action} || sqlid:xxxxxx || exec:xxxxxx
        data:json格式的参数，例{id:'xxx',name:'aaa'}
        callback:回调函数，带一个参数，如果是执行sqlid:xxxxxx，则该参数是返回一个数组，每一个数组是SqlId返回的行;
                                   如果是执行exec:xxxxxx，则该参数是返回执行SQL受影响的行数;
                                   如果是执行/{Controller}/{Action}，则根据action返回值而定。
        async:true为异步,false为同步
        */
        dataService: function (url, data, callback, method, async) {
            if (async === undefined) {
                async = true;
            }
            if (method === undefined) {
                method = "post";
            }
            var prefixSet = [];
            var that = this;
            prefixSet.push({ prefix: 'sqlid:', url: '/Home/GetDataBySqlId' });
            prefixSet.push({ prefix: 'exec:', url: '/Home/GetDataBySqlId' });
            for (var i = 0; i < prefixSet.length; i++) {
                var p = prefixSet[i];
                if (url.indexOf(p.prefix) == 0) {
                    data.sqlId = url.substring(p.prefix.length);
                    data.type = p.prefix;
                    url = p.url;
                    break;
                }
            }
            $.ajax({
                type: method,
                url: url,
                async: async,
                data: data,
                dataType: "json",
                success: function (response) {
                    if (response.Result) {
                        if (response.NoAuthority) {
                            that.alert(response.ErrorMessage);
                        }
                        else {
                            if (response.LoginTimeOut) {
                                window.top.location.href = foxOne.contextPath + "/Home/LogOn";
                            }
                            else {
                                callback(response.Data);
                            }
                        }
                    }
                    else {
                        that.alert(response.ErrorMessage);
                    }
                },
                error: function (res) {
                    that.alert("请求处理异常:" + res.responseText);
                }
            });
        },
        alert: function (msg) {
            window.alert(msg);
        },
        confirm: function (msg, onOk, onCancel) {
            if (window.confirm(msg)) {
                onOk();
            }
            else {
                onCancel();
            }
        },
        /*
        stringFormat:类似于C#中的string.Format
        使用示例：foxOne.stringFormat("my name is {0}","liuhaifeng") -- 输出:my name is liuhaifeng
        */
        stringFormat: function (string) {
            var args = arguments;
            var pattern = new RegExp("{([0-" + arguments.length + "])}", "g");
            return String(string).replace(pattern, function (match, index) {
                var currentIndex = parseInt(index);
                if (currentIndex + 1 > args.length || currentIndex < 0) {
                    throw new Error("参数索引出错");
                }
                return args[currentIndex + 1];
            });
        },
        /*
        异步提交表单
        formId:表单Id
        onSuccess:提交成功回调
        */
        formAjaxSubmit: function (formId, onSuccess) {
            var form = $("#" + formId);
            var postData = form.serialize();
            this.dataService(form.attr("action"), postData, onSuccess);
        },
        /*
        modelViewBinder:模型视图绑定
        使用示例：
        var model={name:'liuhaifeng',age:30};
        var view="<div>{name}</div><p>{age}</p>";
        var s = foxOne.modelViewBinder(model,view);
        执行完后，s="<div>liuhaifeng</div><p>30</p>"
        */
        modelViewBinder: function (model, view) {
            var pattern = new RegExp("{([^}]*)}", "g");
            var that = this;
            return String(view).replace(pattern, function (match, index) {
                match = match.substring(1, match.length - 1);
                return that.getPropertyValue(model, match);
            });
        },
        /*
        modelListBinder:批量模型视图绑定
        使用示例：
        var modelList=[{name:'liuhaifeng',age:30},{name:'hello',age:31}];
        var view="<div>{name}</div><p>{age}</p>";
        var s = foxOne.modelListBinder(modelList,view,function(m){m.name+='1';return m;});
        执行完后，s="<div>liuhaifeng1</div><p>30</p> <div>hello1</div><p>31</p>"
        */
        modelListBinder: function (modelList, view, onBind) {
            var rValue = "";
            var that = this;
            $.each(modelList, function (i) {
                if (typeof (onBind) == "function") {
                    onBind(modelList[i]);
                }
                rValue += that.modelViewBinder(modelList[i], view);
            });
            return rValue;
        },
        getPropertyValue: function (model, propertyString) {
            var i = propertyString.split(".");
            var result = model;
            for (var j = 0; j < i.length; j++) {
                result = result[i[j]];
            }
            return result;
        },
        dataBinding: function (container, data, template, bindingType) {
            bindingType = bindingType || "append";
            switch (bindingType) {
                case "prepend": container.prepend(this.modelListBinder(data, template)); break;
                case "replace": container.html(this.modelListBinder(data, template)); break;
                case "append": container.append(this.modelListBinder(data, template)); break;
            }
        },
        /*
        获取当前页面的URL参数，以JSON返回
        示例：当前页面URL为/home/index?id=3&name=hello;
             var s=foxOne.getQueryString();
             执行完后s={id:3,name:'hello'}
        */
        getQueryString: function () {
            if (window.location.search == "") {
                return { "": "无URL参数" };
            }
            var q = window.location.search.substring(1).split("&");
            var returnValue = {};
            for (var i = 0; i < q.length; i++) {
                var temp = q[i].split("=");
                returnValue[temp[0]] = temp[1];
            }
            return returnValue;
        },
        changeDatePickerDate: function (datePickerId, days) {
            var pk = $("#" + datePickerId);
            var newTimePoint = this.changeDate(pk.val(), days);
            pk.val(newTimePoint);
        },
        changeDate: function (date, days) {
            var date = date == "" ? new Date() : new Date(date);
            date.setDate(date.getDate() + days);
            var d = date.getDate();
            if (d < 10) {
                d = "0" + d;
            }
            var m = date.getMonth() + 1;
            if (m < 10) {
                m = "0" + m;
            }
            var newTimePoint = date.getFullYear() + "-" + m + "-" + d;//date.getDate();
            return newTimePoint;
        },
        slideDown: function (source, target, offset) {
            offset = offset || 0;
            var sourcePosition = source.offset();
            var top = 0, left = 0;
            var top = sourcePosition.top + source.outerHeight() + offset;
            var left = sourcePosition.left;
            var isOverflow = (sourcePosition.left + target.outerWidth()) > $(window).width();
            while ((left + target.outerWidth()) > $(window).width() && left > 0) {
                left -= 50;
            }
            target.css({ 'position': 'absolute', 'left': left + 'px', 'top': top + 'px' }).show();
        },
        toolTip: function (source, target) { },
        /*
        buildUrl:根据url和传入的parameter，构建新的url
        url:url
        parameter:JSON参数
        addRandomCode:是否加随机数（防止JS请求过程中客户端缓存）
        示例：
        var url="/home/index";
        var parameter={id:3,name:'hello'};
        var s=foxOne.buildUrl(url,parameter,true);
        执行后s="/home/index?id=3&name=hello&randomCode=198485938505";
        */
        buildUrl: function (url, parameter, addRandomCode) {
            addRandomCode = addRandomCode || false;
            if (parameter) {
                if (addRandomCode) {
                    parameter.randomCode = new Date().valueOf();
                }
                var queryString = "";
                for (var attr in parameter) {
                    if (attr == "") continue;
                    var value = parameter[attr];
                    if (queryString.length > 0) { queryString += "&"; }
                    queryString += attr + "=" + encodeURI(value);
                }
                if (queryString.length > 0) {
                    if (url.indexOf("?") >= 0) {
                        url = url + "&" + queryString;
                    } else {
                        url = url + "?" + queryString;
                    }
                }
            }
            if (url.indexOf('http://') < 0) {
                if (url.indexOf('/') != 0) {
                    url = this.contextPath + "/" + url;
                }
                else {
                    url = this.contextPath + url;
                }
            }
            return url;
        },
        refresh: function (id) {
            var that = this;
            var setting = that.setting(id);
            var urlParameter = that.getQueryString();
            var ifExistInSetting = false;
            for (var key in urlParameter) {
                if (key == "") continue;
                ifExistInSetting = false;
                for (var sKey in setting) {
                    if (sKey.toUpperCase() == key.toUpperCase()) {
                        ifExistInSetting = true;
                        break;
                    }
                }
                if (!ifExistInSetting) {
                    setting[key] = urlParameter[key];
                }
            }
            var target = $("#" + id);
            if (target.attr("widget") == "Table" || target.attr("widget") == "Repeater") {
                var ctrlId = id;
                var pageId = target.attr("PageId");
                $.get(that.stringFormat("/Page/{0}/{1}", pageId, ctrlId), setting, function (res) {
                    target.before(res).remove();
                    var afterRefresh = $.Event("afterRefresh", res);
                    $("#" + id).trigger(afterRefresh);
                    that.autoHeight();
                }, "html");
            }
        },
        setting: function (id) {
            var prefix = "widget-setting-";
            if (!$("body").data(prefix + id)) {
                var param = {};
                param[this.pageIndex] = 1;
                param[this.sortExpression] = "";
                $("body").data(prefix + id, param);
            }
            return $("body").data(prefix + id);
        },
        init: function () {
            this.autoHeight();
            $(window).bind("resize", this.autoHeight);
            $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);
        },
        autoHeight: function () {
            var listHeight = $(window).height();
            var offset = 20;
            var autoHeightCtrl = $("[data-autoHeight]");
            if (autoHeightCtrl.length > 0) {
                if (autoHeightCtrl.attr("data-autoOffset").length > 0) {
                    offset += parseInt(autoHeightCtrl.attr("data-autoOffset"));
                }
                autoHeightCtrl.siblings().each(function () {
                    listHeight -= ($(this).outerHeight());
                });
                listHeight -= offset;
                autoHeightCtrl.css("height", listHeight).css("overflow-y", "auto");
            }
        },
        design: function () {
            var that = this;
            $("[layout]").each(function () {
                var urlParam = that.getQueryString();
                var pageId = urlParam[that.pageId];
                var layout = $(this);
                var url = "/PageDesigner/ComponentList";
                var param = {};
                param[that.formViewMode] = "Insert";
                param[that.parentId] = pageId;
                param[that.pageId] = pageId;
                param[that.baseType] = "FoxOne.Controls.PageControlBase";
                param[that.targetId] = layout.attr("layout");
                url = that.buildUrl(url, param);
                if (layout.find(".add-component").length == 0) {
                    $("<div class='add-component' style='margin:10px;padding:10px;cursor:pointer;background-color:#EEE;border:1px solid #CCC;font-size:14px;font-weight:bold;text-align:center;'>添加组件</div>").appendTo(layout).bind("click", function () {
                        $.modal({ title: '添加页面组件', url: url, overlayClose: true, width: 1100, height: 650, onClose: function (res) { window.location.href = window.location.href; } });
                    });
                }
            });
            var initWidgetDesign = function (_this) {
                var widget = $(_this);
                var ctrlId = widget.attr("id");
                var pageId = widget.attr("PageId");
                if (!ctrlId) return;
                var param = {};
                param[that.ctrlId] = ctrlId;
                param[that.pageId] = pageId;
                param[that.parentId] = pageId;
                param[that.formViewMode] = "Edit";
                widget.css("border", "1px dashed red");
                if (widget.children("[daction]").length == 0) {
                    $("<span daction='edit' style='margin:5px;padding:5px;cursor:pointer;background-color:#FFF;border:1px dashed red;font-size:14px;font-weight:bold;color:red;'>编辑</span>").appendTo(widget).bind("click", function () {
                        var url = "/PageDesigner/ComponentEditor";
                        url = that.buildUrl(url, param);
                        $.modal({ url: url, width: 1100, height: 650, overlayClose: true, onClose: function (res) { window.location.href = window.location.href; } });
                    });
                    $("<span daction='delete' style='padding:5px;cursor:pointer;background-color:#FFF;border:1px dashed red;font-size:14px;font-weight:bold;color:red;'>删除</span>").appendTo(widget).bind("click", function () {
                        if (confirm("确定删除该组件?")) {
                            that.dataService("/PageDesigner/ComponentDelete/", param, function (res) {
                                if (res) {
                                    widget.remove();
                                }
                            }, "post", true);
                        }
                    });
                }
            }
            $("[widget]").each(function () {
                initWidgetDesign(this);
            });
            $(document).on("mouseover", "[widget]", function () {
                initWidgetDesign(this);
            });
        },
    };
    window.foxOne = new foxOne();
    window.foxOne.init();
})(window, jQuery);