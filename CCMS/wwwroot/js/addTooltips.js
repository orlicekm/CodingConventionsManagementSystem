function addTooltips() {
    $('[data-toggle="tooltip"]').tooltip({
        trigger: "hover"
    });
    $('[data-toggle="tooltip"]').on("mouseleave",
        function() {
            $(this).tooltip("hide");
        });
    $('[data-toggle="tooltip"]').on("click",
        function() {
            $(this).tooltip("dispose");
        });
}