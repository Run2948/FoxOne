(function ($) {
    $.fn.tipsy = function (options) {

        options = $.extend({}, $.fn.tipsy.defaults, options);

        return this.each(function () {

            var opts = $.fn.tipsy.elementOptions(this, options);

            $(this).hover(function () {

                $.data(this, 'cancel.tipsy', true);

                var tip = $.data(this, 'active.tipsy');
                if (!tip) {
                    tip = $('<div class="tipsy"><div class="tipsy-inner"/></div>');
                    tip.css({ position: 'absolute', zIndex: 100000 });
                    $.data(this, 'active.tipsy', tip);
                }

                if ($(this).attr('title') || typeof ($(this).attr('original-title')) != 'string') {
                    $(this).attr('original-title', $(this).attr('title') || '').removeAttr('title');
                }

                var title;
                if (typeof opts.title == 'string') {
                    title = $(this).attr(opts.title == 'title' ? 'original-title' : opts.title);
                } else if (typeof opts.title == 'function') {
                    title = opts.title.call(this);
                }

                if (title == undefined || title.length == 0) return false;

                tip.find('.tipsy-inner')[opts.html ? 'html' : 'text'](title || opts.fallback);

                var pos = $.extend({}, $(this).offset(), { width: this.offsetWidth, height: this.offsetHeight });
                tip.get(0).className = 'tipsy'; 
                tip.remove().css({ top: 0, left: 0, visibility: 'hidden', display: 'block' }).appendTo(document.body);
                var actualWidth = tip[0].offsetWidth, actualHeight = tip[0].offsetHeight;
                var gravity = (typeof opts.gravity == 'function') ? opts.gravity.call(this) : opts.gravity;

                switch (gravity.charAt(0)) {
                    case 'n':
                        tip.css({ top: pos.top + pos.height, left: pos.left + pos.width / 2 - actualWidth / 2 }).addClass('tipsy-north');
                        break;
                    case 's':
                        tip.css({ top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2 }).addClass('tipsy-south');
                        break;
                    case 'e':
                        tip.css({ top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth }).addClass('tipsy-east');
                        break;
                    case 'w':
                        tip.css({ top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width }).addClass('tipsy-west');
                        break;
                }

                if (opts.fade) {
                    tip.css({ opacity: 0, display: 'block', visibility: 'visible' }).animate({ opacity: 0.8 });
                } else {
                    tip.css({ visibility: 'visible' });
                }

            }, function () {
                $.data(this, 'cancel.tipsy', false);
                var self = this;
                setTimeout(function () {
                    if ($.data(this, 'cancel.tipsy')) return;
                    var tip = $.data(self, 'active.tipsy');
                    if (!tip) return;
                    if (opts.fade) {
                        tip.stop().fadeOut(function () { $(this).remove(); });
                    } else {
                        tip.remove();
                    }
                }, 100);

            });

        });

    };
    $.fn.tipsy.elementOptions = function (ele, options) {
        return $.metadata ? $.extend({}, options, $(ele).metadata()) : options;
    };

    $.fn.tipsy.defaults = {
        fade: false,
        fallback: '',
        gravity: 'n',
        html: false,
        title: 'title'
    };

    $.fn.tipsy.autoNS = function () {
        return $(this).offset().top > ($(document).scrollTop() + $(window).height() / 2) ? 's' : 'n';
    };

    $.fn.tipsy.autoWE = function () {
        return $(this).offset().left > ($(document).scrollLeft() + $(window).width() / 2) ? 'e' : 'w';
    };

})(jQuery);

(function ($) {
    $.fn.validationConfig = function () {
    };
    $.validationConfig = {
        newLang: function () {
            $.validationConfig.allRules = {
                "required": {
                    "executor": "_required"
                },
                "pattern": {
                    "executor": "_customRegex"
                },
                "func": {
                    "executor": "_funcCall"
                },
                "unique": {
                    "executor": "_unique"
                },
                "length": {
                    "executor": "_length"
                },
                "range": {
                    "executor": "_range"
                },
                "equalToField": {
                    "executor": "_confirm",
                    "alertText": "输入值与相关信息不相符"
                },
                "url": {
                    "regex": /^(http|https|ftp):\/\/(([A-Z0-9][A-Z0-9_-]*)(\.[A-Z0-9][A-Z0-9_-]*)+)(:(\d+))?\/?/i,
                    "executor": "_customRegex",
                    "alertText": "网址输入不正确"
                },
                "qq": {
                    "regex": /^[1-9][0-9]{4,}$/,
                    "executor": "_customRegex",
                    "alertText": "QQ号码输入不正确（非零开头的四位以上的数字）"
                },
                "telephone": {
                    "regex": /^(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/,
                    "executor": "_customRegex",
                    "alertText": "电话号码输入不正确"
                },
                "mobile": {
                    "regex": /^1[3|5|8]\d{9}$/,
                    "executor": "_customRegex",
                    "alertText": "手机号码输入不正确"
                },
                "zip": {
                    "regex": /^[1-9]\d{5}$/,
                    "alertText": "邮政编码输入不正确"
                },
                "email": {
                    "regex": /^[a-zA-Z0-9_\.\-]+\@([a-zA-Z0-9\-]+\.)+[a-zA-Z0-9]{2,4}$/,
                    "executor": "_customRegex",
                    "alertText": "邮箱地址输入不正确"
                },
                "date": {
                    "regex": /^[0-9]{4}\-[0-9]{1,2}\-[0-9]{1,2}$/,
                    "executor": "_customRegex",
                    "alertText": "日期输入格式不正确（YYYY-MM-DD）"
                },
                "identity": {
                    "regex": /\d{15}|\d{18}/,
                    "executor": "_customRegex",
                    "alertText": "身份证输入不正确"
                },
                "money": {
                    "regex": /^[0-9]+(.[0-9]{1,4})?$/,
                    "executor": "_customRegex",
                    "alertText": "金额格式输入不正确"
                },
                "money1": {
                    "regex": /^[0-9]+(.[0-9]{1,2})?$/,
                    "executor": "_customRegex",
                    "alertText": "金额格式输入不正确"
                },
                "integer": {
                    "regex": /^\d+$/,
                    "executor": "_customRegex",
                    "alertText": "输入值必须是正整数"
                },
                "telnumber": {
                    "regex": /^\d+$/,
                    "executor": "_customRegex",
                    "alertText": "电话号码必须是数值字符"
                },
                "double": {
                    "regex": /^(-)?[0-9]+([.][0-9]{1,})?$/,
                    "executor": "_customRegex",
                    "alertText": "输入值必须是数值"
                },
                "digit": {
                    "regex": /^[0-9]+$/,
                    "executor": "_customRegex",
                    "alertText": "只能输入数字"
                },
                "noSpecialCaracters": {
                    "regex": /^[0-9a-zA-Z]+$/,
                    "executor": "_customRegex",
                    "alertText": "不允许输入字母和数字之外的特殊字符"
                },
                "letter": {
                    "regex": /^[a-zA-Z]+$/,
                    "executor": "_customRegex",
                    "alertText": "只允许输入英文"
                },
                "chinese": {
                    "regex": /^[\u0391-\uFFE5]+$/,
                    "executor": "_customRegex",
                    "alertText": "只允许输入中文"
                }
            }
        }
    }

    $.validationConfig.newLang();
})(jQuery);

(function ($) {

    var gravity = $.fn.tipsy.autoNS;
    $.fn.validation = function (settings) {
        $.validation.settings = $.extend({
            allrules: $.validationConfig.allRules,
            validationEventTriggers: "blur",
            beforeSuccess: function () { },
            failure: function () { }
        }, settings);

        var els = $(this).find("[validator]");
        els.tipsy({ gravity: gravity, title: "errorInfo", trigger: 'manual' });
        els.find(":radio,:checkbox").each(function () {
            if (!this.disabled) {
                $(this).attr("group", $(this).closest("[validator]").attr("id"));
                $(this).attr("validator", $(this).closest("[validator]").attr("validator"));
            }
        });

        els.each(function () {
            var el = $(this);
            if (el.attr("disabled") == 'disabled') {
                return;
            }
            if (el.attr("event")) {
                el.bind(el.attr("event"), function () {
                    $.validation.loadValidation(el);
                });
            }
            else {
                if (el.is("select")) {
                    el.bind("change", function () {
                        $.validation.loadValidation(el);
                    });
                }
                else if (el.is("input")) {
                    if (el.attr("type") == "text") {
                        el.bind("blur", function () {
                            $.validation.loadValidation(el);
                        });
                    }
                    else {
                        el.bind("click", function () {
                            $.validation.loadValidation(el);
                        });
                    }
                }
                else {
                    el.bind("blur", function () {
                        $.validation.loadValidation(el);
                    });
                }
            }
        });
    };

    $.validation = {
        loadValidation: function (caller) {
            var rules = new Array();
            if (!$.validation.settings)
            {
                $.validation.settings = {
                    allrules: $.validationConfig.allRules,
                    validationEventTriggers: "blur",
                    failure: function () { }
                }
            }
            var getRules = $(caller).attr('validator');
            if (!getRules)
                return false;
            var ruleOptions = getRules.match(/\[[^\]]+(\]\]|\])/g);
            if (ruleOptions) {
                $.each(ruleOptions, function (index, value) {
                    getRules = getRules.replace(this, ("##" + index));
                });
            }

            getRules = getRules.split(",");
            $.each(getRules, function (index, value) {
                var ruleAndOption = this.split("##");
                if (ruleAndOption && ruleAndOption.length == 2) {
                    rules.push({
                        name: ruleAndOption[0],
                        options: ruleOptions[ruleAndOption[1]].replace(/^\[|\]$/g, "").split(",")
                    });
                } else {
                    rules.push({
                        name: ruleAndOption[0],
                        options: []
                    });
                }
            });

            return $.validation.validateCall(caller, rules)
        },

        validateCall: function (caller, rules) {
            var promptText = "";
            var callerName = $(caller).attr("name");
            $.validation.isError = false;
            callerType = $(caller).attr("type");

            $.each(rules, function (i, v) {
                var validator = $.validation.settings.allrules[this.name];
                if (validator) {
                    eval(validator.executor + "(caller,this)");
                } else {
                    $.validation.debug("验证器拼写有误: " + "name=" + $(caller).attr("id") + " validator=" + $(caller).attr("validator"));
                    return false;
                }
                if (promptText.length > 0) {
                    return false;
                }
            });

            groupInputHack();
            if ($.validation.isError == true) {
                $.validation.buildPrompt(caller, promptText);
            } else {
                $.validation.closePrompt(caller);
            }

            function groupInputHack() {
                callerName = $(caller).attr("group");
                if ($("input[group='" + callerName + "']").size() > 1) {
                    caller = $("input[group='" + callerName + "']");
                }
            }

            function _required(caller, rule) {
                var callerType = $(caller).attr("type");
                if (caller.tagName == "SPAN") {
                    if (!$.trim($(caller).text())) {
                        $.validation.isError = true;
                        promptText += _buildPromptText("该输入项必填", rule.options[0]);
                    }
                }
                if (caller.tagName == "TEXTAREA" || callerType == "text" || callerType == "password" || callerType == "file") {
                    if (!$.trim($(caller).val())) {
                        $.validation.isError = true;
                        promptText += _buildPromptText("该输入项必填", rule.options[0]);
                    }
                } else if (callerType == "radio" || callerType == "checkbox") {
                    callerName = $(caller).attr("group");
                    if ($("input[group='" + callerName + "']:checked").size() == 0) {
                        $.validation.isError = true;
                        if ($("input[group='" + callerName + "']").size() == 1) {
                            promptText += _buildPromptText("该选项为必选项", rule.options[0]);
                        } else {
                            promptText += _buildPromptText("必须选择一个选项", rule.options[0]);
                        }
                    }
                }
                else if (callerType == "select-one") {
                    if (!$(caller).val()) {
                        $.validation.isError = true;
                        promptText += _buildPromptText("该选择项必选", rule.options[0]);
                    }
                } else if (callerType == "select-multiple") {
                    if (!$(caller).find("option:selected").val()) {
                        $.validation.isError = true;
                        promptText += _buildPromptText("该选择项必选", rule.options[0]);
                    }
                }
            }

            function _unique(caller, rule) {
                var form = $(caller).closest("form");
            }

            function _customRegex(caller, rule) {
                if (_isValueEmpty(caller)) {
                    return false;
                }

                var customRule = rule.name;
                if (customRule == "pattern") {
                    customRule = rule.options[0];
                }
                var customPT = customRule.match(/\[[^\]]+\]/g);
                if (customPT) {
                    customRule = customRule.replace(customPT[0], "");
                    customPT = customPT[0].replace(/^\[|\]$/g, "");
                }

                var pattern = $.validation.settings.allrules[customRule];
                if (!pattern) {
                    $.validation.debug("正则表达式:" + customRule + " 没有定义，请检查拼写是否正确");
                }
                pattern = eval(pattern.regex);

                if (!pattern.test($.trim($(caller).val()))) {
                    $.validation.isError = true;
                    promptText += _buildPromptText($.validation.settings.allrules[customRule].alertText, customPT);
                }
            }

            function _funcCall(caller, rule) {
                var funce = rule.options[0];

                var fn = window[funce];
                if (typeof (fn) === 'function') {
                    var fn_result = fn(caller);
                    if (fn_result.isError) {
                        $.validation.isError = true;
                        promptText += _buildPromptText(fn_result.errorInfo);
                    }
                }
            }

            function _confirm(caller, rule) {
                var confirmField = rule.options[0];

                if ($(caller).val() != $("#" + confirmField).val()) {
                    $.validation.isError = true;
                    promptText += _buildPromptText($.validation.settings.allrules[rule.name].alertText, rule.options[1]);
                } else {
                    $.validation.closePrompt($("#" + confirmField));
                }
            }

            function _lessThan(caller, rule) {
                var callerValueType = typeof $(caller);
            }

            function _length(caller, rule) {
                if (_isValueEmpty(caller)) {
                    return false;
                }

                var minL = rule.options[0];
                var maxL = rule.options[1];
                var feildLength = $.trim($(caller).val()).length;

                if (feildLength < minL || feildLength > maxL) {
                    $.validation.isError = true;
                    promptText += _buildPromptText("输入值的长度必须在" + minL + "和" + maxL + "之间", rule.options[2]);
                }
            }

            function _range(caller, rule) {
                var min = rule.options[0];
                var max = rule.options[1];

                var callerType = $(caller).attr("type");
                if (callerType == "checkbox") {
                    var groupSize = $("input[group='" + $(caller).attr("group") + "']:checked").size();
                    if (groupSize < min || groupSize > max) {
                        $.validation.isError = true;
                        promptText += _buildPromptText("必须选择" + min + "到" + max + "选项", rule.options[2]);
                    }
                }
                else {
                    if (_isValueEmpty(caller)) {
                        return false;
                    }
                    var inputValue = parseFloat($.trim($(caller).val())) || 0;
                    if (inputValue < min || inputValue > max) {
                        $.validation.isError = true;
                        promptText += _buildPromptText("输入的值必须在" + min + "到" + max + "之间", rule.options[2]);
                    }
                }
            }

            function _buildPromptText(defaultPT, customPT) {
                return customPT ? customPT : defaultPT;
            }

            function _isValueEmpty(caller) {
                return !($(caller).val() && $.trim($(caller).val()).length > 0);
            }

            return ($.validation.isError) ? $.validation.isError : false;
        },
        buildPrompt: function (caller, promptText) {
            if (!!$(caller).attr("group")) {
                caller = $("#" + $(caller).attr("group"));
            }
            $(caller).attr("errorInfo", promptText)
            $(caller).removeClass("success");
            $(caller).addClass("error");

        },

        closePrompt: function (caller) {
            if (!!$(caller).attr("group")) {
                caller = $("#" + $(caller).attr("group"));
            }
            $(caller).removeClass("error");
            $(caller).addClass("success");
            $(caller).removeAttr("errorInfo")
        },

        debug: function (error) {
            console.log(error);
        },

        validate: function (caller) {
            var stopForm = false;
            var errorInfo = "";
            $(caller).find("[validator]").not(":disabled").each(function () {
                var validationPass = $.validation.loadValidation(this);
                return (validationPass) ? stopForm = true : "";
            });
            if (stopForm) {
                $(caller).find("[errorInfo]").each(function () {
                    errorInfo += $(this).attr("errorInfo") + "\n";
                });
                $(caller).find("[errorInfo]").first().focus();
            }
            return { isError: stopForm, errorInfo: errorInfo };
        }
    }
})(jQuery);
