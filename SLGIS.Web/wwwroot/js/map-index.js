(function ($) {

    "use strict";

    var fullHeight = function () {

        $('.js-fullheight').css('height', $(window).height());
        $(window).resize(function () {
            $('.js-fullheight').css('height', $(window).height());
        });
    };
    fullHeight();

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
    $('#search-marker').select2({
        placeholder: 'Tìm kiếm'
    });
    $('input[name=layerSelect]').click(showMarkers);
    $('.go-back').click(() => $('.info').each(function () { $(this).addClass('d-none') }));
})(jQuery);