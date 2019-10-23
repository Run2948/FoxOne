(function ($) {
    var Widget = function (element, options) {
        this.options = options;
        this.$body = $(document.body);
        this.$element = $(element);
        this.inited = false;
    }
    Widget.DefaultOption = {

    };

    Widget.prototype.init = function () {
        if (this.inited) return;
        var e = $.Event("widget.init");
        var that = this;
        var widgetName = this.options.widget;
        this.$element.trigger(e);
        if (e.isDefaultPrevented()) return;
        jQuery.each(widgets, function () {
            alert(this.widget);
            if (this.widget == widgetName) {
                this.init(that);
            }
        });
    }

    Widget.prototype.refresh = function () {
    }

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('Widget')
            var options = $.extend({}, Widget.DefaultOption, $this.data(), typeof option == 'object' && option)
            if (!data) {
                $this.data('Widget', (data = new Widget(this, options)))
            }
            if (typeof option == 'string') data[option]()
            else
                data.init()
        });
    }
    $.extend({
        Widget: function (option) {
            $("[data-widget]").Widget(option);
        }
    });
    var old = $.fn.Widget;
    $.fn.Widget = Plugin;
    $.fn.Widget.Constructor = Widget;
    $.fn.Widget.noConflict = function () {
        $.fn.Widget = old;
        return this;
    }
})(jQuery)