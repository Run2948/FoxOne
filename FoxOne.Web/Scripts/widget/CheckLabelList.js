/// <reference path="../jquery-1.8.2.js" />
/// <reference path="../common.js" />
(function (window, $) {
    var foxOne = window.foxOne;
    $("div[widget='checklabel'] .btn.btn-default").each(function (e) {
        $(this).click(function () {
            var p = $(this).parent();
            p.find("a.active").removeClass("active");
            $(this).addClass("active");
            p.find("input[type='hidden']").val($(this).attr("value"));
            var tiggerSearch=$(this).closest("[widget]").attr("ChangeTiggerSearch");
            if (tiggerSearch && tiggerSearch.length > 0) {
                $(this).closest("form").submit();
            }
        });
    });
})(window, jQuery);