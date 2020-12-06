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
    $('input[name=layerSelect]').click(function () {
        (function ($element) {
            checkSubstations($element);
            showMarkers();
        }($(this)));
    });

    $('.go-back').click(() => $('.info').each(function () { $(this).addClass('d-none') }));

    function checkSubstations($element) {
        if ($element.val() == 'substation') {
            if ($element.is(':checked')) {
                $('.electric-level').each(function () { $(this).prop('checked', true); });
            }
            else {
                $('.electric-level').each(function () { $(this).prop('checked', false); });
            }
        }
        if ($element.hasClass('electric-level')) {
            if ($('.electric-level').length === $('.electric-level:checked').length) {
                $('#chkLine').prop('checked', true);
            }
            else {
                $('#chkLine').prop('checked', false);
            }
        }
    }
})(jQuery);