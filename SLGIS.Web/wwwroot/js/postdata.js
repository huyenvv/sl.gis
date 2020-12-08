function beforeSubmit() {
    var i = 0;
    var result = "";
    $('#post-data tr').each(function () {
        var j = 0;
        var first = true;
        var html = "";
        $("input", $(this)).each(function () {
            var value = $(this).val();
            if (!value) value = 0;
            if (first) {
                html += "<input type='hidden' name='PostData.PostDataDetails[" + i + "].Hour' value='" + value + "' />";
                first = false;
            }
            else {
                var code = $(this).data("code");
                html += "<input type='hidden' name='PostData.PostDataDetails[" + i + "].Values[" + j + "].Code' value='" + code + "' />";
                html += "<input type='hidden' name='PostData.PostDataDetails[" + i + "].Values[" + j + "].Value' value='" + value + "' />";
                j++;
            }
        });
        if (html) {
            result += html;
            i++;
        }
    });
    $("#inputElement").html(result);
    return true;
}

function addRow() {
    var row = $('#post-data tr:last').html();
    var tlast = $('#post-data tr:last input:first').val();
    $('#post-data tr:last').after('<tr>' + row + '</tr>');
    $('#post-data tr:last input').each(() => {
        $(this).val(0);
    });
    if ($('#post-data tr').length > 3) {
        var t1 = $('#post-data tr input:first').val();
        var t2 = $('#post-data tr:nth-child(2) input:first').val();
        if (!!t1 && !!t2) {
            var t = parseInt(t2) - parseInt(t1);
            $('#post-data tr:last input:first').val(parseInt(tlast) + t);
        }
    }
    //$('#post-data tr:last input:first').val(100);
}