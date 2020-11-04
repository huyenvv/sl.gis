function resetInputFile(input) {
    var displayTextElement = input.next();
    var text = displayTextElement.attr('data-text');
    displayTextElement.html(text);
}

function showHideInput() {
    $(".row-item").each(function (_, element) {
        var input = $(element).find("input[type='number'], input[type='file'], input[type='text'], select");
        var inputFile = $(element).find("input[type='file']");

        var isChecked = $(element).find("input[type='checkbox']").is(":checked");
        if (isChecked) {
            input.removeAttr('disabled', 'disabled');
            inputFile.parent().removeClass('input-disabled');
        } else {
            input.attr('disabled', 'disabled');
            inputFile.parent().addClass('input-disabled');
        }
    });
}

function hideOtherCheckBox($this) {
    var group = ".box-choice-function input[type='checkbox']";
    $(group).prop("checked", false);
    $this.prop("checked", true);
}

function showSettupTab() {
    $(".box-choice-function input[type='checkbox']").each(function (_, element) {
        var btn = $($(element).attr("data-item"));

        var isChecked = $(element).is(":checked");
        if (isChecked) {
            btn.parent().show();
        } else {
            btn.parent().hide();

            if (btn.hasClass("active")) {
                $("#download-tab").click();
            }
        }
    });
}

function loadFacebookFanpages(token, callback) {
    $.ajax({
        type: "GET",
        url: "/api/Channel/GetFanpages/" + token,
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            callback(response);
        }
    });
}

function bindFacebookFanpages(fanpages) {
    var box = $("#box-fb-fanpages tbody");
    box.empty();

    $.each(fanpages, function (i, fpage) {
        var fpageName = fpage.PageSelection + " (" + fpage.id + ")";
        var item =
            "<tr class='stat even row-item'>" +
            "<td class='ac'>" +
            (i + 1) +
            "</td>" +
            "<td>" +
            "	<div class='ckbox ckbox-default'>" +
            "		<label class='ckbox'>" +
            "			<input name='Campaign.FacebookConfig.Fanpages[" + i + "].IsChecked' type='checkbox' value='true'>" +
            "           <span>&nbsp;</span>" +
            "		</label>" +
            "	</div>" +
            "</td>" +
            "<td>" +
            "	<input name='Campaign.FacebookConfig.Fanpages[" + i + "].Id' type='hidden' value='" + fpage.id + "'>" +
            "	<input name='Campaign.FacebookConfig.Fanpages[" + i + "].Token' type='hidden' value='" + fpage.token + "'>" +
            "	<input name='Campaign.FacebookConfig.Fanpages[" + i + "].Title' type='hidden' value='" + fpageName + "'>" +
            "	<input name='Campaign.FacebookConfig.Fanpages[" + i + "].Title' type='text' value='" + fpageName + "' class='form-control'>" +
            "</td>" +
            "<td>" +
            "	<label class='custom-file'>" +
            "		<input name='Campaign.FacebookConfig.Fanpages[" + i + "].TitlePath' type='hidden' value=''>" +
            "		<input accept='.txt' class='custom-file-input text-box single-line' name='Campaign.FacebookConfig.Fanpages[" + i + "].TitleFile' type='file'>" +
            "		<span class='custom-file-control' data-text='Chèn file...'>" +
            "		</span>" +
            "	</label>" +
            "</td>" +
            "<td>" +
            "	<label class='custom-file'>" +
            "		<input name='Campaign.FacebookConfig.Fanpages[" + i + "].DescriptionPath' type='hidden' value=''>" +
            "		<input accept='.txt' class='custom-file-input text-box single-line' name='Campaign.FacebookConfig.Fanpages[" + i + "].DescriptionFile' type='file'>" +
            "		<span class='custom-file-control' data-text='Chèn file...'>" +
            "		</span>" +
            "	</label>" +
            "</td>" +
            "</tr>";
        box.append(item);
    });

    $("#box-fb-fanpages .custom-file input[type='file']").each(function () {
        resetInputFile($(this));
    });

    showHideInput();
}

function getDate(element) {
    var date;
    try {
        date = $.datepicker.parseDate(dateFormat, element.value);
    } catch (error) {
        date = null;
    }

    return date;
}