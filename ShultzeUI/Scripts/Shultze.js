$(document).ready(function () {
    $("table td span.js-drop").on("click", function () {
        $(this).parents("tr").hide("fade", 500);
    });

    $("table span.js-show").on("click", function () {
        $("table tr").show("fade", 500);
    });

    $("a.js-vote.btn").on("click", function () {
        $("#dialog").dialog();
    });

    $("a.js-reload.btn").on("click", function () {
        var $this = $(this);
        $.get($this.prop("href"), function (data) {
            $(".table-placeholder").empty().html(data);
        });
        return false;
    });
});
